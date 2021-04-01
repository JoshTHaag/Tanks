using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
//using Tanks.Networking;
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.NetworkedVar.Collections;
using MLAPI.Messaging;

public class GameRoom : NetworkedBehaviour
{
    public NetworkedList<Player> Players { get; private set; }

    public static GameRoom Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Players = new NetworkedList<Player>();
    }

    private void Start()
    {
        if (Instance != this)
            return;
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;
    }

    private void Update()
    {
    }

    public void Init()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Host_ClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += Host_ClientDisconnected;
        GetComponent<NetworkedObject>().Spawn();
        Host_ClientConnected(NetworkManager.Singleton.ServerClientId);
    }

    public void AddNewPlayer(ulong id)
    {
        Players.Add(new Player(id));
    }

    void Host_ClientConnected(ulong id)
    {
        Debug.Log("Host_ClientConnected: " + id);

        AddNewPlayer(id);
    }

    void Host_ClientDisconnected(ulong id)
    {
        Debug.Log("Host_ClientConnected: " + id);

        for(int i = 0; i < Players.Count; ++i)
        {
            if(Players[i].id == id)
            {
                Players.RemoveAt(i);
                break;
            }
        }
    }

    [ServerRPC(RequireOwnership = false)]
    public void CmdRequestNameChange(ulong id, string name)
    {
        //var player = Players.Find(x => x.id == id);
        Player player = null;
        foreach (var p in Players)
            if (p.id == id)
                player = p;

        if (player == null)
        {
            Debug.LogError("player with id " + id + " not found");
            return;
        }

        if (player.name != name)
            player.name = name;
    }

    [QFSW.QC.Command("log-players")]
    public void ConsoleLogPlayers()
    {
        foreach (var p in Players)
        {
            Debug.Log(p);
        }
    }
}
