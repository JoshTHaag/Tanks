using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Host()
    {
        TanksNetworkManager.HostOnLobbyStartup = true;
        SceneManager.LoadScene("LobbyScene");
    }

    public void Connect()
    {
        TanksNetworkManager.HostOnLobbyStartup = false;
        SceneManager.LoadScene("LobbyScene");
    }
}
