using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using UnityEngine.SceneManagement;

public class TanksNetworkManager : NetworkManager
{
    public static bool HostOnLobbyStartup = false;

    private void Awake()
    {
        OnClientDisconnectCallback += OnDisconnect;
    }

    void OnDisconnect(ulong id)
    {
        if(!IsHost)
        {
            SceneManager.LoadScene("MenuScene");
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
    }
}
