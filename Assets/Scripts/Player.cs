using System;
using UnityEngine;

[Serializable]
public class Player : NetworkListExElement
{
    [SerializeField] private ulong id;
    [SerializeField] private string name;
    [SerializeField] private bool isReady;
    [SerializeField] private bool isHost;

    public ulong Id
    { 
        get { return id; }
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
        get { return name; }
        set
        {
            if(name != value)
            {
                name = value;
                OnElementChanged(this);
            }
        }
    }

    public bool IsReady
    {
        get { return isReady; }
        set
        {
            if (isReady != value)
            {
                isReady = value;
                OnElementChanged(this);
            }
        }
    }

    public bool IsHost
    {
        get { return isHost; }
        set
        {
            if (isHost != value)
            {
                isHost = value;
                OnElementChanged(this);
            }
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
