using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.Serialization;

[Serializable]
public class Player : AutoNetworkSerializable
{
    public ulong id;
    public string name;

    public Player() { }

    public Player(ulong id)
    {
        this.id = id;
        name = "Player" + (id + 1);
    }

    public override string ToString()
    {
        return string.Format("ID: {0}, Name: {1}", id, name);
    }

    public static implicit operator bool(Player obj)
    {
        return obj != null;
    }

    //public void Read(Stream stream)
    //{
    //    //byte[] buffer = new byte[sizeof(ulong)];
    //    //stream.Read(buffer, 0, buffer.Length);
    //    //id = BitConverter.ToUInt64(buffer, 0);

    //    BinaryFormatter bf = new BinaryFormatter();
    //    stream.Seek(0, SeekOrigin.Begin);
    //    try
    //    {
    //        Player p = bf.Deserialize(stream) as Player;

    //        id = p.id;
    //        name = p.name;
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError(ex);
    //    }
    //}

    //public void Write(Stream stream)
    //{
    //    BinaryFormatter bf = new BinaryFormatter();
    //    bf.Serialize(stream, this);
    //}
}
