#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetPinProc.Domain.Json
{
    public class StringEnumConverter<T> : JsonConverter<T> where T : Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (T)Enum.Parse(typeToConvert, reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
