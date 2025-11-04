using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;

/// <summary>
/// Single entry representing a dismissed suggested user.
/// Maps to JSON fields: `title`, `media_list_data`, and `string_list_data`.
/// Reuses <see cref="StringListData"/> for `string_list_data` and keeps `media_list_data` as raw JsonElement until structure is provided.
/// </summary>
public class DismissedSuggestedUser
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("media_list_data")]
    public List<JsonElement> MediaListData { get; set; }

    [JsonPropertyName("string_list_data")]
    public List<StringListData> StringListData { get; set; }
}