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
    
    void Awake()
    {
        nameInputField.onEndEdit.AddListener(OnEndEditName);
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
    }

    void OnEndEditName(string newName)
    {
        if(newName != player.name)
        {
            GameRoom.Instance.RequestNameChange_ServerRpc(player.id, newName);
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
