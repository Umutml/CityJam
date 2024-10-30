using TMPro;
using UnityEngine;

namespace Utilities
{
    public class FpsCounter : MonoBehaviour
    {
        private float _deltaTime;
        private TextMeshProUGUI _fpsText;
        [SerializeField] private bool showFps;

        private void Awake()
        {
        
#if UNITY_ANDROID || UNITY_IOS 
            Application.targetFrameRate = 120;
#endif
            Application.targetFrameRate = -1; // Set the target frame rate to the maximum
        
            if (!showFps) return;
            _fpsText = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (!showFps) return;
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
            var fps = 1.0f / _deltaTime;
            _fpsText.text = $"FPS: {Mathf.Ceil(fps)}";
        }
    }
}
