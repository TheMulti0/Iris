﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Updates.Facebook
{
    internal class StringToLongConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
            => long.Parse(reader.GetString());

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value);
    }
}