using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField] private LevelData[] levelDatas;
        private Dictionary<CollectableType, int> requiredBuildings;
        private Dictionary<CollectableType, int> currentBuildings;
        public LevelData currentLevelData;    

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            LoadLevel(levelDatas[0].levelNumber); // Load the first level
        }

        public void LoadLevel(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= levelDatas.Length)
            {
                Debug.LogError("Invalid level index");
                return;
            }

            UpdateCurrentLevelData(levelIndex);
            requiredBuildings = new Dictionary<CollectableType, int>();
            currentBuildings = new Dictionary<CollectableType, int>();

            foreach (var buildingType in currentLevelData.buildingRequirements)
            {
                requiredBuildings[buildingType.buildingType] = buildingType.requiredCount;
                Debug.Log($"Level {currentLevelData.levelNumber} requires {buildingType.requiredCount} {buildingType.buildingType}");
                currentBuildings[buildingType.buildingType] = 0;
            }

            // Add logic to load the map and create objects for the new level
            Debug.Log($"Loaded Level {currentLevelData.levelNumber}");
        }

        private void UpdateCurrentLevelData(int levelIndex)
        {
            currentLevelData = levelDatas[levelIndex];
        }

        public void AddBuilding(CollectableType buildingType)
        {
            Debug.Log($"Added {buildingType}");
            if (currentBuildings.ContainsKey(buildingType))
            {
                currentBuildings[buildingType]++;
                CheckLevelCompletion();
            }
        }

        private void CheckLevelCompletion()
        {
            foreach (var building in requiredBuildings)
            {
                if (currentBuildings[building.Key] < building.Value)
                {
                    return;
                }
            }
            LevelCompleted();
        }

        private void LevelCompleted()
        {
            Debug.Log("Level Completed!");
            // Add additional logic for level completion here

            // Load the next level if available
            if (currentLevelData.levelNumber + 1 < levelDatas.Length)
            {
                LoadLevel(currentLevelData.levelNumber + 1);
            }
            else
            {
                Debug.Log("All levels completed!");
            }
        }
    }
}