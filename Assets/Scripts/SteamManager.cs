using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamManager : MonoBehaviour
{
    public static SteamManager Instance { get; private set; }

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Init()
    {
        try
        {
            SteamClient.Init(1305290, true);
        }
        catch (System.Exception e)
        {
            // Something went wrong - it's one of these:
            //
            //     Steam is closed?
            //     Can't find steam_api dll?
            //     Don't have permission to play app?
            //

            Debug.LogError(e);
            return;
        }
    }

    private void OnDestroy()
    {
        if (Instance)
            return;

        SteamClient.Shutdown();
    }
}
