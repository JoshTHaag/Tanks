using System.Collections;
using System.Collections.Generic;
using System.IO;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Serialization.Pooled;

namespace Tanks.Networking
{
    public class NetworkListEx<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, INetworkVariable where T : NetworkListExElement
    {
        /// <summary>
        /// Gets the last time the variable was synced
        /// </summary>
        public float LastSyncedTime { get; internal set; }

        /// <summary>
        /// The callback to be invoked when the list is changed.
        /// </summary>
        public event OnListChangedDelegate OnListChanged;

        public readonly NetworkVariableSettings Settings = new NetworkVariableSettings();

        private readonly IList<T> m_List = new List<T>();
        private readonly List<NetworkListExEvent<T>> m_DirtyEvents = new List<NetworkListExEvent<T>>();
        private NetworkBehaviour m_NetworkBehaviour;

        /// <summary>
        /// Creates a NetworkList with the default value and settings
        /// </summary>
        public NetworkListEx() { }

        /// <summary>
        /// Creates a NetworkList with the default value and custom settings
        /// </summary>
        /// <param name="settings">The settings to use for the NetworkList</param>
        public NetworkListEx(NetworkVariableSettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// Creates a NetworkList with a custom value and the default settings
        /// </summary>
        /// <param name="value">The initial value to use for the NetworkList</param>
        public NetworkListEx(IList<T> value)
        {
            m_List = value;
        }

        /// <summary>
        /// Creates a NetworkList with a custom value and custom settings
        /// </summary>
        /// <param name="settings">The settings to use for the NetworkList</param>
        /// <param name="value">The initial value to use for the NetworkList</param>
        public NetworkListEx(NetworkVariableSettings settings, IList<T> value)
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
                if (NetworkManager.Singleton.IsServer)
                {
                    if(m_List[index] != value)
                    {
                        value.ElementChanged += OnElementChanged;

                        m_List[index] = value;

                        HandleAddListEvent(new NetworkListExEvent<T>()
                        {
                            Type = NetworkListExEvent<T>.EventType.Value,
                            Value = value,
                            Index = index
                        });
                    }
                }
            }
        }

        public int Count => m_List.Count;

        public bool IsReadOnly => m_List.IsReadOnly;

        public ushort RemoteTick
        {
            get
            {
                // TODO: Implement proper network tick for NetworkList.
                return NetworkTickSystem.NoTick;
            }
        }

        public void Add(T item)
        {
            m_List.Add(item);

            item.ElementChanged += OnElementChanged;

            HandleAddListEvent(new NetworkListExEvent<T>()
            {
                Type = NetworkListExEvent<T>.EventType.Add,
                Value = item,
                Index = m_List.Count - 1
            });
        }

        void OnElementChanged(NetworkListExElement element)
        {
            int index = m_List.IndexOf(element as T);

            HandleAddListEvent(new NetworkListExEvent<T>()
            {
                Type = NetworkListExEvent<T>.EventType.ElementChanged,
                Value = element as T,
                Index = index
            });
        }

        public bool CanClientRead(ulong clientId)
        {
            return Settings.ReadPermission == NetworkVariablePermission.Everyone
                    || (Settings.ReadPermission == NetworkVariablePermission.OwnerOnly && clientId == m_NetworkBehaviour.OwnerClientId)
                    || (Settings.ReadPermission == NetworkVariablePermission.Custom && Settings.ReadPermissionCallback != null && Settings.ReadPermissionCallback(clientId));
        }

        public bool CanClientWrite(ulong clientId)
        {
            return Settings.WritePermission == NetworkVariablePermission.Everyone
                    || (Settings.WritePermission == NetworkVariablePermission.OwnerOnly && clientId == m_NetworkBehaviour.OwnerClientId)
                    || (Settings.WritePermission == NetworkVariablePermission.Custom && Settings.WritePermissionCallback != null && Settings.WritePermissionCallback(clientId));
        }

