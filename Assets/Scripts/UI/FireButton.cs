using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FireButton : MonoBehaviour
{
    Tank tank;

    public void SetTank(Tank tank)
    {
        this.tank = tank;
    }

    private void Start()
    {
        SetTank(FindObjectOfType<Tank>());
    }

    public void Fire()
    {
        tank.Fire();
    }
}