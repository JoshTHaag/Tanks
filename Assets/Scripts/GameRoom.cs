using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using MLAPI.Messaging;
using Tanks.Networking;
using UnityEngine.SceneManagement;
using MLAPI.Transports;
using MLAPI.Configuration;

public class GameRoom : NetworkBehaviour
{
    public NetworkListEx<Player> Players { get; private set; }

    public static GameRoom Instance { get; private set; }

    public System.Action onSingletonInitialized = delegate { };

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

        Players = new NetworkListEx<Player>();

        onSingletonInitialized();
    }

    private void Start()
    {
        if (Instance != this)
            return;

        if(!IsHost)
        {
            ClientInit();
        }
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        Instance = null;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var p in Players)
            {
                Debug.Log(p);
            }
        }
    }

    public void HostInit()
    {
        if(Players == null)
            Players = new NetworkListEx<Player>();

        TanksNetworkManager.Singleton.OnClientConnectedCallback += Host_ClientConnected;
        TanksNetworkManager.Singleton.OnClientDisconnectCallback += Host_ClientDisconnected;
        GetComponent<NetworkObject>().Spawn();
        Host_ClientConnected(TanksNetworkManager.Singleton.ServerClientId);
    }

    public void ClientInit()
    {
        var lobbyUI = FindObjectOfType<LobbyUI>();
        if (lobbyUI && !lobbyUI.IsInitialized)
            lobbyUI.Init();
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
        Debug.Log("Host_ClientDisconnected: " + id);

        var disconnectedPlayer = Players.Find(x => x.Id == id);

        if (disconnectedPlayer)
        {
            Players.Remove(disconnectedPlayer);
        }
    }

    public void StartGame()
    {
        var loadProg = MLAPI.SceneManagement.NetworkSceneManager.SwitchScene("GameScene");

        loadProg.OnComplete += (timedOut) => 
        {
            //GameManager.Instance.NetworkObject.Spawn(null, true);
        };
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestNameChange_ServerRpc(string name, ServerRpcParams rpcParams = default)
    {
        var player = Players.Find(x => x.Id == rpcParams.Receive.SenderClientId);

        if (player == null)
        {
            Debug.LogError("player with id " + rpcParams.Receive.SenderClientId + " not found");
            return;
        }

        if (player.Name != name)
            player.Name = name;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerReady_ServerRpc(ServerRpcParams rpcParams = default)
    {
        var player = Players.Find(x => x.Id == rpcParams.Receive.SenderClientId);

        if (player == null)
        {
            Debug.LogError("player with id " + rpcParams.Receive.SenderClientId + " not found");
            return;
        }

        player.IsReady = !player.IsReady;
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
