using System.Reactive;

namespace Extensions
{
    /// <summary>
    ///     Singleton class that represents nothing
    ///     Used for generics that require a class.
    ///     Checkout the struct version, Unit
    ///     <seealso cref="Unit" />
    /// </summary>
    public class Nothing
    {
        public static Nothing Instance { get; } = new Nothing();

        private Nothing()
        {
        }
    }
}