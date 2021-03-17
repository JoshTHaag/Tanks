using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerCharacter controlledCharacter;

    private void Update()
    {
        controlledCharacter.moveInput = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
            controlledCharacter.moveInput.x = -1f;
        if (Input.GetKey(KeyCode.D))
            controlledCharacter.moveInput.x = 1f;
        if (Input.GetKey(KeyCode.W))
            controlledCharacter.moveInput.y = 1f;
        if (Input.GetKey(KeyCode.S))
            controlledCharacter.moveInput.y = -1f;
    }
}
