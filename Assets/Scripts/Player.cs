using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.Serialization;
using System.Reflection;
using System.Linq;

[Serializable]
public class Player : NetworkListExElement
{
    [SerializeField] private ulong id;
    [SerializeField] private string name;

    public ulong Id
    { 
        get
        {
            return id;
        }
        set
        {
            if(id != value)
            {
                id = value;
                OnElementChanged(this);
            }
        }
    }

    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
            OnElementChanged(this);
        }
    }

    public Player() { }

    public Player(ulong id)
    {
        Id = id;
        Name = "Player" + (id + 1);
    }

    public override string ToString()
    {
        return string.Format("ID: {0}, Name: {1}", Id, Name);
    }

    public static implicit operator bool(Player obj)
    {
        return obj != null;
    }
}
