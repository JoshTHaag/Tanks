using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class LobbyInitializer : MonoBehaviour
{
    public GameRoom prefabGameRoom;

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
        if(TanksNetworkManager.HostOnLobbyStartup)
        {
            TanksNetworkManager.Singleton.StartHost();

            // Instantiate the GameRoom first to initialize the singleton.
            var gameRoom = Instantiate(prefabGameRoom);

            var lobbyUI = FindObjectOfType<LobbyUI>();

            lobbyUI.Init();

            gameRoom.HostInit();
        }
        else
        {
            TanksNetworkManager.Singleton.StartClient();
        }

        TanksNetworkManager.HostOnLobbyStartup = false;
    }
}
