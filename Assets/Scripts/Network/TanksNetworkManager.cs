using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Transports;
using MLAPI.Transports.UNET;
using UnityEngine.SceneManagement;
using MLAPI.Exceptions;
using MLAPI.Logging;
using MLAPI.Profiling;
using MLAPI.Transports.Tasks;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.PlayerConnection;
using Tanks.Networking;

public class TanksNetworkManager : NetworkManager
{
    public static bool HostOnLobbyStartup = false;

    bool isStarted = false;
    bool prevIsStarted = false;

    private void Awake()
    {
        OnClientDisconnectCallback += OnDisconnect;
    }

    private void Start()
    {
        var transport = GetComponent<UNetTransport>();
        transport.Channels.Add(new UNetChannel()
        {
            Id = (NetworkChannel)TanksNetworkChannel.TerrainUpdate,
            Type = QosType.ReliableFragmentedSequenced
        });
    }

    private void Update()
    {
        isStarted = IsClient || IsServer;

        if(!isStarted && prevIsStarted)
        {
            OnShutdown();
        }

        prevIsStarted = isStarted;
    }

    void OnShutdown()
    {
        if (GameRoom.Instance) Destroy(GameRoom.Instance.gameObject);
        TanksNetworkManager.Singleton.DontDestroy = false;
        Destroy(gameObject);
        SceneManager.LoadScene("MenuScene");
    }

    void OnDisconnect(ulong id)
    {
        if(id == LocalClientId)
        {
            //if (GameRoom.Instance) Destroy(GameRoom.Instance.gameObject);
            //TanksNetworkManager.Singleton.DontDestroy = false;
            //Destroy(gameObject);
            //SceneManager.LoadScene("MenuScene");
        }
    }

    [QFSW.QC.Command("start-host")]
    public static void CommandStartHost()
    {
        TanksNetworkManager.HostOnLobbyStartup = true;
        SceneManager.LoadScene("LobbyScene");
    }

    [QFSW.QC.Command("start-client")]
    public static void CommandStartClient()
    {
        TanksNetworkManager.HostOnLobbyStartup = false;
        SceneManager.LoadScene("LobbyScene");
    }

    [QFSW.QC.Command("disconnect")]
    public static void CommandDisconnect()
    {
        if (Singleton.IsHost)
            Singleton.StopHost();
        else if (Singleton.IsClient)
            Singleton.StopClient();

        SceneManager.LoadScene("MenuScene");
    }
}
