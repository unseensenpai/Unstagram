using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;

/// <summary>
/// Root response for the user's following list.
/// Maps to JSON property `relationships_following`.
/// </summary>
public class FollowingResponse
{
    [JsonPropertyName("relationships_following")]
    public List<FollowingProfile> RelationshipsFollowing { get; set; }
}
