using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public float moveSpeed;

    [HideInInspector]
    public Vector2 moveInput;

    NetMovement netMovement;

    private void Awake()
    {
        netMovement = GetComponent<NetMovement>();
    }

    private void Update()
    {
        moveInput.Normalize();
        netMovement.velocity = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
    }
}
