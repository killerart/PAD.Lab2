using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lab2.Shared.JsonConverters {
    public class DateConverter : JsonConverter<DateTime?> {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            return reader.GetDateTime().Date;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options) {
            writer.WriteStringValue(value?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }
    }
}
