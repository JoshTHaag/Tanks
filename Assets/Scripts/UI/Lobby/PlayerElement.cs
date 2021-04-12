using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public TMP_InputField nameInputField;
    public Image readyBGImage;
    public Image readyIndicatorImage;
    public Sprite readySprite;
    public Sprite notReadySprite;
    public Sprite hostSprite;

    public Color localPlayerNameColor;
    public Color remotePlayerNameColor;

    public Player player;

    bool isLocalPlayer = false;
    
    void Awake()
    {
        nameInputField.onEndEdit.AddListener(OnEndEditName);
    }

    void OnDestroy()
    {
        nameInputField.onEndEdit.RemoveListener(OnEndEditName);
    }

    public void Init(Player player)
    {
        this.player = player;
        isLocalPlayer = player.Id == TanksNetworkManager.Singleton.LocalClientId;
        transform.SetParent(LobbyUI.Instance.playerListContent);
        nameInputField.textComponent.SetText(player.Name);

        if(player.IsHost)
        {
            readyIndicatorImage.sprite = hostSprite;
        }

        readyBGImage.enabled = !player.IsHost;
        nameInputField.interactable = isLocalPlayer;
        if (isLocalPlayer)
            nameInputField.textComponent.color = localPlayerNameColor;
        else
            nameInputField.textComponent.color = remotePlayerNameColor;
    }

    void LateUpdate()
    {
        if (!nameInputField.isFocused)
            nameInputField.text = player.Name;

        if (!player.IsHost)
        {
            if (player.IsReady)
                readyIndicatorImage.sprite = readySprite;
            else
                readyIndicatorImage.sprite = notReadySprite;
        }
    }

    void OnEndEditName(string newName)
    {
        if(newName != player.Name)
        {
            GameRoom.Instance.RequestNameChange_ServerRpc(newName);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
