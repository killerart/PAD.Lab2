using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VideoChat.Shared.JsonConverters {
    public class VersionConverter : JsonConverter<Version> {
        public override Version Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            try {
                return Version.Parse(reader.GetString()!);
            } catch (Exception e) {
                throw new JsonException(e.Message, e);
            }
        }

        public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options) {
            writer.WriteStringValue(value.ToString());
        }
    }
}
