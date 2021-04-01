using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.NetworkedVar.Collections;
//using Tanks.Networking;


public class LobbyUI : MonoBehaviour
{
    public Transform playerListContent;
    public PlayerElement prefabPlayerElement;
  
    public List<PlayerElement> playerElements;

    public static LobbyUI Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (Instance != this)
            return;       
    }

    public void Init()
    {
        playerElements = new List<PlayerElement>();
        NetworkManager.HostOnLobbyStartup = false;
        GameRoom.Instance.Players.OnListChanged += OnPlayerListChanged;
        GetComponent<NetworkedObject>().Spawn();
    }

    void OnPlayerListChanged(NetworkedListEvent<Player> listEvent)
    {
        if(NetworkManager.Singleton.IsHost)
        {
            if(listEvent.eventType == NetworkedListEvent<Player>.EventType.Add)
            {
                var playerElement = Instantiate(prefabPlayerElement);
                playerElement.Spawn(listEvent.value.id);
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;
    }

    private void LateUpdate()
    {
        
    }
}
