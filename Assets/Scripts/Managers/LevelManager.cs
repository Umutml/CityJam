using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gamecore;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelData[] levelDatas;

        [HideInInspector] public LevelData currentLevelData;
        private Dictionary<CollectableTypes, int> currentBuildings;
        private Dictionary<CollectableTypes, int> requiredBuildings;
        public static LevelManager Instance { get; private set; }
        public static Action OnLevelLoaded;
        public static Action OnLevelCompleted;
        public static Action OnGameOver;
        public static bool IsLevelPlaying { get; private set; }

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

            GamebarController.OnCollectableDestroyed += CheckLevelCompletion;
        }

        private void Start()
        {
            LoadLevel(levelDatas[0].levelNumber); // Load the first level
        }

        private void LoadLevel(int levelIndex)
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
                currentBuildings[buildingType.buildingTypes] = 0;
            }

            LevelCreator.Instance.CreateLevel(); // Add logic to load the map and create objects for the new level
            OnLevelLoaded?.Invoke(); // Notify subscribers that the level has been loaded
            IsLevelPlaying = true;
            // Add logic to load the map and create objects for the new level
            Debug.Log($"Loaded Level {currentLevelData.levelNumber + 1}"); // Level numbers are 0 based
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

            IncrementLevel();
            LevelCompleted();
        }

        private void IncrementLevel()
        {
            if (currentLevelData.levelNumber + 1 >= levelDatas.Length)
            {
                Debug.LogError("No more levels available could not increment level restarted to 0");
                currentLevelData = levelDatas[0];
                return;
            }

            currentLevelData = levelDatas[currentLevelData.levelNumber + 1];
        }

        private void LevelCompleted()
        {
            IsLevelPlaying = false;
            Debug.Log("Level Completed!");
            OnLevelCompleted?.Invoke();
        }

        public void PlayNextLevel()
        {
            if (currentLevelData.levelNumber < levelDatas.Length)
            {
                LoadLevel(currentLevelData.levelNumber);
            }
            else
            {
                Debug.Log("All levels completed!");
            }
        }
        
        public void GameOver()
        {
            OnGameOver?.Invoke();
            IsLevelPlaying = false;
        }
        
        public void RestartLevel()
        {
            LoadLevel(currentLevelData.levelNumber);
        }

        private void OnDestroy()
        {
            GamebarController.OnCollectableDestroyed -= CheckLevelCompletion;
        }
    }
}
