using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;

/// <summary>
/// Single blocked profile entry.
/// </summary>
public class BlockedProfile
{
    /// <summary>
    /// Profile title (usually the username).
    /// Maps to JSON property `title`.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// Additional string list data for the profile.
    /// Maps to JSON property `string_list_data`.
    /// </summary>
    [JsonPropertyName("string_list_data")]
    public List<StringListData> StringListData { get; set; }
}
