using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankController : MonoBehaviour
{
    [SerializeField, HideInInspector] Tank tank;

    Vector2 moveInput;
    Vector2 aimInput;
    bool fireHoldInput;
    bool fireReleaseInput;

    private void Update()
    {
        SampleInput();

        tank.Drive(moveInput.x);

        //tank.Aim(aimInput);

        //if (fireHoldInput)
        //    tank.ChargeFire();

        //if(fireReleaseInput)
        //    tank.Fire();
    }

    void SampleInput()
    {
        moveInput = new Vector2();

        moveInput.x += Input.GetAxisRaw("Horizontal");
        moveInput.y += Input.GetAxisRaw("Vertical");

        aimInput = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        fireHoldInput = Input.GetMouseButtonDown(0) && Application.isFocused;
        fireReleaseInput = Input.GetMouseButtonUp(0) && Application.isFocused;
    }

    private void OnValidate()
    {
        tank = GetComponentInParent<Tank>();
    }
}
