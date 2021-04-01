using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FPSCounter : MonoBehaviour
{
    TextMeshProUGUI text;

    public float refreshRate;
    float timer, avgFramerate;
    string display = "FPS: {0}";

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void LateUpdate()
    {
        float timelapse = Time.smoothDeltaTime / Time.timeScale;
        timer = timer <= 0 ? refreshRate : timer -= timelapse;

        if (timer <= 0) avgFramerate = (int)(1f / timelapse);
        text.SetText(string.Format(display, avgFramerate.ToString("#")));
    }
}
