
namespace Tanks.Networking
{
    /// <summary>
    /// Struct containing event information about changes to a NetworkedList.
    /// </summary>
    /// <typeparam name="T">The type for the list that the event is about</typeparam>
    public struct NetworkedListEvent<T>
    {
        /// <summary>
        /// Enum representing the operation made to the list.
        /// </summary>
        public EventType eventType;

        /// <summary>
        /// The value changed, added or removed if available.
        /// </summary>
        public T value;

        /// <summary>
        /// The index changed, added or removed if available.
        /// </summary>
        public int index;

        /// <summary>
        /// Enum representing the different operations available for triggering an event.
        /// </summary>
        public enum EventType
        {
            Add = 0,
            Insert = 1,
            Remove = 2,
            RemoveAt = 3,
            Value = 4,
            Clear = 5
        }
    }
}