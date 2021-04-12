using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gauge : MonoBehaviour
{
    public GaugeIndicator indicator;
    public TextMeshProUGUI textGauge;

    public RectTransform minPos;
    public RectTransform maxPos;

    public float maxVal;

    protected Tank tank;

    protected bool isIndicatorSelected = false;

    protected Vector2 lastMousePos;

    public float gaugeValPerc;

    public virtual void SetTank(Tank tank)
    {
        this.tank = tank;
    }

    private void Start()
    {
        if(FindObjectOfType<Tank>())
            SetTank(FindObjectOfType<Tank>());

        if(tank)
        {
            SetValFromIndicator();
            UpdateText();
        }
    }

    public void IndicatorSelected()
    {
        isIndicatorSelected = true;

        lastMousePos = Input.mousePosition;
    }

    public void IndicatorUnselected()
    {
        isIndicatorSelected = false;
    }

    protected virtual void Update()
    {
    }

    protected virtual void LateUpdate()
    {
        if(isIndicatorSelected)
        {
            Vector2 pos = (Vector2)Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            pos.y = 0f;

            indicator.rectTransform.anchoredPosition += pos;

            Vector2 anchoredPos = indicator.rectTransform.anchoredPosition;
            anchoredPos.x = Mathf.Clamp(anchoredPos.x, minPos.anchoredPosition.x, maxPos.anchoredPosition.x);
            indicator.rectTransform.anchoredPosition = anchoredPos;

            SetValFromIndicator();
            UpdateText();
        }
    }

    protected virtual void SetValFromIndicator()
    {
        gaugeValPerc = ((indicator.rectTransform.anchoredPosition.x - minPos.anchoredPosition.x) * 1f) / (maxPos.anchoredPosition.x - minPos.anchoredPosition.x);
    }

    protected virtual void UpdateText()
    {
        textGauge.SetText((gaugeValPerc * maxVal).ToString("0.#"));
    }
}
