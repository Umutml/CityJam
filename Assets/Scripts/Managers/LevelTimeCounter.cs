using System;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class LevelTimeCounter : MonoBehaviour
    {
        private bool _isCounting;
        private float _levelTime;
        private TextMeshProUGUI _timeText;

        private void Awake()
        {
            _timeText = GetComponent<TextMeshProUGUI>();
            LevelManager.OnLevelLoaded += SetLevelTime;
            LevelManager.OnLevelCompleted += StopCounter;
            LevelManager.OnGameOver += StopCounter;
        }

        private void Update()
        {
            TimeCounter();
        }

        private void OnDestroy()
        {
            LevelManager.OnLevelLoaded -= SetLevelTime;
            LevelManager.OnLevelCompleted -= StopCounter;
            LevelManager.OnGameOver -= StopCounter;
        }

        private void StopCounter()
        {
            _isCounting = false;
        }

        private void SetLevelTime()
        {
            _levelTime = LevelManager.Instance.currentLevelData.levelTime;
            _isCounting = true;
        }

        private void TimeCounter()
        {
            if (!_isCounting) return;
            _levelTime -= Time.deltaTime;
            var timeSpan = TimeSpan.FromSeconds(_levelTime);
            _timeText.text = $"Time {timeSpan:mm\\:ss}";
            if (_levelTime <= 0) // If the level timer reaches 0, the game is over
            {
                LevelManager.Instance.GameOver();
            }
        }
    }
}
