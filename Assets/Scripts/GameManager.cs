using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

public class GameManager : NetworkBehaviour
{
    public Tank prefabPlayer;
    public TanksTerrain prefabNetTerrain;

    public float maxWind;
    public float minWind;
    public float minWindChange;
    public float maxWindChange;

    public NetworkVariable<float> Wind { get; private set; }

    public static GameManager Instance { get; private set; }

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

        TanksNetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        if(IsHost)
        {
            StartGame();
        }
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;
    }

    public void StartGame()
    {
        Wind.Value = Random.Range(minWind, maxWind);

        foreach(var p in GameRoom.Instance.Players)
        {
            Tank newTank = Instantiate(prefabPlayer);
            newTank.GetComponent<NetworkObject>().SpawnAsPlayerObject(p.Id, null, true);
        }
    }

    void OnClientConnected(ulong id)
    {
        Debug.Log(TanksNetworkManager.Singleton.ConnectedClients[id]);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeWind_ServerRpc()
    {
        float change = Random.Range(minWindChange, maxWindChange);
        Wind.Value = Mathf.Clamp(Wind.Value + change, minWind, maxWind);
    }
}
