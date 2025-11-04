using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;


/// <summary>
/// Single profile entry for hidden stories.
/// Contains optional media list data and string list data.
/// </summary>
public class HiddenStoryProfile
{
    /// <summary>
    /// Profile title (usually the username).
    /// Maps to JSON property `title`.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// Raw media list data for the profile.
    /// Structure may vary; preserved as <see cref="JsonElement"/> for now.
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
