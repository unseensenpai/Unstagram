using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;

/// <summary>
/// Root model for blocked users JSON: contains list of blocked profiles.
/// Maps to JSON property `relationships_blocked_users`.
/// </summary>
public class BlockedProfilesResponse
{
    [JsonPropertyName("relationships_blocked_users")]
    public List<BlockedProfile> RelationshipsBlockedUsers { get; set; }
}

