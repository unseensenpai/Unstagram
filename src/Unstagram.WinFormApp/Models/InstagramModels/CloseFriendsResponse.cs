using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Models.InstagramModels;

/// <summary>
/// Root response for close friends list.
/// Maps to JSON property `relationships_close_friends`.
/// </summary>
public class CloseFriendsResponse
{
    [JsonPropertyName("relationships_close_friends")]
    public List<CloseFriendProfile> RelationshipsCloseFriends { get; set; }
}
