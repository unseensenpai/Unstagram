using System;
using System.Text.Json.Serialization;
using Unstagram.WinFormApp.Core.Converters;

namespace Unstagram.WinFormApp.Models.InstagramModels;

/// <summary>
/// Generic string-list item used in several Instagram models.
/// - `href` is the link to the profile or resource.
/// - `value` is an optional textual value (e.g., username).
/// - `timestamp` is an epoch seconds value converted to <see cref="DateTime"/> (UTC) by the converter.
/// </summary>
public class StringListData
{
    /// <summary>
    /// The href link (maps to JSON `href`).
    /// </summary>
    [JsonPropertyName("href")]
    public string Href { get; set; }

    /// <summary>
    /// Optional value field (maps to JSON `value`).
    /// May be null for entries that do not include it.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; }

    /// <summary>
    /// Timestamp in Unix epoch seconds. Deserialized to UTC <see cref="DateTime"/>.
    /// Uses <see cref="UnixEpochSecondsDateTimeConverter"/>.
    /// </summary>
    [JsonPropertyName("timestamp"), JsonConverter(typeof(UnixEpochSecondsDateTimeConverter))]
    public DateTime Timestamp { get; set; }
}


