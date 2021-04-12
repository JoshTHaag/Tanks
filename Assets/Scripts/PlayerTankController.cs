using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankController : MonoBehaviour
{
    Tank tank;

    Vector2 moveInput;
    Vector2 aimInput;
    bool fireHoldInput;
    bool fireReleaseInput;

    public void SetTank(Tank tank)
    {
        this.tank = tank;
    }

    private void Update()
    {
        if (!tank)
            return;

        SampleInput();

        tank.Drive(moveInput.x);
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
}
