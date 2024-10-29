using System.Collections.Generic;
using Gamecore;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField] private LevelData[] levelDatas;
        private Dictionary<CollectableTypes, int> requiredBuildings;
        private Dictionary<CollectableTypes, int> currentBuildings;
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
            requiredBuildings = new Dictionary<CollectableTypes, int>();
            currentBuildings = new Dictionary<CollectableTypes, int>();

            foreach (var buildingType in currentLevelData.buildingRequirements)
            {
                requiredBuildings[buildingType.buildingTypes] = buildingType.requiredCount;
                Debug.Log($"Level {currentLevelData.levelNumber} requires {buildingType.requiredCount} {buildingType.buildingTypes}");
                currentBuildings[buildingType.buildingTypes] = 0;
            }

            
            LevelCreator.Instance.CreateLevel(); // Add logic to load the map and create objects for the new level
            // Add logic to load the map and create objects for the new level
            Debug.Log($"Loaded Level {currentLevelData.levelNumber}");
        }

        private void UpdateCurrentLevelData(int levelIndex)
        {
            currentLevelData = levelDatas[levelIndex];
        }

        public void AddBuilding(CollectableTypes buildingTypes)
        {
            Debug.Log($"Added {buildingTypes}");
            if (currentBuildings.ContainsKey(buildingTypes))
            {
                currentBuildings[buildingTypes]++;
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