using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;

/// <summary>
/// Root response for follow requests the user has received.
/// Maps to JSON property `relationships_follow_requests_received`.
/// </summary>
public class FollowRequestsReceivedResponse
{
    [JsonPropertyName("relationships_follow_requests_received")]
    public List<FollowRequestReceivedProfile> RelationshipsFollowRequestsReceived { get; set; }
}
