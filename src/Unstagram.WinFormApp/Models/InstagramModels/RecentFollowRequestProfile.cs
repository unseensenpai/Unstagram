using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;

/// <summary>
/// Single entry representing a recent follow request.
/// Maps to JSON: `title`, `media_list_data`, and `string_list_data`.
/// </summary>
public class RecentFollowRequestProfile
{
    /// <summary>
    /// Profile title (usually the username).
    /// Maps to JSON property `title`.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// Raw media list data for the profile.
    /// Kept as <see cref="JsonElement"/> until you provide concrete structure.
    /// Maps to JSON property `media_list_data`.
    /// </summary>
    [JsonPropertyName("media_list_data")]
    public List<JsonElement> MediaListData { get; set; }

    /// <summary>
    /// Additional string list data for the profile (uses <see cref="StringListData"/>).
    /// Maps to JSON property `string_list_data`.
    /// </summary>
    [JsonPropertyName("string_list_data")]
    public List<StringListData> StringListData { get; set; }
}