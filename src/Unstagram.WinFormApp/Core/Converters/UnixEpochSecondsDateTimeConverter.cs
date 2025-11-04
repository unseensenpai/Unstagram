using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Unstagram.WinFormApp.Core.Converters;


public class UnixEpochSecondsDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            long seconds = reader.GetInt64();
            return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (long.TryParse(s, out var seconds))
                return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;

            if (DateTime.TryParse(s, out var dt))
                return dt;
        }

        throw new JsonException("Invalid timestamp value for Unix epoch seconds.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var seconds = new DateTimeOffset(value.ToUniversalTime()).ToUnixTimeSeconds();
        writer.WriteNumberValue(seconds);
    }
}
