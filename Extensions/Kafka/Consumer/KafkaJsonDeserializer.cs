﻿using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Kafka.Public;

namespace Extensions
{
    internal class KafkaJsonDeserializer<T> : IDeserializer
    {
        private static readonly StringDeserializer StringDeserializer = new StringDeserializer();
        private readonly JsonSerializerOptions _options;

        public KafkaJsonDeserializer(JsonSerializerOptions options)
        {
            if (options == null)
            {
                options = new JsonSerializerOptions();
            }
            
            _options = options;

            _options.IgnoreNullValues = true;
            
            if (_options.Converters.All(
                    converter => converter.GetType() != typeof(DateTimeConverter)))
            {
                _options.Converters.Add(new DateTimeConverter());
            }
        }

        public object Deserialize(MemoryStream fromStream, int length)
        {
            string json = "";
            try
            {
                json = StringDeserializer.Deserialize(fromStream, length).ToString();
                return JsonSerializer.Deserialize<T>(
                    json,
                    _options);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to deserialize Kafka message, this is the string message (might be null):\n{json}" +
                                  "\n" +
                                  $"{e}");
                return null;
            }
        }
    }
}