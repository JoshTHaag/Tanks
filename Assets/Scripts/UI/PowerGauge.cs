using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerGauge : Gauge
{
    public override void SetTank(Tank tank)
    {
        base.SetTank(tank);

        maxVal = tank.maxFireForce;
    }

    protected override void Update()
    {
        base.Update();

        tank.SetPower(gaugeValPerc * maxVal);
    }

    protected override void SetValFromIndicator()
    {
        base.SetValFromIndicator();
    }

    protected override void UpdateText()
    {
        textGauge.SetText(Mathf.Clamp(gaugeValPerc * maxVal, tank.minFireForce, tank.maxFireForce).ToString("0.#"));
    }
}
