using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Gamecore;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject levelCompletedPanel;
    [SerializeField] private GameObject gameOverPanel;
    
    [SerializeField] private GameObject goalSlotPrefab;
    [SerializeField] private Transform goalSlotParent;
    
    private List<GameObject> _goalSlots = new List<GameObject>();
    
    private void Awake()
    {
        LevelManager.OnLevelCompleted += LevelCompletedHandler;
        LevelManager.OnLevelLoaded += LevelLoadedHandler;
        LevelManager.OnGameOver += GameOverHandler;
        GamebarController.OnCollectableDestroyed += UpdateGoalSlots;
    }
    
    private void GameOverHandler()
    {
        ShowGameOverPanel();
    }
    
    private void LevelCompletedHandler()
    {
        ShowLevelCompletedPanel();
    }
    
    private void LevelLoadedHandler()
    {
        HideLevelCompletedPanel();
        HideGameOverPanel();
        ClearGoalSlots();
        CreateGoalSlots();
    }

    private void CreateGoalSlots()
    {
        var requiredBuildings = LevelManager.Instance.requiredBuildings;
        foreach (var buildingType in requiredBuildings)
        {
            var goalSlot = Instantiate(goalSlotPrefab, goalSlotParent);
            _goalSlots.Add(goalSlot);
            var goalSlotImage = goalSlot.transform.Find("Image").GetComponent<Image>();
            var goalSlotCount = goalSlot.transform.Find("Count").GetComponent<TextMeshProUGUI>();
            goalSlotImage.sprite = GetBuildingSprite(buildingType.Key);
            goalSlotCount.text = buildingType.Value.ToString();
        }
    }
    
    private void UpdateGoalSlots()
    {
        var currentBuildings = LevelManager.Instance.currentBuildings;
        for (var i = 0; i < _goalSlots.Count; i++)
        {
            var goalSlot = _goalSlots[i];
            var buildingType = LevelManager.Instance.requiredBuildings.Keys.ElementAt(i);
            var currentBuildingCount = currentBuildings[buildingType];
            var requiredBuildingCount = LevelManager.Instance.requiredBuildings[buildingType];
            var goalSlotCount = goalSlot.transform.Find("Count").GetComponent<TextMeshProUGUI>();
            var remainingCount = requiredBuildingCount - currentBuildingCount; // Calculate the remaining count
            goalSlotCount.text = remainingCount > 0 ? remainingCount.ToString() : "0";
        }
    }
    
    private void ClearGoalSlots()
    {
        foreach (var goalSlot in _goalSlots)
        {
            Destroy(goalSlot);
        }
        _goalSlots.Clear();
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

    
    
    public Sprite GetBuildingSprite(CollectableTypes buildingType)
    {
        foreach (var building in LevelManager.Instance.currentLevelData.buildingRequirements)
        {
            if (building.buildingTypes == buildingType)
            {
                return building.BuldingSprite;
            }
        }
        return null;
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
        LevelManager.OnLevelCompleted -= LevelCompletedHandler;
        LevelManager.OnLevelLoaded -= LevelLoadedHandler;
        LevelManager.OnGameOver -= GameOverHandler;
        GamebarController.OnCollectableDestroyed -= UpdateGoalSlots;
    }
}
