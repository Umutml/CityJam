using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    private float _deltaTime;
    private TextMeshProUGUI fpsText;

    private void Awake()
    {
#if UNITY_ANDROID || UNITY_IOS 
        Application.targetFrameRate = 120;
#endif
        Application.targetFrameRate = -1; // Set the target frame rate to the maximum
        fpsText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        var fps = 1.0f / _deltaTime;
        fpsText.text = $"FPS: {Mathf.Ceil(fps)}";
    }
}
