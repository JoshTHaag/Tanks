using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public TMP_InputField nameInputField;

    public Player player;

    bool isLocalPlayer = false;
    
    void Awake()
    {
        nameInputField.onEndEdit.AddListener(OnEndEditName);
    }

    public void Init(Player player)
    {
        this.player = player;
        isLocalPlayer = player.Id == TanksNetworkManager.Singleton.LocalClientId;
        transform.SetParent(LobbyUI.Instance.playerListContent);
        nameInputField.textComponent.SetText(player.Name);

        //if (!isLocalPlayer)
        //    nameInputField.DeactivateInputField(false);
    }

    void LateUpdate()
    {
        if (!nameInputField.isFocused)
            nameInputField.text = player.Name;
    }

    void OnEndEditName(string newName)
    {
        if(newName != player.Name)
        {
            GameRoom.Instance.RequestNameChange_ServerRpc(player.Id, newName);
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

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
