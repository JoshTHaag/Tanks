using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankController : MonoBehaviour
{
    [SerializeField, HideInInspector] Tank tank;

    Vector2 moveInput;

    private void Update()
    {
        SampleInput();

        tank.Drive(moveInput.x);
    }

    void SampleInput()
    {
        moveInput = new Vector2();

        moveInput.x += Input.GetAxisRaw("Horizontal");
        moveInput.y += Input.GetAxisRaw("Vertical");
    }

    private void OnValidate()
    {
        tank = GetComponentInParent<Tank>();
    }
}
