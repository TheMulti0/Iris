namespace Extensions
{
    public class Optional<T>
    {
        public bool HasValue { get; set; }

        public T Value { get; set; }

        private Optional(bool hasValue, T value)
        {
            HasValue = hasValue;
            Value = value;
        }

        public static Optional<T> WithValue(T value)
            => new Optional<T>(true, value);

        public static Optional<T> Empty()
            => new Optional<T>(false, default);

        public static Optional<T> CreateIfInstanceOf(object obj)
        {
            if (obj is T tObj)
            {
                return WithValue(tObj);
            }
            return Empty();
        }

        public override string ToString()
        {
            string optional = $"Optional<{typeof(T)}>";

            return HasValue 
                ? $"{optional}: {Value}" 
                : $"Empty {optional}";
        }
    }
}