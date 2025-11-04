using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;
#region Hidden stories models

/// <summary>
/// Root model for "hide stories from" JSON: contains list of profiles whose stories are hidden.
/// Maps to JSON property `relationships_hide_stories_from`.
/// </summary>
public class HiddenStoriesProfilesResponse
{
    [JsonPropertyName("relationships_hide_stories_from")]
    public List<HiddenStoryProfile> RelationshipsHideStoriesFrom { get; set; }
}

#endregion

