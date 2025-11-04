using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;

/// <summary>
/// Root response for recent (permanent) follow requests.
/// Maps to JSON property `relationships_permanent_follow_requests`.
/// </summary>
public class RecentFollowRequestsResponse
{
    [JsonPropertyName("relationships_permanent_follow_requests")]
    public List<RecentFollowRequestProfile> RelationshipsPermanentFollowRequests { get; set; }
}
