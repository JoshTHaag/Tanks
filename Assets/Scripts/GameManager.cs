using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public float maxWind;
    public float minWind;
    public float minWindChange;
    public float maxWindChange;

    public float Wind { get; private set; }

    public UnityEvent<float> OnWindChanged = new UnityEvent<float>();

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

        StartGame();
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;
    }

    public void StartGame()
    {
        Wind = Random.Range(minWind, maxWind);
    }

    void OnClientConnected(ulong id)
    {
        Debug.Log(TanksNetworkManager.Singleton.ConnectedClients[id]);
    }

    public void ChangeWind()
    {
        float change = Random.Range(minWindChange, maxWindChange);
        Wind = Mathf.Clamp(Wind + change, minWind, maxWind);
        OnWindChanged.Invoke(Wind);
    }
}
