using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject levelCompletedPanel;
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        LevelManager.OnLevelCompleted += ShowLevelCompletedPanel;
        LevelManager.OnLevelLoaded += HideLevelCompletedPanel;
        LevelManager.OnLevelLoaded += HideGameOverPanel;
        LevelManager.OnGameOver += ShowGameOverPanel;
    }

    private void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true); // Show the game over screen
        var gameOverCanvGroup = gameOverPanel.GetComponent<CanvasGroup>();
        gameOverCanvGroup.DOFade(1f, 0.75f).From(0f).SetEase(Ease.Linear);
    }
    
    private void HideGameOverPanel()
    {
        gameOverPanel.SetActive(false); // Hide the game over screen
    }

    private void ShowLevelCompletedPanel()
    {
        // Show the level completed screen
        levelCompletedPanel.SetActive(true);
        var levelCompletedCanvGroup = levelCompletedPanel.GetComponent<CanvasGroup>();
        levelCompletedCanvGroup.DOFade(1f, 0.5f).From(0f).SetEase(Ease.Linear);
    }

    private void HideLevelCompletedPanel()
    {
        // Hide the level completed screen
        levelCompletedPanel.SetActive(false);
    }

    private void UpdateGoalProgress()
    {
        // Update the UI to show the progress towards the level goal
    }

    public void OnNextButtonClicked()
    {
        LevelManager.Instance.PlayNextLevel();
    }
    
    public void OnRestartButtonClicked()
    {
        LevelManager.Instance.RestartLevel();
    }

    private void OnDestroy()
    {
        LevelManager.OnGameOver -= ShowGameOverPanel;
        LevelManager.OnLevelCompleted -= ShowLevelCompletedPanel;
        LevelManager.OnLevelLoaded -= HideLevelCompletedPanel;
    }
}
