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
        NetworkManager.HostOnLobbyStartup = true;
        SceneManager.LoadScene("LobbyScene");
    }

    public void Connect()
    {
        NetworkManager.HostOnLobbyStartup = false;
        SceneManager.LoadScene("LobbyScene");
    }
}
