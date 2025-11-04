using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;

/// <summary>
/// Root model for "follow requests sent" JSON.
/// Maps to JSON property `relationships_follow_requests_sent`.
/// </summary>
public class FollowRequestsSentResponse
{
    [JsonPropertyName("relationships_follow_requests_sent")]
    public List<FollowRequestProfile> RelationshipsFollowRequestsSent { get; set; }
}
