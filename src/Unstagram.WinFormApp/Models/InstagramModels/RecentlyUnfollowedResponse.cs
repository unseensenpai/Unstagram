using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;

/// <summary>
/// Root response for recently unfollowed users.
/// Maps to JSON property `relationships_unfollowed_users`.
/// </summary>
public class RecentlyUnfollowedResponse
{
    [JsonPropertyName("relationships_unfollowed_users")]
    public List<RecentlyUnfollowedProfile> RelationshipsUnfollowedUsers { get; set; }
}
