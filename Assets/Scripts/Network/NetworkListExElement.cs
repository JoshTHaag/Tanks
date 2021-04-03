using System;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.Serialization;
using System.Reflection;
using System.Linq;

public abstract class NetworkListExElement : INetworkSerializable
{
    private static Dictionary<Type, FieldInfo[]> s_FieldCache = new Dictionary<Type, FieldInfo[]>();

    public event OnElementChangedDelegate ElementChanged;

    public virtual void OnElementChanged(NetworkListExElement changedElement)
    {
        UnityEngine.Networking.NetworkTransport.QueueMessageForSending(0, 0, 0, null, 0, out byte error);

        ElementChanged?.Invoke(changedElement);
    }

    public void Write(NetworkWriter writer)
    {
        var fields = GetFields(GetType());
        for (int i = 0; i < fields.Length; i++)
        {
            writer.WriteObjectPacked(fields[i].GetValue(this));
        }
    }

    public void Read(NetworkReader reader)
    {
        var fields = GetFields(GetType());
        for (int i = 0; i < fields.Length; i++)
        {
            fields[i].SetValue(this, reader.ReadObjectPacked(fields[i].FieldType));
        }
    }

    public void NetworkSerialize(NetworkSerializer serializer)
    {
        if (serializer.IsReading)
        {
            Read(serializer.Reader);
        }
        else
        {
            Write(serializer.Writer);
        }
    }

    static FieldInfo[] GetFields(Type type)
    {
        if (s_FieldCache.ContainsKey(type)) return s_FieldCache[type];

        var fields = typeof(Player)
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(x => (x.IsPublic || x.GetCustomAttributes(typeof(SerializeField), true).Length > 0))
            .OrderBy(x => x.Name, StringComparer.Ordinal).ToArray();

        s_FieldCache.Add(type, fields);

        return fields;
    }


    public delegate void OnElementChangedDelegate(NetworkListExElement changedElement);
}
