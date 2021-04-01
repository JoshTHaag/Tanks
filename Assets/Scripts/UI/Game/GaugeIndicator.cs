using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GaugeIndicator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent onPointerDown = new UnityEvent();
    public UnityEvent onPointerUp = new UnityEvent();

    public RectTransform rectTransform { get; private set; }

    void Awake()
    {
        rectTransform = transform as RectTransform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        onPointerDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        onPointerUp.Invoke();
    }
}