        public void Clear()
        {
            if (NetworkManager.Singleton.IsServer) m_List.Clear();

            NetworkListExEvent<T> listEvent = new NetworkListExEvent<T>()
            {
                Type = NetworkListExEvent<T>.EventType.Clear
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

        public MLAPI.Transports.NetworkChannel GetChannel()
        {
            return Settings.SendNetworkChannel;
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
            if (NetworkManager.Singleton.IsServer) m_List.Insert(index, item);

            item.ElementChanged += OnElementChanged;

            NetworkListExEvent<T> listEvent = new NetworkListExEvent<T>()
            {
                Type = NetworkListExEvent<T>.EventType.Insert,
                Index = index,
                Value = item
            };

            HandleAddListEvent(listEvent);
        }

        public bool IsDirty()
        {
            if (m_DirtyEvents.Count == 0) return false;
            if (Settings.SendTickrate == 0) return true;
            if (Settings.SendTickrate < 0) return false;
            if (NetworkManager.Singleton.NetworkTime - LastSyncedTime >= (1f / Settings.SendTickrate)) return true;
            return false;
        }

        public bool Remove(T item)
        {
            bool removed = false;

            int index = m_List.IndexOf(item);

            if (NetworkManager.Singleton.IsServer)
            {
                removed = index != -1;
                if(removed)
                    m_List.RemoveAt(index);
            }

            if (removed)
            {
                NetworkListExEvent<T> listEvent = new NetworkListExEvent<T>()
                {
                    Type = NetworkListExEvent<T>.EventType.Remove,
                    Value = item,
                    Index = index
                };

                HandleAddListEvent(listEvent);
            }

            return removed;
        }

        public void RemoveAt(int index)
        {
            if (NetworkManager.Singleton.IsServer) m_List.RemoveAt(index);

            NetworkListExEvent<T> listEvent = new NetworkListExEvent<T>()
            {
                Type = NetworkListExEvent<T>.EventType.RemoveAt,
                Index = index
            };

            HandleAddListEvent(listEvent);
        }

        public void ResetDirty()
        {
            m_DirtyEvents.Clear();
            LastSyncedTime = NetworkManager.Singleton.NetworkTime;
        }

        public void SetNetworkBehaviour(NetworkBehaviour behaviour)
        {
            m_NetworkBehaviour = behaviour;
        }

        public void WriteDelta(Stream stream)
        {
            using (var writer = PooledNetworkWriter.Get(stream))
            {
                writer.WriteUInt16Packed((ushort)m_DirtyEvents.Count);
                for (int i = 0; i < m_DirtyEvents.Count; i++)
                {
                    writer.WriteBits((byte)m_DirtyEvents[i].Type, 3);
                    switch (m_DirtyEvents[i].Type)
                    {
                        case NetworkListExEvent<T>.EventType.Add:
                            {
                                writer.WriteObjectPacked(m_DirtyEvents[i].Value); //BOX
                            }
                            break;
                        case NetworkListExEvent<T>.EventType.Insert:
                            {
                                writer.WriteInt32Packed(m_DirtyEvents[i].Index);
                                writer.WriteObjectPacked(m_DirtyEvents[i].Value); //BOX
                            }
                            break;
                        case NetworkListExEvent<T>.EventType.Remove:
                            {
                                writer.WriteObjectPacked(m_DirtyEvents[i].Index); //BOX
                                writer.WriteObjectPacked(m_DirtyEvents[i].Value); //BOX
                            }
                            break;
                        case NetworkListExEvent<T>.EventType.RemoveAt:
                            {
                                writer.WriteInt32Packed(m_DirtyEvents[i].Index);
                            }
                            break;
                        case NetworkListExEvent<T>.EventType.Value:
                            {
                                writer.WriteInt32Packed(m_DirtyEvents[i].Index);
                                writer.WriteObjectPacked(m_DirtyEvents[i].Value); //BOX
                            }
                            break;
                        case NetworkListExEvent<T>.EventType.Clear:
                            {
                                //Nothing has to be written
                            }
                            break;
                        case NetworkListExEvent<T>.EventType.ElementChanged:
                            {
                                writer.WriteInt32Packed(m_DirtyEvents[i].Index);
                                //writer.WriteObjectPacked(m_DirtyEvents[i].Value); //BOX
                                m_List[m_DirtyEvents[i].Index].Write(writer);
                            }
                            break;
                    }
                }
            }
        }

        public void WriteField(Stream stream)
        {
            using (var writer = PooledNetworkWriter.Get(stream))
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

        private void HandleAddListEvent(NetworkListExEvent<T> listEvent)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                if (NetworkManager.Singleton.ConnectedClients.Count > 0)
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

        public void ReadField(Stream stream, ushort localTick, ushort remoteTick)
        {
            using (var reader = PooledNetworkReader.Get(stream))
            {
                m_List.Clear();
                ushort count = reader.ReadUInt16Packed();
                for (int i = 0; i < count; i++)
                {
                    m_List.Add((T)reader.ReadObjectPacked(typeof(T))); //BOX
                }
            }
        }

        public void ReadDelta(Stream stream, bool keepDirtyDelta, ushort localTick, ushort remoteTick)
        {
            using (var reader = PooledNetworkReader.Get(stream))
            {
                ushort deltaCount = reader.ReadUInt16Packed();
                for (int i = 0; i < deltaCount; i++)
                {
                    NetworkListExEvent<T>.EventType eventType = (NetworkListExEvent<T>.EventType)reader.ReadBits(3);
                    switch (eventType)
                    {
                        case NetworkListExEvent<T>.EventType.Add:
                            {
                                m_List.Add((T)reader.ReadObjectPacked(typeof(T))); //BOX

                                if (OnListChanged != null)
                                {
                                    OnListChanged(new NetworkListExEvent<T>
                                    {
                                        Type = eventType,
                                        Index = m_List.Count - 1,
                                        Value = m_List[m_List.Count - 1]
                                    });
                                }

                                if (keepDirtyDelta)
                                {
                                    m_DirtyEvents.Add(new NetworkListExEvent<T>()
                                    {
                                        Type = eventType,
                                        Index = m_List.Count - 1,
                                        Value = m_List[m_List.Count - 1]
                                    });
                                }
                            }
                            break;
                        case NetworkListExEvent<T>.EventType.Insert:
                            {
                                int index = reader.ReadInt32Packed();
                                m_List.Insert(index, (T)reader.ReadObjectPacked(typeof(T))); //BOX

                                if (OnListChanged != null)
                                {
                                    OnListChanged(new NetworkListExEvent<T>
                                    {
                                        Type = eventType,
                                        Index = index,
                                        Value = m_List[index]
                                    });
                                }

                                if (keepDirtyDelta)
                                {
                                    m_DirtyEvents.Add(new NetworkListExEvent<T>()
                                    {
                                        Type = eventType,
                                        Index = index,
                                        Value = m_List[index]
                                    });
                                }
                            }
                            break;
                        case NetworkListExEvent<T>.EventType.Remove:
                            {
                                int index = reader.ReadInt32Packed();
                                var value = m_List[index];
                                m_List.RemoveAt(index);

                                if (OnListChanged != null)
                                {
                                    OnListChanged(new NetworkListExEvent<T>
                                    {
                                        Type = eventType,
                                        Index = index,
                                        Value = value
                                    });
                                }

                                if (keepDirtyDelta)
                                {
                                    m_DirtyEvents.Add(new NetworkListExEvent<T>()
                                    {
                                        Type = eventType,
                                        Index = index,
                                        Value = value
                                    });
                                }
                            }
                            break;
                        case NetworkListExEvent<T>.EventType.RemoveAt:
                            {
                                int index = reader.ReadInt32Packed();
                                T value = m_List[index];
                                m_List.RemoveAt(index);

                                if (OnListChanged != null)
                                {
                                    OnListChanged(new NetworkListExEvent<T>
                                    {
                                        Type = eventType,
                                        Index = index,
                                        Value = value
                                    });
                                }

                                if (keepDirtyDelta)
                                {
                                    m_DirtyEvents.Add(new NetworkListExEvent<T>()
                                    {
                                        Type = eventType,
                                        Index = index,
                                        Value = value
                                    });
                                }
                            }
                            break;
                        case NetworkListExEvent<T>.EventType.Value:
                            {
                                int index = reader.ReadInt32Packed();
                                T value = (T)reader.ReadObjectPacked(typeof(T)); //BOX
                                if (index < m_List.Count) m_List[index] = value;

                                if (OnListChanged != null)
                                {
                                    OnListChanged(new NetworkListExEvent<T>
                                    {
                                        Type = eventType,
                                        Index = index,
                                        Value = value
                                    });
                                }

                                if (keepDirtyDelta)
                                {
                                    m_DirtyEvents.Add(new NetworkListExEvent<T>()
                                    {
                                        Type = eventType,
                                        Index = index,
                                        Value = value
                                    });
                                }
                            }
                            break;
                        case NetworkListExEvent<T>.EventType.Clear:
                            {
                                //Read nothing
                                m_List.Clear();

                                if (OnListChanged != null)
                                {
                                    OnListChanged(new NetworkListExEvent<T>
                                    {
                                        Type = eventType,
                                    });
                                }

                                if (keepDirtyDelta)
                                {
                                    m_DirtyEvents.Add(new NetworkListExEvent<T>()
                                    {
                                        Type = eventType
                                    });
                                }
                            }
                            break;
                        case NetworkListExEvent<T>.EventType.ElementChanged:
                            {
                                int index = reader.ReadInt32Packed();
                                
                                if (index < m_List.Count)
                                {
                                    m_List[index].Read(reader);

                                    if (OnListChanged != null)
                                    {
                                        OnListChanged(new NetworkListExEvent<T>
                                        {
                                            Type = eventType,
                                            Index = index,
                                            Value = m_List[index]
                                        });
                                    }

                                    if (keepDirtyDelta)
                                    {
                                        m_DirtyEvents.Add(new NetworkListExEvent<T>()
                                        {
                                            Type = eventType,
                                            Index = index,
                                            Value = m_List[index]
                                        });
                                    }
                                }

                            }
                            break;
                    }
                }
            }
        }

        public delegate void OnListChangedDelegate(NetworkListExEvent<T> changeEvent);
    }
}
