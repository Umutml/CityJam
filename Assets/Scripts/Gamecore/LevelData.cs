using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public BuildingRequirement[] buildingRequirements;
    public int levelNumber;
    public float levelTime;
    public GameObject levelMap;
}

[System.Serializable]
public class BuildingRequirement
{
    public CollectableTypes buildingTypes;
    public int requiredCount;
    public Image BuildingImage;
    public GameObject BuildingPrefab;
}