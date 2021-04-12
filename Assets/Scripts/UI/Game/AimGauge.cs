using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AimGauge : Gauge
{
    protected override void Update()
    {
        base.Update();

        if (!tank)
            return;

        tank.Aim(gaugeValPerc * maxVal);
    }

    protected override void SetValFromIndicator()
    {
        base.SetValFromIndicator();

        gaugeValPerc = Mathf.Abs(1f - gaugeValPerc);
    }
}
