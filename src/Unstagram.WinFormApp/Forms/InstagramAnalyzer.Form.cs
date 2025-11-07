using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Unstagram.WinFormApp.Models.Enums;
using Unstagram.WinFormApp.Models.InstagramModels;

namespace Unstagram.WinFormApp.Forms
{
    public partial class InstagramAnalyzerForm : DevExpress.XtraEditors.XtraForm
    {
        private List<StringListData> _followers = [];
        private List<StringListData> _following = [];

        private DevExpress.XtraTab.XtraTabPage XTP_TheyFollowIDont, XTP_IFollowTheyDont;
        private GridControl GC_TheyFollowIDont, GC_IFollowTheyDont;
        private GridView GV_TheyFollowIDont, GV_IFollowTheyDont;


        public InstagramAnalyzerForm()
        {
            InitializeComponent();
            InitializeUIValeus();
        }

        public void InitializeUIValeus()
        {
            // Ensure default selection = JSON
            try
            {
                if (RG_ContentTypeSelector != null)
                {
                    RG_ContentTypeSelector.SelectedIndex = 0; // 0 = Json based on designer items
                }
            }
            catch
            {
                // ignore - safe default
            }
        }

        private void RG_ContentTypeSelector_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // No immediate UI change required here; we determine filter on open files.
            // Keep method so designer event stays wired.
        }

        private void ACE_OpenFiles_Click(object sender, System.EventArgs e)
        {
            // Open file dialog (multi-select). Use radio selection to decide filter (default JSON).
            using var ofd = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Select analyze files",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                CheckFileExists = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            try
            {
                int sel = RG_ContentTypeSelector.SelectedIndex;
                // Keep JSON/HTML default but explicitly expose ZIP so user can pick archives
                ofd.Filter = sel == 1
                    ? "Supported files (*.html;*.htm;*.zip)|*.html;*.htm;*.zip|HTML files (*.html;*.htm)|*.html;*.htm|Zip files (*.zip)|*.zip|All files (*.*)|*.*"
                    : "Supported files (*.json;*.zip)|*.json;*.zip|JSON files (*.json)|*.json|Zip files (*.zip)|*.zip|All files (*.*)|*.*";
            }
            catch
            {
                // ignore and keep default filter
            }

            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            // Prepare set of filenames lower-cased for quick matching
            var selectedFileNames = ofd.FileNames.Select(Path.GetFileName).Select(n => n.ToLowerInvariant()).ToHashSet();

            // Reset all tabs to "not loaded" state, then mark ones present
            ResetAllTabsToNotLoaded();

            foreach (var filePath in ofd.FileNames)
            {
                try
                {
                    var ext = Path.GetExtension(filePath).ToLowerInvariant();
                    if (ext == ".zip")
                    {
                        // Process archive entries and load any known files inside
                        ProcessZipFile(filePath);
                        continue;
                    }

                    var lowerName = Path.GetFileName(filePath).ToLowerInvariant();
                    // determine content type from radio
                    var contentType = AnalyzeFileType.Json;
                    try
                    {
                        contentType = RG_ContentTypeSelector.SelectedIndex == 1 ? AnalyzeFileType.Html : AnalyzeFileType.Json;
                    }
                    catch { /* ignore */ }

                    var mapping = GetTargetGridByFileName(lowerName);
                    if (mapping == null) continue; // no matching tab

                    ManuelLoadFileIntoGrid(filePath, mapping.Grid, mapping.TabPage, contentType);
                }
                catch (Exception ex)
                {
                    // Keep overall load going; surface minimal feedback
                    System.Diagnostics.Debug.WriteLine($"Failed to load {filePath}: {ex}");
                }
            }
        }

