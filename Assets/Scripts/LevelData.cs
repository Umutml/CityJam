using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public BuildingRequirement[] buildingRequirements;
    public int levelNumber;
    public float levelTime;
}

[System.Serializable]
public class BuildingRequirement
{
    public CollectableType buildingType;
    public int requiredCount;
    public Image BuildingImage;
}
