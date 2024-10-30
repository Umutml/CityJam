using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    private float _deltaTime;
    private TextMeshProUGUI fpsText;

    private void Awake()
    {
        fpsText = GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        float msec = _deltaTime * 1000.0f;
        float fps = 1.0f / _deltaTime;
        fpsText.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
    }
}
