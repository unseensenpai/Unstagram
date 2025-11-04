using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;

/// <summary>
/// Single follower entry as found in `followers_1` JSON.
/// The JSON file is an array of this object; deserialize with `List<FollowerProfile>`.
/// Maps to JSON fields: `title`, `media_list_data`, `string_list_data`.
/// </summary>
public class FollowerProfile
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("media_list_data")]
    public List<JsonElement> MediaListData { get; set; }

    [JsonPropertyName("string_list_data")]
    public List<StringListData> StringListData { get; set; }
}