using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks.Networking
{ 
    public struct NetworkListExEvent<T>
    {
        public EventType Type;
        public T Value;
        public int Index;

        public enum EventType
        {
            Add = 0,
            Insert = 1,
            Remove = 2,
            RemoveAt = 3,
            Value = 4,
            Clear = 5,
            ElementChanged = 6
        }
    }
}
