using SteamGameTracker.DataTransferObjects;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SteamGameTracker.JsonConverters
{
    public class SystemRequirementsConverter : JsonConverter<SystemRequirementsDTO>
    {
        public override SystemRequirementsDTO Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Convert(ref reader, options);
        }

        private static SystemRequirementsDTO Convert(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                // Skip array
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) { }
                return new SystemRequirementsDTO();
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                return JsonSerializer.Deserialize<SystemRequirementsDTO>(ref reader, options);
            }

            throw new JsonException($"Unexpected token: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, SystemRequirementsDTO value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
