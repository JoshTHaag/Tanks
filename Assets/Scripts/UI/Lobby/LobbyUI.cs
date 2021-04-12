using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using MLAPI;
using Tanks.Networking;

public class LobbyUI : MonoBehaviour
{
    public Transform playerListContent;
    public PlayerElement prefabPlayerElement;
    public Button startButton;
    public Button readyButton;

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

        if(TanksNetworkManager.Singleton.IsHost)
        {
            startButton.gameObject.SetActive(true);
            startButton.interactable = false;
            readyButton.gameObject.SetActive(false);
        }
        else
        {
            readyButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
        }

        for (int x = 0; x < GameRoom.Instance.Players.Count; ++x)
        {
            PlayerElement newPlayerElement = null;
            for (int y = 0; y < playerElements.Count; ++y)
            {
                if (playerElements[y].player == GameRoom.Instance.Players[x])
                {
                    newPlayerElement = playerElements[y];
                    break;
                }
            }

            if (newPlayerElement == null)
            {
                var playerElement = Instantiate(prefabPlayerElement);
                playerElements.Add(playerElement);
                playerElement.Init(GameRoom.Instance.Players[x]);
            }
        }
    }

    void OnPlayerListChanged(NetworkListExEvent<Player> listEvent)
    {
        if (listEvent.Type == NetworkListExEvent<Player>.EventType.Add)
        {
            Player newPlayer = listEvent.Value;
            newPlayer.IsHost = newPlayer.Id == TanksNetworkManager.Singleton.ServerClientId;
            if(newPlayer.IsHost)
            {
                newPlayer.IsReady = true;
            }

            if (playerElements.Find(x => x.player == newPlayer))
                return;

            var playerElement = Instantiate(prefabPlayerElement);
            playerElements.Add(playerElement);
            playerElement.Init(newPlayer);

            if(TanksNetworkManager.Singleton.IsHost)
            {
                startButton.interactable = GetAreAllPlayersReady();
            }
        }
        else if (listEvent.Type == NetworkListExEvent<Player>.EventType.Remove)
        {
            var removeElement = playerElements.Find(x => x.player == listEvent.Value);

            if (!removeElement)
                return;

            removeElement.Destroy();
            playerElements.Remove(removeElement);
        }
        else if (listEvent.Type == NetworkListExEvent<Player>.EventType.Value)
        {
            Debug.Log("value change: " + listEvent.Value);

            if(TanksNetworkManager.Singleton.IsHost)
            {
                startButton.interactable = GetAreAllPlayersReady();
            }
        }
        else if(listEvent.Type == NetworkListExEvent<Player>.EventType.ElementChanged)
        {
            if (TanksNetworkManager.Singleton.IsHost)
            {
                startButton.interactable = GetAreAllPlayersReady();
            }

            if (listEvent.Value.Id == TanksNetworkManager.Singleton.LocalClientId)
            {
                if (listEvent.Value.IsReady)
                    readyButton.GetComponentInChildren<TextMeshProUGUI>().SetText("UNREADY");
                else
                    readyButton.GetComponentInChildren<TextMeshProUGUI>().SetText("READY");
            }
        }
    }

    public void StartGame()
    {
        if (!GetAreAllPlayersReady())
            return;

        StartCoroutine(DelayUnready());

        GameRoom.Instance.StartGame();
    }

    public void Ready()
    {
        GameRoom.Instance.PlayerReady_ServerRpc();
    }

    IEnumerator DelayUnready()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < GameRoom.Instance.Players.Count; ++i)
        {
            GameRoom.Instance.Players[i].IsReady = false;
        }
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        Instance = null;
    }

    private void LateUpdate()
    {
        if (Instance != this)
            return;
    }

    public bool GetAreAllPlayersReady()
    {
        for (int i = 0; i < GameRoom.Instance.Players.Count; ++i)
        {
            if (!GameRoom.Instance.Players[i].IsReady)
            {
                return false;
            }
        }

        return true;
    }
}
