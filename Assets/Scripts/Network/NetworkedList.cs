using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.Serialization.Pooled;

namespace Tanks.Networking
{
    public class NetworkedList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, INetworkedVar
    {
        /// <summary>
        /// Gets the last time the variable was synced
        /// </summary>
        public float LastSyncedTime { get; internal set; }

        /// <summary>
        /// The callback to be invoked when the list is changed.
        /// </summary>
        public event OnListChangedDelegate OnListChanged;

        public readonly NetworkedVarSettings Settings = new NetworkedVarSettings();

        private readonly IList<T> m_List = new List<T>();
        private readonly List<NetworkedListEvent<T>> m_DirtyEvents = new List<NetworkedListEvent<T>>();
        private NetworkedBehaviour m_NetworkBehaviour;

        /// <summary>
        /// Creates a NetworkList with the default value and settings
        /// </summary>
        public NetworkedList() { }

        /// <summary>
        /// Creates a NetworkList with the default value and custom settings
        /// </summary>
        /// <param name="settings">The settings to use for the NetworkList</param>
        public NetworkedList(NetworkedVarSettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// Creates a NetworkList with a custom value and the default settings
        /// </summary>
        /// <param name="value">The initial value to use for the NetworkList</param>
        public NetworkedList(IList<T> value)
        {
            m_List = value;
        }

        /// <summary>
        /// Creates a NetworkList with a custom value and custom settings
        /// </summary>
        /// <param name="settings">The settings to use for the NetworkList</param>
        /// <param name="value">The initial value to use for the NetworkList</param>
        public NetworkedList(NetworkedVarSettings settings, IList<T> value)
        {
            Settings = settings;
            m_List = value;
        }

        public T this[int index]
        {
            get
            {
                return m_List[index];
            }
            set
            {
                if (NetworkingManager.Singleton.IsServer)
                    m_List[index] = value;

                HandleAddListEvent(new NetworkedListEvent<T>()
                {
                    eventType = NetworkedListEvent<T>.EventType.Value,
                    value = value,
                    index = index
                });          
            }
        }

        public int Count => m_List.Count;

        public bool IsReadOnly => m_List.IsReadOnly;

        public void Add(T item)
        {
            m_List.Add(item);

            HandleAddListEvent(new NetworkedListEvent<T>()
            {
                eventType = NetworkedListEvent<T>.EventType.Add,
                value = item,
                index = m_List.Count - 1
            });
        }

        public bool CanClientRead(ulong clientId)
        {
            return Settings.ReadPermission == NetworkedVarPermission.Everyone
                    || (Settings.ReadPermission == NetworkedVarPermission.OwnerOnly && clientId == m_NetworkBehaviour.OwnerClientId)
                    || (Settings.ReadPermission == NetworkedVarPermission.Custom && Settings.ReadPermissionCallback != null && Settings.ReadPermissionCallback(clientId));
        }

        public bool CanClientWrite(ulong clientId)
        {
            return Settings.WritePermission == NetworkedVarPermission.Everyone
                    || (Settings.WritePermission == NetworkedVarPermission.OwnerOnly && clientId == m_NetworkBehaviour.OwnerClientId)
                    || (Settings.WritePermission == NetworkedVarPermission.Custom && Settings.WritePermissionCallback != null && Settings.WritePermissionCallback(clientId));
        }

        public void Clear()
        {
            if (NetworkingManager.Singleton.IsServer) m_List.Clear();

            NetworkedListEvent<T> listEvent = new NetworkedListEvent<T>()
            {
                eventType = NetworkedListEvent<T>.EventType.Clear
            };

            HandleAddListEvent(listEvent);
        }

        public bool Contains(T item)
        {
            return m_List.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_List.CopyTo(array, arrayIndex);
        }

        public string GetChannel()
        {
            return Settings.SendChannel;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return m_List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (NetworkingManager.Singleton.IsServer) m_List.Insert(index, item);

            NetworkedListEvent<T> listEvent = new NetworkedListEvent<T>()
            {
                eventType = NetworkedListEvent<T>.EventType.Insert,
                index = index,
                value = item
            };

            HandleAddListEvent(listEvent);
        }

        public bool IsDirty()
        {
            if (m_DirtyEvents.Count == 0) return false;
            if (Settings.SendTickrate == 0) return true;
            if (Settings.SendTickrate < 0) return false;
            if (NetworkingManager.Singleton.NetworkTime - LastSyncedTime >= (1f / Settings.SendTickrate)) return true;
            return false;
        }

        public void ReadDelta(Stream stream, bool keepDirtyDelta)
        {
            using (var reader = PooledBitReader.Get(stream))
            {
                ushort deltaCount = reader.ReadUInt16Packed();
                for (int i = 0; i < deltaCount; i++)
                {
                    NetworkedListEvent<T>.EventType eventType = (NetworkedListEvent<T>.EventType)reader.ReadBits(3);
                    switch (eventType)
                    {
                        case NetworkedListEvent<T>.EventType.Add:
                            {
                                m_List.Add((T)reader.ReadObjectPacked(typeof(T))); //BOX

                                if (OnListChanged != null)
                                {
                                    OnListChanged(new NetworkedListEvent<T>
                                    {
                                        eventType = eventType,
                                        index = m_List.Count - 1,
                                        value = m_List[m_List.Count - 1]
                                    });
                                }

                                if (keepDirtyDelta)
                                {
                                    m_DirtyEvents.Add(new NetworkedListEvent<T>()
                                    {
                                        eventType = eventType,
                                        index = m_List.Count - 1,
                                        value = m_List[m_List.Count - 1]
                                    });
                                }
                            }
                            break;
                        case NetworkedListEvent<T>.EventType.Insert:
                            {
                                int index = reader.ReadInt32Packed();
                                m_List.Insert(index, (T)reader.ReadObjectPacked(typeof(T))); //BOX

                                if (OnListChanged != null)
                                {
                                    OnListChanged(new NetworkedListEvent<T>
                                    {
                                        eventType = eventType,
                                        index = index,
                                        value = m_List[index]
                                    });
                                }

                                if (keepDirtyDelta)
                                {
                                    m_DirtyEvents.Add(new NetworkedListEvent<T>()
                                    {
                                        eventType = eventType,
                                        index = index,
                                        value = m_List[index]
                                    });
                                }
                            }
                            break;
                        case NetworkedListEvent<T>.EventType.Remove:
                            {
                                T value = (T)reader.ReadObjectPacked(typeof(T)); //BOX
                                int index = m_List.IndexOf(value);
                                m_List.RemoveAt(index);

                                if (OnListChanged != null)
                                {
                                    OnListChanged(new NetworkedListEvent<T>
                                    {
                                        eventType = eventType,
                                        index = index,
                                        value = value
                                    });
                                }

                                if (keepDirtyDelta)
                                {
                                    m_DirtyEvents.Add(new NetworkedListEvent<T>()
                                    {
                                        eventType = eventType,
                                        index = index,
                                        value = value
                                    });
                                }
                            }
                            break;
                        case NetworkedListEvent<T>.EventType.RemoveAt:
                            {
                                int index = reader.ReadInt32Packed();
                                T value = m_List[index];
                                m_List.RemoveAt(index);

                                if (OnListChanged != null)
                                {
                                    OnListChanged(new NetworkedListEvent<T>
                                    {
                                        eventType = eventType,
                                        index = index,
                                        value = value
                                    });
                                }

                                if (keepDirtyDelta)
                                {
                                    m_DirtyEvents.Add(new NetworkedListEvent<T>()
                                    {
                                        eventType = eventType,
                                        index = index,
                                        value = value
                                    });
                                }
                            }
                            break;
                        case NetworkedListEvent<T>.EventType.Value:
                            {
                                int index = reader.ReadInt32Packed();
                                T value = (T)reader.ReadObjectPacked(typeof(T)); //BOX
                                if (index < m_List.Count) m_List[index] = value;

                                if (OnListChanged != null)
                                {
                                    OnListChanged(new NetworkedListEvent<T>
                                    {
                                        eventType = eventType,
                                        index = index,
                                        value = value
                                    });
                                }

                                if (keepDirtyDelta)
                                {
                                    m_DirtyEvents.Add(new NetworkedListEvent<T>()
                                    {
                                        eventType = eventType,
                                        index = index,
                                        value = value
                                    });
                                }
                            }
                            break;
                        case NetworkedListEvent<T>.EventType.Clear:
                            {
                                //Read nothing
                                m_List.Clear();

                                if (OnListChanged != null)
                                {
                                    OnListChanged(new NetworkedListEvent<T>
                                    {
                                        eventType = eventType,
                                    });
                                }

                                if (keepDirtyDelta)
                                {
                                    m_DirtyEvents.Add(new NetworkedListEvent<T>()
                                    {
                                        eventType = eventType
                                    });
                                }
                            }
                            break;
                    }
                }
            }
        }

        public void ReadField(Stream stream)
        {
            using (var reader = PooledBitReader.Get(stream))
            {
                m_List.Clear();
                ushort count = reader.ReadUInt16Packed();
                for (int i = 0; i < count; i++)
                {
                    m_List.Add((T)reader.ReadObjectPacked(typeof(T))); //BOX
                }
            }
        }

        public bool Remove(T item)
        {
            bool removed = false;
            if (NetworkingManager.Singleton.IsServer) removed = m_List.Remove(item);

            if(removed)
            {
                NetworkedListEvent<T> listEvent = new NetworkedListEvent<T>()
                {
                    eventType = NetworkedListEvent<T>.EventType.Remove,
                    value = item
                };

                HandleAddListEvent(listEvent);
            }

            return removed;
        }

        public void RemoveAt(int index)
        {
            if (NetworkingManager.Singleton.IsServer) m_List.RemoveAt(index);

            NetworkedListEvent<T> listEvent = new NetworkedListEvent<T>()
            {
                eventType = NetworkedListEvent<T>.EventType.RemoveAt,
                index = index
            };

            HandleAddListEvent(listEvent);
        }

        public void ResetDirty()
        {
            m_DirtyEvents.Clear();
            LastSyncedTime = NetworkingManager.Singleton.NetworkTime;
        }

        public void SetNetworkedBehaviour(NetworkedBehaviour behaviour)
        {
            m_NetworkBehaviour = behaviour;
        }

        public void WriteDelta(Stream stream)
        {
            using (var writer = PooledBitWriter.Get(stream))
            {
                writer.WriteUInt16Packed((ushort)m_DirtyEvents.Count);
                for (int i = 0; i < m_DirtyEvents.Count; i++)
                {
                    writer.WriteBits((byte)m_DirtyEvents[i].eventType, 3);
                    switch (m_DirtyEvents[i].eventType)
                    {
                        case NetworkedListEvent<T>.EventType.Add:
                            {
                                writer.WriteObjectPacked(m_DirtyEvents[i].value); //BOX
                            }
                            break;
                        case NetworkedListEvent<T>.EventType.Insert:
                            {
                                writer.WriteInt32Packed(m_DirtyEvents[i].index);
                                writer.WriteObjectPacked(m_DirtyEvents[i].value); //BOX
                            }
                            break;
                        case NetworkedListEvent<T>.EventType.Remove:
                            {
                                writer.WriteObjectPacked(m_DirtyEvents[i].value); //BOX
                            }
                            break;
                        case NetworkedListEvent<T>.EventType.RemoveAt:
                            {
                                writer.WriteInt32Packed(m_DirtyEvents[i].index);
                            }
                            break;
                        case NetworkedListEvent<T>.EventType.Value:
                            {
                                writer.WriteInt32Packed(m_DirtyEvents[i].index);
                                writer.WriteObjectPacked(m_DirtyEvents[i].value); //BOX
                            }
                            break;
                        case NetworkedListEvent<T>.EventType.Clear:
                            {
                                //Nothing has to be written
                            }
                            break;
                    }
                }
            }
        }

        public void WriteField(Stream stream)
        {
            using (var writer = PooledBitWriter.Get(stream))
            {
                writer.WriteUInt16Packed((ushort)m_List.Count);
                for (int i = 0; i < m_List.Count; i++)
                {
                    writer.WriteObjectPacked(m_List[i]); //BOX
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        private void HandleAddListEvent(NetworkedListEvent<T> listEvent)
        {
            if (NetworkingManager.Singleton.IsServer)
            {
                if (NetworkingManager.Singleton.ConnectedClients.Count > 0)
                {
                    m_DirtyEvents.Add(listEvent);
                }

                OnListChanged?.Invoke(listEvent);
            }
            else
            {
                m_DirtyEvents.Add(listEvent);
            }
        }

        public T Find(System.Predicate<T> match)
        {
            for (int i = 0; i < Count; ++i)
            {
                if (match.Invoke(this[i]))
                    return this[i];
            }

            return default;
        }

        public delegate void OnListChangedDelegate(NetworkedListEvent<T> changeEvent);
    }
}
