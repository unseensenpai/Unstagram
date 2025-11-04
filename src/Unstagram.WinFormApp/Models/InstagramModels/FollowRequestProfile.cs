using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;

/// <summary>
/// Single entry representing a profile you sent a follow request to.
/// - `Title` maps to JSON `title`.
/// - `MediaListData` kept as raw JSON because structure wasn't provided.
/// - `StringListData` reuses existing <see cref="StringListData"/> model (contains href, value, timestamp).
/// </summary>
public class FollowRequestProfile
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("media_list_data")]
    public List<JsonElement> MediaListData { get; set; }

    [JsonPropertyName("string_list_data")]
    public List<StringListData> StringListData { get; set; }
}
