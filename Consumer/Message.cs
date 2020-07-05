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
            return $"Key = {Key}, Value = {Value}, Timestamp = {Timestamp:f}, Topic = {Topic}";
        }
    }
}