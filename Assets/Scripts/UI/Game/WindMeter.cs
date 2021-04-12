using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindMeter : MonoBehaviour
{
    public Image windImage;
    public Image leftArrow;
    public Image rightArrow;
    public TextMeshProUGUI windText;

    public float referenceWindMax;

    public float maxSymbolSizePadding;
    float maxSymbolSize;
    float minSymbolSize;
    float symbolSizeDiff;

    public float maxArrowSizePadding;
    float maxArrowSize;
    float minArrowSize;
    float arrowSizeDiff;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = transform as RectTransform;

        maxSymbolSize = rectTransform.sizeDelta.x - maxSymbolSizePadding;
        minSymbolSize = windImage.rectTransform.sizeDelta.x;
        symbolSizeDiff = maxSymbolSize - minSymbolSize;

        maxArrowSize = (rectTransform.sizeDelta.x * 0.5f) - (windText.rectTransform.sizeDelta.x * 0.5f) - maxArrowSizePadding;
        minArrowSize = leftArrow.rectTransform.sizeDelta.x;
        arrowSizeDiff = maxArrowSize - minArrowSize;
    }

    private void Start()
    {
        GameManager.Instance.Wind.OnValueChanged += OnWindChanged;
        //GameManager.Instance.OnWindChanged.AddListener(OnWindChanged);

        OnWindChanged(0f, GameManager.Instance.Wind.Value);
    }

    public void OnWindChanged(float previousWind, float newWind)
    {
        // Set text.
        string text = Mathf.Abs(newWind).ToString("#");
        if (text == "")
            text = "0";
        windText.SetText(text);

        float percent = Mathf.Abs(newWind) / referenceWindMax;

        // Set wind symbol size.
        float size = Mathf.Clamp(maxSymbolSize * percent, minSymbolSize, maxSymbolSize);
        windImage.rectTransform.sizeDelta = new Vector2(size, windImage.rectTransform.sizeDelta.y);

        // Set arrow size and direction.
        size = Mathf.Clamp(maxArrowSize * percent, minArrowSize, maxArrowSize);
        if (newWind <= -1f)
        {
            leftArrow.enabled = true;
            rightArrow.enabled = false;
            leftArrow.rectTransform.sizeDelta = new Vector2(size, leftArrow.rectTransform.sizeDelta.y);
        }
        else if(newWind >= 1f)
        {
            rightArrow.enabled = true;
            leftArrow.enabled = false;
            rightArrow.rectTransform.sizeDelta = new Vector2(size, rightArrow.rectTransform.sizeDelta.y);
        }
        else
        {
            rightArrow.enabled = false;
            leftArrow.enabled = false;
        }
    }
}