        private AnalyzeFileType DetectContentTypeForPath(string path, AnalyzeFileType radioDefault)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".html" or ".htm" => AnalyzeFileType.Html,
                ".json" => AnalyzeFileType.Json,
                _ => radioDefault
            };
        }

        private void ProcessZipFile(string zipPath)
        {
            try
            {
                using var za = ZipFile.OpenRead(zipPath);
                ProcessZipArchive(za, Path.GetFileName(zipPath), 0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Zip processing failed for {zipPath}: {ex}");
            }
        }

        private void ProcessZipArchive(ZipArchive za, string archiveDisplayName, int depth)
        {
            const int MaxDepth = 3;
            const long MaxEntrySize = 150L * 1024 * 1024; // 150 MB
            const int MaxEntriesPerArchive = 5000;

            if (depth > MaxDepth) return;

            int processedEntries = 0;

            foreach (var entry in za.Entries)
            {
                if (++processedEntries > MaxEntriesPerArchive) break;

                // skip directory entries
                if (string.IsNullOrEmpty(entry.Name)) continue;

                // Prevent ZipSlip-like paths
                var normalized = entry.FullName.Replace('\\', '/');
                if (normalized.Contains("..")) continue;

                var entryFileNameLower = Path.GetFileName(entry.Name).ToLowerInvariant();
                var entryFullLower = normalized.ToLowerInvariant();

                // If entry is itself a zip -> recurse
                var entryExt = Path.GetExtension(entry.Name).ToLowerInvariant();
                try
                {
                    if (entryExt == ".zip")
                    {
                        try
                        {
                            using var entryStream = entry.Open();
                            using var ms = new MemoryStream();
                            entryStream.CopyTo(ms);
                            ms.Position = 0;

                            using var nested = new ZipArchive(ms, ZipArchiveMode.Read, leaveOpen: false);
                            ProcessZipArchive(nested, entry.FullName, depth + 1);
                        }
                        catch (InvalidDataException)
                        {
                            // Not a valid zip or encrypted; skip
                            continue;
                        }
                        catch (Exception exNested)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to process nested zip {entry.FullName}: {exNested}");
                            continue;
                        }

                        continue;
                    }

                    // Try to map to a grid (filename or full path)
                    var mapping = GetTargetGridByFileName(entryFileNameLower) ?? GetTargetGridByFileName(entryFullLower);
                    if (mapping == null) continue;

                    // Safety: avoid huge entries
                    if (entry.Length > MaxEntrySize) continue;

                    using var s = entry.Open();
                    using var sr = new StreamReader(s);
                    var content = sr.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(content)) continue;

                    var contentType = DetectContentTypeForPath(entry.Name, AnalyzeFileType.Json);

                    // reuse existing loader via temp file (or refactor ManuelLoadFileIntoGrid to accept string/stream)
                    var extForTemp = Path.GetExtension(entry.Name);
                    if (string.IsNullOrEmpty(extForTemp)) extForTemp = contentType == AnalyzeFileType.Html ? ".html" : ".json";
                    var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + extForTemp);
                    File.WriteAllText(temp, content);

                    try
                    {
                        ManuelLoadFileIntoGrid(temp, mapping.Grid, mapping.TabPage, contentType);
                    }
                    finally
                    {
                        try { File.Delete(temp); } catch { /* ignore */ }
                    }
                }
                catch (Exception exEntry)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to process zip entry {entry.FullName} in {archiveDisplayName}: {exEntry}");
                }
            }
        }

        private void GV_DoubleClick(object sender, System.EventArgs e)
        {
            if (sender is not GridView view) return;

            string href = null;

            // 1) If the row object is StringListData, use it
            var rowObj = view.GetFocusedRow();
            if (rowObj is StringListData sld)
            {
                href = sld.Href;
                if (string.IsNullOrWhiteSpace(href) && !string.IsNullOrWhiteSpace(sld.Value))
                    href = GuessHrefFromUsername(sld.Value);
            }

            // 2) Fallback: try to read "Href" cell
            if (string.IsNullOrWhiteSpace(href))
            {
                try
                {
                    var cell = view.GetFocusedRowCellValue("Href");
                    href = cell?.ToString();
                }
                catch { /* ignore */ }
            }

            // 3) Fallback: try to read "Value" and build instagram URL
            if (string.IsNullOrWhiteSpace(href))
            {
                try
                {
                    var cell = view.GetFocusedRowCellValue("Value");
                    var username = cell?.ToString();
                    if (!string.IsNullOrWhiteSpace(username))
                        href = GuessHrefFromUsername(username);
                }
                catch { /* ignore */ }
            }

            if (!string.IsNullOrWhiteSpace(href))
            {
                try
                {
                    var psi = new System.Diagnostics.ProcessStartInfo(href) { UseShellExecute = true };
                    System.Diagnostics.Process.Start(psi);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to open link: {ex}");
                }
            }
        }

        private void GV_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (sender is not GridView view) return;

            if (e.Control && e.KeyCode == Keys.C)
            {
                try
                {
                    object value = view.GetFocusedValue();
                    // If no focused value try focused column cell specifically
                    if (value == null && view.FocusedColumn != null)
                    {
                        value = view.GetRowCellValue(view.FocusedRowHandle, view.FocusedColumn);
                    }

                    if (value != null)
                    {
                        string text = value.ToString();
                        Clipboard.SetText(text);
                        // optionally show a short feedback (comment out if undesired)
                        // XtraMessageBox.Show(this, "Copied to clipboard", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // mark handled to avoid further processing
                    e.Handled = true;
                }
                catch (Exception ex)
                {
                    // swallow but inform in debug / minimal UI
                    System.Diagnostics.Debug.WriteLine($"Copy to clipboard failed: {ex}");
                }
            }
        }

        // Helper types & methods

        private record GridMapping(DevExpress.XtraTab.XtraTabPage TabPage, GridControl Grid, GridView View);

        private GridMapping GetTargetGridByFileName(string lowerFileName)
        {
            if (string.IsNullOrWhiteSpace(lowerFileName)) return null;

            // Normalize: convert any non-word char to underscore, collapse multiples, trim edges.
            var normalized = GetNonWordRegex().Replace(lowerFileName, "_");
            normalized = GetUnderscoreRegex().Replace(normalized, "_").Trim('_');

            // Build token set for robust matching (handles "followers_1", "you've", etc.)
            var tokens = normalized.Split('_').Where(t => !string.IsNullOrWhiteSpace(t))
                                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

            bool Has(params string[] parts) => parts.All(p => tokens.Contains(p));

            // Map well-known instagram export fragments to tab/grid
            if (tokens.Contains("followers")) return new GridMapping(XTP_Followers, GC_Followers, GV_Followers);
            if (tokens.Contains("following")) return new GridMapping(XTP_Following, GC_Following, GV_Following);
            if (Has("recently", "unfollowed") || normalized.Contains("recentlyunfollowed")) return new GridMapping(XTP_RecentlyUnfollowed, GC_RecentlyUnfollowed, GV_RecentlyUnfollowed);

            // follow requests you've received -> tokens like ["follow","requests","youve","received"]
            if (Has("follow", "requests", "received") || normalized.Contains("follow_requests_you")) return new GridMapping(XTP_FollowRequestYouHaveReceived, GC_FollowRequestsYouHaveRecieved, GV_FollowRequestsYouHaveRecieved);

            if (Has("close", "friends")) return new GridMapping(XTP_CloseFriends, GC_CloseFriends, GV_CloseFriends);
            if (Has("hide", "story", "from") || Has("hide", "story")) return new GridMapping(XTP_HideStoryFrom, GC_HideStoryFrom, GV_HideStoryFrom);
            if (Has("pending", "follow", "requests") || Has("pending", "follow")) return new GridMapping(XTP_PendingFollowRequests, GC_PendingFollowRequests, GV_PendingFollowRequests);
            if (Has("recent", "follow", "requests") || normalized.Contains("recent_follow_requests")) return new GridMapping(XTP_RecentFollowRequests, GC_RecentFollowRequests, GV_RecentFollowRequests);
            if (Has("removed", "suggestions")) return new GridMapping(XTP_RemovedSuggestions, GC_RemovedSuggestions, GV_RemovedSuggestions);
            if (tokens.Contains("blocked") || tokens.Contains("blocked_profiles")) return new GridMapping(XTP_BlockedProfiles, GC_Blocked, GV_Blocked);

            // Final fallbacks (substring on normalized) to catch odd cases
            if (normalized.Contains("blocked")) return new GridMapping(XTP_BlockedProfiles, GC_Blocked, GV_Blocked);
            if (normalized.Contains("followers")) return new GridMapping(XTP_Followers, GC_Followers, GV_Followers);
            if (normalized.Contains("following")) return new GridMapping(XTP_Following, GC_Following, GV_Following);

            return null;
        }

        private void ManuelLoadFileIntoGrid(string filePath, GridControl grid, DevExpress.XtraTab.XtraTabPage tab, AnalyzeFileType contentType)
        {
            if (grid == null || tab == null) return;

            string content = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(content)) return;

            object dataSource = null;

            if (contentType == AnalyzeFileType.Html)
            {
                var dt = new DataTable();
                dt.Columns.Add("Html", typeof(string));
                dt.Rows.Add(content);
                dataSource = dt;
            }
            else
            {
                try
                {
                    // Map grid -> expected root array property and whether root is array
                    // Based on sample JSON keys you shared
                    string rootArrayProp = null;
                    bool rootIsArray = false;

                    if (grid.Name == GC_Followers.Name)
                    {
                        rootIsArray = true; // Followers JSON is a plain array
                    }
                    else if (grid.Name == GC_Following.Name)
                    {
                        rootArrayProp = "relationships_following";
                    }
                    else if (grid.Name == GC_Blocked.Name)
                    {
                        rootArrayProp = "relationships_blocked_users";
                    }
                    else if (grid.Name == GC_CloseFriends.Name)
                    {
                        rootArrayProp = "relationships_close_friends";
                    }
                    else if (grid.Name == GC_FollowRequestsYouHaveRecieved.Name)
                    {
                        rootArrayProp = "relationships_follow_requests_received";
                    }
                    else if (grid.Name == GC_PendingFollowRequests.Name)
                    {
                        rootArrayProp = "relationships_follow_requests_sent";
                    }
                    else if (grid.Name == GC_RecentFollowRequests.Name)
                    {
                        rootArrayProp = "relationships_permanent_follow_requests";
                    }
                    else if (grid.Name == GC_RecentlyUnfollowed.Name)
                    {
                        rootArrayProp = "relationships_unfollowed_users";
                    }
                    else if (grid.Name == GC_RemovedSuggestions.Name)
                    {
                        rootArrayProp = "relationships_dismissed_suggested_users";
                    }
                    else if (grid.Name == GC_HideStoryFrom.Name)
                    {
                        rootArrayProp = "relationships_hide_stories_from";
                    }

                    var list = ExtractStringListData(content, rootArrayProp, rootIsArray);

                    // Special handling: ensure username/value when missing for following/blocked (title carries username)
                    if (grid.Name == GC_Following.Name || grid.Name == GC_Blocked.Name)
                    {
                        // if Value still empty but we can derive from Href, fill it
                        foreach (var item in list)
                        {
                            if (string.IsNullOrWhiteSpace(item.Value) && !string.IsNullOrWhiteSpace(item.Href))
                            {
                                item.Value = ExtractUsernameFromHref(item.Href) ?? item.Value;
                            }
                            if (string.IsNullOrWhiteSpace(item.Href) && !string.IsNullOrWhiteSpace(item.Value))
                            {
                                item.Href = GuessHrefFromUsername(item.Value);
                            }
                        }
                    }

                    // Keep followers/following lists for diffs
                    if (grid.Name == GC_Followers.Name)
                    {
                        _followers = list;
                        EnsureDiffTabsCreated();
                        UpdateDiffTabs();
                    }
                    else if (grid.Name == GC_Following.Name)
                    {
                        _following = list;
                        EnsureDiffTabsCreated();
                        UpdateDiffTabs();
                    }

                    dataSource = list;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Load/parse failed for {filePath}: {ex}");
                }
            }

            // Fallback to raw if nothing parsed
            if (dataSource == null)
            {
                var dt = new System.Data.DataTable();
                dt.Columns.Add("Raw", typeof(string));
                dt.Rows.Add(content);
                dataSource = dt;
            }

            grid.DataSource = dataSource;
            grid.Refresh();
            ApplyFooterCount(grid);

            grid.Enabled = true;
            MarkGridLoadState(grid, true);
            MarkTabHeaderLoaded(tab, true);
        }

        // Robust extractor for your JSON shapes -> List<StringListData>
        private List<StringListData> ExtractStringListData(
            string json,
            string arrayPropName,
            bool rootIsArray)
        {
            var results = new List<StringListData>();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            JsonElement arrayNode;

            if (rootIsArray)
            {
                if (root.ValueKind != JsonValueKind.Array)
                    return results;
                arrayNode = root;
            }
            else
            {
                if (root.ValueKind != JsonValueKind.Object)
                    return results;

                if (string.IsNullOrWhiteSpace(arrayPropName))
                {
                    // Find first array property as fallback
                    var firstArray = root.EnumerateObject().FirstOrDefault(p => p.Value.ValueKind == JsonValueKind.Array);
                    if (firstArray.Value.ValueKind != JsonValueKind.Array)
                        return results;
                    arrayNode = firstArray.Value;
                }
                else
                {
                    if (!root.TryGetProperty(arrayPropName, out arrayNode) || arrayNode.ValueKind != JsonValueKind.Array)
                        return results;
                }
            }

            foreach (var profile in arrayNode.EnumerateArray())
            {
                string title = ExtractTitle(profile);

                // Prefer string_list_data if exists
                if (profile.ValueKind == JsonValueKind.Object && profile.TryGetProperty("string_list_data", out var sldArr) && sldArr.ValueKind == JsonValueKind.Array)
                {
                    foreach (var sld in sldArr.EnumerateArray())
                    {
                        var item = new StringListData();

                        if (sld.ValueKind == JsonValueKind.Object)
                        {
                            if (sld.TryGetProperty("href", out var href)) item.Href = href.GetString();
                            if (sld.TryGetProperty("value", out var val)) item.Value = val.GetString();
                            if (sld.TryGetProperty("timestamp", out var ts))
                            {
                                // timestamp is Unix seconds
                                if (ts.ValueKind == JsonValueKind.Number && ts.TryGetInt64(out var t))
                                {
                                    item.Timestamp = DateTime.UnixEpoch.AddSeconds(t);
                                }
                            }
                        }

                        // Fill missing value/href from title if necessary
                        if (string.IsNullOrWhiteSpace(item.Value) && !string.IsNullOrWhiteSpace(title))
                            item.Value = title;
                        if (string.IsNullOrWhiteSpace(item.Href) && !string.IsNullOrWhiteSpace(item.Value))
                            item.Href = GuessHrefFromUsername(item.Value);

                        // If still no value, attempt to derive username from href
                        if (string.IsNullOrWhiteSpace(item.Value) && !string.IsNullOrWhiteSpace(item.Href))
                            item.Value = ExtractUsernameFromHref(item.Href) ?? item.Value;

                        results.Add(item);
                    }
                }
                else
                {
                    // No string_list_data; synthesize from title if present
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        results.Add(new StringListData
                        {
                            Value = title,
                            Href = GuessHrefFromUsername(title),
                            Timestamp = default
                        });
                    }
                }
            }

            return results;
        }

        private static string ExtractTitle(JsonElement profile)
        {
            if (profile.ValueKind != JsonValueKind.Object) return null;
            if (profile.TryGetProperty("title", out var t))
                return t.GetString();
            return null;
        }

        private static string ExtractUsernameFromHref(string href)
        {
            if (string.IsNullOrWhiteSpace(href)) return null;
            try
            {
                var uri = new Uri(href);
                var seg = uri.AbsolutePath.Trim('/'); // _u/username or username[/...]
                if (seg.StartsWith("_u/", StringComparison.OrdinalIgnoreCase))
                    seg = seg.Substring(3);
                if (seg.Contains('/')) seg = seg.Split('/')[0];
                return string.IsNullOrWhiteSpace(seg) ? null : seg;
            }
            catch
            {
                var last = href.TrimEnd('/').Split('/').LastOrDefault();
                if (string.Equals(last, "_u", StringComparison.OrdinalIgnoreCase))
                    return null;
                return last;
            }
        }

        private void ResetAllTabsToNotLoaded()
        {
            // Helper to mark all known grids as not loaded (disabled, red-ish)
            void DisableGrid(GridControl g, DevExpress.XtraTab.XtraTabPage p, string originalText)
            {
                if (g == null || p == null) return;
                g.DataSource = null;
                g.Enabled = false;

                // Use appearance-based coloring so skins don't override
                MarkGridLoadState(g, false);

                // Restore tab header and color
                if (!string.IsNullOrEmpty(originalText))
                    p.Text = originalText;

                MarkTabHeaderLoaded(p, false);
            }

            // Main tabs (designer)
            DisableGrid(GC_Followers, XTP_Followers, "Followers");
            DisableGrid(GC_Following, XTP_Following, "Following");
            DisableGrid(GC_RecentlyUnfollowed, XTP_RecentlyUnfollowed, "Recently Unfollowed");
            DisableGrid(GC_FollowRequestsYouHaveRecieved, XTP_FollowRequestYouHaveReceived, "Follow Requests You Have Recieved");
            DisableGrid(GC_CloseFriends, XTP_CloseFriends, "Close Friends");
            DisableGrid(GC_HideStoryFrom, XTP_HideStoryFrom, "Hide Story From");
            DisableGrid(GC_PendingFollowRequests, XTP_PendingFollowRequests, "Pending Follow Requests");
            DisableGrid(GC_RecentFollowRequests, XTP_RecentFollowRequests, "Recent Follow Requests");
            DisableGrid(GC_RemovedSuggestions, XTP_RemovedSuggestions, "Removed Suggestions");
            DisableGrid(GC_Blocked, XTP_BlockedProfiles, "Blocked");

            // Runtime diff tabs (if created)
            if (XTP_TheyFollowIDont != null && GC_TheyFollowIDont != null)
                DisableGrid(GC_TheyFollowIDont, XTP_TheyFollowIDont, "They Follow Me (I Don't)");

            if (XTP_IFollowTheyDont != null && GC_IFollowTheyDont != null)
                DisableGrid(GC_IFollowTheyDont, XTP_IFollowTheyDont, "I Follow (They Don't)");

            // Clear cached sets so next diff calc starts clean
            _followers = new();
            _following = new();
        }

        private DataTable JsonToDataTable(string json)
        {
            var doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            // If root is object and contains a property whose value is an array, pick the first array property
            if (root.ValueKind == JsonValueKind.Object)
            {
                var firstArrayProp = root.EnumerateObject().FirstOrDefault(p => p.Value.ValueKind == JsonValueKind.Array);
                if (firstArrayProp.Value.ValueKind == JsonValueKind.Array)
                {
                    root = firstArrayProp.Value;
                }
                else
                {
                    // If no array property found but root itself has properties that look like list entries, wrap root in array
                    // fallback: try to treat object as single-row table
                    return JsonObjectToDataTable(root);
                }
            }

            if (root.ValueKind == JsonValueKind.Array)
            {
                var array = root;
                // If array of primitives (string, number...), create single column "Value"
                if (!array.EnumerateArray().Any())
                {
                    var empty = new DataTable();
                    empty.Columns.Add("Value", typeof(string));
                    return empty;
                }

                var first = array.EnumerateArray().First();
                if (first.ValueKind != JsonValueKind.Object)
                {
                    var dt = new DataTable();
                    dt.Columns.Add("Value", typeof(string));
                    foreach (var item in array.EnumerateArray())
                    {
                        var row = dt.NewRow();
                        row["Value"] = item.ToString();
                        dt.Rows.Add(row);
                    }
                    return dt;
                }

                // Collect all property names across elements
                var props = array.EnumerateArray()
                    .SelectMany(el => el.EnumerateObject().Select(p => p.Name))
                    .Distinct()
                    .ToList();

                var table = new DataTable();
                foreach (var p in props)
                {
                    table.Columns.Add(p, typeof(string));
                }

                foreach (var el in array.EnumerateArray())
                {
                    var row = table.NewRow();
                    foreach (var col in props)
                    {
                        if (el.TryGetProperty(col, out var propVal))
                        {
                            if (propVal.ValueKind == JsonValueKind.Object || propVal.ValueKind == JsonValueKind.Array)
                            {
                                row[col] = propVal.GetRawText();
                            }
                            else if (propVal.ValueKind == JsonValueKind.Null || propVal.ValueKind == JsonValueKind.Undefined)
                            {
                                row[col] = DBNull.Value;
                            }
                            else
                            {
                                row[col] = propVal.ToString();
                            }
                        }
                        else
                        {
                            row[col] = DBNull.Value;
                        }
                    }
                    table.Rows.Add(row);
                }

                return table;
            }

            // fallback: if root not array/object, just return single-column table
            var fallback = new DataTable();
            fallback.Columns.Add("Value", typeof(string));
            fallback.Rows.Add(root.ToString());
            return fallback;
        }

        private DataTable JsonObjectToDataTable(JsonElement obj)
        {
            // Create a single-row table with properties as columns
            var table = new DataTable();
            foreach (var prop in obj.EnumerateObject())
            {
                table.Columns.Add(prop.Name, typeof(string));
            }

            var row = table.NewRow();
            foreach (var prop in obj.EnumerateObject())
            {
                var val = prop.Value;
                if (val.ValueKind == JsonValueKind.Object || val.ValueKind == JsonValueKind.Array)
                    row[prop.Name] = val.GetRawText();
                else if (val.ValueKind == JsonValueKind.Null || val.ValueKind == JsonValueKind.Undefined)
                    row[prop.Name] = DBNull.Value;
                else
                    row[prop.Name] = val.ToString();
            }

            table.Rows.Add(row);
            return table;
        }


        private static void MarkGridLoadState(GridControl grid, bool loaded)
        {
            if (grid?.MainView is GridView gv)
            {
                var color = loaded ? Color.FromArgb(228, 255, 233) : Color.FromArgb(255, 228, 225);
                gv.Appearance.Empty.BackColor = color;
                gv.Appearance.Empty.Options.UseBackColor = true;
                gv.Appearance.Row.BackColor = color;
                gv.Appearance.Row.Options.UseBackColor = true;
            }
        }
        private static void MarkTabHeaderLoaded(DevExpress.XtraTab.XtraTabPage tab, bool loaded)
        {
            if (tab == null) return;
            var baseText = tab.Text.Replace(" (Loaded)", string.Empty);
            tab.Text = loaded ? $"{baseText} (Loaded)" : baseText;
            tab.Appearance.Header.ForeColor = loaded ? Color.Green : Color.DarkRed;
            tab.Appearance.Header.Options.UseForeColor = true;
        }

        // Create diff tabs at runtime (insert at beginning)
        private void EnsureDiffTabsCreated()
        {
            if (XTP_TheyFollowIDont == null)
            {
                XTP_TheyFollowIDont = new DevExpress.XtraTab.XtraTabPage { Name = "XTP_TheyFollowIDont", Text = "They Follow Me (I Don't)" };
                GC_TheyFollowIDont = new GridControl { Name = "GC_TheyFollowIDont", Dock = DockStyle.Fill };
                GV_TheyFollowIDont = new GridView { Name = "GV_TheyFollowIDont" };
                GC_TheyFollowIDont.MainView = GV_TheyFollowIDont;
                GC_TheyFollowIDont.ViewCollection.Add(GV_TheyFollowIDont);
                GV_TheyFollowIDont.DoubleClick += GV_DoubleClick;
                GV_TheyFollowIDont.KeyDown += GV_KeyDown;
                GV_TheyFollowIDont.OptionsBehavior.Editable = false;
                GV_TheyFollowIDont.OptionsBehavior.ReadOnly = true;
                GV_TheyFollowIDont.OptionsView.ShowFooter = true;
                XTP_TheyFollowIDont.Controls.Add(GC_TheyFollowIDont);
                XTC_Analyzes.TabPages.Insert(0, XTP_TheyFollowIDont);
                MarkGridLoadState(GC_TheyFollowIDont, false);
                MarkTabHeaderLoaded(XTP_TheyFollowIDont, false);
            }
            if (XTP_IFollowTheyDont == null)
            {
                XTP_IFollowTheyDont = new DevExpress.XtraTab.XtraTabPage { Name = "XTP_IFollowTheyDont", Text = "I Follow (They Don't)" };
                GC_IFollowTheyDont = new GridControl { Name = "GC_IFollowTheyDont", Dock = DockStyle.Fill };
                GV_IFollowTheyDont = new GridView { Name = "GV_IFollowTheyDont" };
                GC_IFollowTheyDont.MainView = GV_IFollowTheyDont;
                GC_IFollowTheyDont.ViewCollection.Add(GV_IFollowTheyDont);
                GV_IFollowTheyDont.DoubleClick += GV_DoubleClick;
                GV_IFollowTheyDont.KeyDown += GV_KeyDown;
                GV_IFollowTheyDont.OptionsBehavior.Editable = false;
                GV_IFollowTheyDont.OptionsBehavior.ReadOnly = true;
                GV_IFollowTheyDont.OptionsView.ShowFooter = true;
                XTP_IFollowTheyDont.Controls.Add(GC_IFollowTheyDont);
                XTC_Analyzes.TabPages.Insert(1, XTP_IFollowTheyDont);
                MarkGridLoadState(GC_IFollowTheyDont, false);
                MarkTabHeaderLoaded(XTP_IFollowTheyDont, false);
            }
        }

        private void ApplyFooterCount(GridControl grid, string preferredField = null)
        {
            if (grid?.MainView is not GridView gv) return;

            // Ensure footer visible
            gv.OptionsView.ShowFooter = true;

            // Prefer supplied field; otherwise try common names
            var candidates = new List<string>();
            if (!string.IsNullOrWhiteSpace(preferredField)) candidates.Add(preferredField);
            candidates.AddRange(new[] { "Value", "Href" });

            // Find a matching column (FieldName or Name) or fallback to first visible column
            var cols = gv.Columns.OfType<DevExpress.XtraGrid.Columns.GridColumn>().Where(c => c.Visible).ToList();
            DevExpress.XtraGrid.Columns.GridColumn target = null;

            foreach (var c in cols)
            {
                if (candidates.Any(p => string.Equals(c.FieldName, p, StringComparison.OrdinalIgnoreCase) || string.Equals(c.Name, p, StringComparison.OrdinalIgnoreCase)))
                {
                    target = c;
                    break;
                }
            }

            if (target == null)
                target = cols.FirstOrDefault();

            if (target == null) return;

            // Clear existing summaries for that column and add a Count summary
            try
            {
                target.Summary.Clear();
                target.Summary.Add(DevExpress.Data.SummaryItemType.Count, target.FieldName ?? target.Name, "Count: {0:n0}");

            }
            catch
            {
                // ignore summary failures for unusual column setups
            }
        }

        private static string GuessHrefFromUsername(string username)
            => string.IsNullOrWhiteSpace(username) ? null : $"https://www.instagram.com/{username.TrimEnd('/')}/";

        private static string GetUsernameSafe(StringListData s)
        {
            if (s == null) return null;
            if (!string.IsNullOrWhiteSpace(s.Value)) return s.Value.Trim();
            if (!string.IsNullOrWhiteSpace(s.Href))
            {
                try
                {
                    var uri = new Uri(s.Href);
                    var seg = uri.AbsolutePath.Trim('/').Split('/')[0];
                    return seg;
                }
                catch
                {
                    return s.Href.TrimEnd('/').Split('/').LastOrDefault();
                }
            }
            return null;
        }
        private void UpdateDiffTabs()
        {
            if (XTC_Analyzes == null) return;
            EnsureDiffTabsCreated();

            var followersSet = _followers.Select(GetUsernameSafe).Where(s => !string.IsNullOrWhiteSpace(s)).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var followingSet = _following.Select(GetUsernameSafe).Where(s => !string.IsNullOrWhiteSpace(s)).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var theyFollowIDont = _followers.Where(s => !followingSet.Contains(GetUsernameSafe(s))).ToList();
            var iFollowTheyDont = _following.Where(s => !followersSet.Contains(GetUsernameSafe(s))).ToList();

            GC_TheyFollowIDont.DataSource = theyFollowIDont;
            GC_TheyFollowIDont.Refresh();
            ApplyFooterCount(GC_TheyFollowIDont);
            MarkGridLoadState(GC_TheyFollowIDont, theyFollowIDont.Count > 0);
            MarkTabHeaderLoaded(XTP_TheyFollowIDont, theyFollowIDont.Count > 0);

            GC_IFollowTheyDont.DataSource = iFollowTheyDont;
            GC_IFollowTheyDont.Refresh();
            ApplyFooterCount(GC_IFollowTheyDont);
            MarkGridLoadState(GC_IFollowTheyDont, iFollowTheyDont.Count > 0);
            MarkTabHeaderLoaded(XTP_IFollowTheyDont, iFollowTheyDont.Count > 0);
        }


        [GeneratedRegex(@"[^\w]")]
        private static partial Regex GetNonWordRegex();

        [GeneratedRegex(@"_+")]
        private static partial Regex GetUnderscoreRegex();
    }
}