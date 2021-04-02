using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.NetworkVariable.Collections;

public class LobbyUI : MonoBehaviour
{
    public Transform playerListContent;
    public PlayerElement prefabPlayerElement;
  
    public List<PlayerElement> playerElements;

    public bool IsInitialized { get; private set; }

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
        GameRoom.Instance.Players.OnListChanged += OnPlayerListChanged;

        for(int x = 0; x < GameRoom.Instance.Players.Count; ++x)
        {
            PlayerElement newPlayer = null;
            for (int y = 0; y < playerElements.Count; ++y)
            {
                if(playerElements[y].player == GameRoom.Instance.Players[x])
                {
                    newPlayer = playerElements[y];
                    break;
                }
            }

            if(newPlayer == null)
            {
                var playerElement = Instantiate(prefabPlayerElement);
                playerElement.Init(GameRoom.Instance.Players[x]);
            }
        }
    }

    void OnPlayerListChanged(NetworkListEvent<Player> listEvent)
    {
        if (listEvent.Type == NetworkListEvent<Player>.EventType.Add)
        {
            Player newPlayer = GameRoom.Instance.Players.Find(x => x == listEvent.Value);

            if (playerElements.Find(x => x.player == newPlayer))
                return;

            var playerElement = Instantiate(prefabPlayerElement);
            playerElements.Add(playerElement);
            playerElement.Init(newPlayer);
        }
        else if (listEvent.Type == NetworkListEvent<Player>.EventType.Remove)
        {
            var removeElement = playerElements.Find(x => x.player == listEvent.Value);

            if (!removeElement)
                return;

            removeElement.Destroy();
            playerElements.Remove(removeElement);
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
