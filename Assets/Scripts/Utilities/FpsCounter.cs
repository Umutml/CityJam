using TMPro;
using UnityEngine;

namespace Utilities
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private bool showFps;
        private float _deltaTime;
        private TextMeshProUGUI _fpsText;

        private void Start()
        {
#if !UNITY_EDITOR
            Application.targetFrameRate = 120;
            QualitySettings.vSyncCount = 0;
#endif
            if (!showFps)
            {
                gameObject.SetActive(false);
                Destroy(this, 0.2f);
                return;
            }

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
