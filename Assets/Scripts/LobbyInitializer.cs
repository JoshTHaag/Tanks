using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class LobbyInitializer : MonoBehaviour
{
    public GameRoom prefabGameRoom;
    public LobbyUI prefabLobbyUI;

    public static LobbyInitializer Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (Instance != this)
            return;

        Init();
    }

    public void Init()
    {
        if (NetworkManager.HostOnLobbyStartup)
            InitHost();
        else
            InitClient();

        NetworkManager.HostOnLobbyStartup = false;
    }

    void InitHost()
    {
        NetworkManager.Singleton.StartHost();

        // Instantiate the GameRoom first to initialize the singleton.
        var gameRoom = Instantiate(prefabGameRoom);

        var lobbyUI = Instantiate(prefabLobbyUI);

        lobbyUI.Init();

        gameRoom.Init();
    }

    void InitClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
