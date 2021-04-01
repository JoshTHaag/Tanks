using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using MLAPI;
using System.IO;

[RequireComponent(typeof(NetworkedObject))]
public class PlayerElement : NetworkedBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public new NetworkedObject networkedObject;

    public TMP_InputField nameInputField;

    public Player player;
    
    void Awake()
    {
        nameInputField.onEndEdit.AddListener(OnEndEditName);
    }

    public override void NetworkStart(Stream stream)
    {
        Debug.Log("NetworkStart");

        //player = GameRoom.Instance.Players.Find(x => x.id == OwnerClientId);
        //foreach (var p in GameRoom.Instance.Players)
        //    if (p.id == OwnerClientId)
        //        player = p;

        //Debug.Log(player);

        //Init(player);
    }

    void Start()
    {
        Debug.Log("Start");

        foreach (var p in GameRoom.Instance.Players)
            if (p.id == OwnerClientId)
                player = p;

        Debug.Log(player);

        Init(player);
    }

    public void Spawn(ulong ownerId)
    {
        networkedObject.Spawn();
        networkedObject.ChangeOwnership(ownerId);
    }

    public void Init(Player player)
    {
        this.player = player;
        transform.SetParent(LobbyUI.Instance.playerListContent);
        nameInputField.textComponent.SetText(player.name);
    }

    void LateUpdate()
    {
        if (!nameInputField.isFocused)
            nameInputField.text = player.name;

        if (Input.GetKeyDown(KeyCode.Space))
            Debug.Log(OwnerClientId);
    }

    void OnEndEditName(string newName)
    {
        if(newName != player.name)
        {
            GameRoom.Instance.CmdRequestNameChange(player.id, newName);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
