using System;

namespace Consumer
{
    public class Message<TKey, TValue>
    {
        public Optional<TKey> Key { get; set; }

        public Optional<TValue> Value { get; set; }

        public DateTime Timestamp { get; set; }

        public string Topic { get; set; }

        public override string ToString()
        {
            string keyString = Key.HasValue 
                ? $"Key = {Key}, " 
                : string.Empty;
            
            string valueString = Value.HasValue 
                ? $"Value = {Value}, " 
                : string.Empty;

            return $"{keyString}{valueString}Timestamp = {Timestamp:f}, Topic = {Topic}"; ;
        }
    }
}