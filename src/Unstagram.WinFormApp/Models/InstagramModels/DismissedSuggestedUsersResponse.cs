using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;

/// <summary>
/// Root response for dismissed suggested users JSON.
/// Maps to JSON property `relationships_dismissed_suggested_users`.
/// </summary>
public class DismissedSuggestedUsersResponse
{
    [JsonPropertyName("relationships_dismissed_suggested_users")]
    public List<DismissedSuggestedUser> RelationshipsDismissedSuggestedUsers { get; set; }
}
