using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject levelCompletedPanel;
    
    private void Awake()
    {
        LevelManager.OnLevelCompleted += ShowLevelCompletedPanel;
        LevelManager.OnLevelLoaded += HideLevelCompletedPanel;
    }
    
    private void ShowLevelCompletedPanel()
    {
        // Show the level completed screen
        levelCompletedPanel.SetActive(true);
        
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
    
    private void OnDestroy()
    {
        LevelManager.OnLevelCompleted -= ShowLevelCompletedPanel;
        LevelManager.OnLevelLoaded -= HideLevelCompletedPanel;
    }
}
