using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gamecore.LevelSystem
{
    public class LevelCreator : MonoBehaviour
    {
        private const float RayUpperDiff = 25f;
        private const float Margin = 1f; // Margin from the edges

        private const string BuildableTag = "Buildable";
        private const string RoadTag = "Road";
        private const string BuildingTag = "Building";
        [SerializeField] private float RaySpacing = 0.1f; // Ray spacing

        [SerializeField] private List<GameObject> levelBuildPrefabs;
        private Vector3 _areaEnd;
        private Transform _areaObject; // Reference to the 3D plane object on map
        private Vector3 _areaStart;
        private Quaternion _buildingRotation;
        private Vector3 _buildingSize;
        private LevelData _currentLevelData;

        private GameObject _levelMap;
        private GameObject _nextBuilding;
        public static LevelCreator Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async void CreateLevel()
        {
            var levelManager = LevelManager.Instance;
            _currentLevelData = LevelManager.Instance.currentLevelData;

            levelBuildPrefabs.Clear();
            foreach (var requirement in levelManager.currentLevelData.buildingRequirements)
            {
                levelBuildPrefabs.Add(requirement.BuildingPrefab);
            }

            // Calculate areaStart and areaEnd based on the given platform object
            await RemoveOldLevelAssets();
            await Task.Delay(100);
            await PlaceLevelMap();
            await Task.Delay(100);
            await PlaceBuildings();
        }

        private Task RemoveOldLevelAssets()
        {
            if (_levelMap != null)
            {
                Destroy(_levelMap);
            }

            Debug.Log("Old level assets removed");
            return Task.CompletedTask;
        }


        private Task PlaceLevelMap()
        {
            _levelMap = Instantiate(_currentLevelData.levelMap, Vector3.zero, Quaternion.identity);
            _areaObject = _levelMap.transform.Find("Area");
            var bounds = _areaObject.GetComponent<MeshRenderer>().bounds;
            _areaStart = bounds.min;
            _areaEnd = bounds.max;
            Debug.Log("Level map created: " + (_currentLevelData.levelNumber + 1));
            return Task.CompletedTask;
        }

        private GameObject GetRandomBuildingPrefab()
        {
            var buildingPrefab = levelBuildPrefabs[Random.Range(0, levelBuildPrefabs.Count)];
            return buildingPrefab;
        }

        private Task PlaceBuildings()
        {
            var levelManager = LevelManager.Instance;
            var buildingRequirements = levelManager.currentLevelData.buildingRequirements;


            // Place required buildings first
            foreach (var requirement in buildingRequirements)
            {
                var buildingPrefab = requirement.BuildingPrefab;
                var buildingCount = requirement.requiredCount + 2; // Place two extra building to ensure all required buildings are placed

                for (var i = 0; i < buildingCount; i++)
                {
                    var placeBuild = PlaceRequiredBuilding(buildingPrefab);
                    if (!placeBuild)
                    {
                        Debug.LogWarning("Not enough space to place all required buildings. Place the remaining buildings randomly.");
                        return Task.CompletedTask;
                    }
                }
            }

            Debug.Log("All required buildings placed");

            // Place random buildings in the remaining space
            for (var x = _areaStart.x + Margin; x <= _areaEnd.x - Margin; x += RaySpacing)
            {
                for (var z = _areaStart.z + Margin; z <= _areaEnd.z - Margin; z += RaySpacing)
                {
                    var rayOrigin = new Vector3(x, _areaStart.y + RayUpperDiff, z);
                    var ray = new Ray(rayOrigin, Vector3.down); // Cast ray downwards

                    if (Physics.Raycast(ray, out var hit))
                    {
                        // Skip if it hits a Road or Building
                        if (hit.transform.CompareTag(RoadTag) || hit.transform.CompareTag(BuildingTag))
                            continue;

                        _nextBuilding = GetRandomBuildingPrefab();
                        _buildingSize = GetBuildingSize(_nextBuilding);
                        _buildingRotation = GetRandomRotation();

                        // Check if the building area is fully within the Buildable area
                        if (IsAreaBuildable(hit.point))
                        {
                            CreateSingleBuild(_nextBuilding, hit.point, _buildingRotation, _levelMap.transform);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        private void CreateSingleBuild(GameObject buildingPrefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var tempBuild = Instantiate(buildingPrefab, position, rotation, parent.transform);
            var firstYPosition = tempBuild.transform.position.y;
            // Scale the building from zero to its original scale with a nice animation
            tempBuild.transform.DOMoveY(firstYPosition, 0.75f).SetEase(Ease.OutQuad).From(Vector3.down * 2);
        }


        private bool PlaceRequiredBuilding(GameObject buildingPrefab)
        {
            for (var x = _areaStart.x + Margin; x <= _areaEnd.x - Margin; x += RaySpacing)
            {
                for (var z = _areaStart.z + Margin; z <= _areaEnd.z - Margin; z += RaySpacing)
                {
                    var rayOrigin = new Vector3(x, _areaStart.y + RayUpperDiff, z);
                    var ray = new Ray(rayOrigin, Vector3.down); // Cast ray downwards

                    if (Physics.Raycast(ray, out var hit))
                    {
                        // Skip if it hits a Road or Building
                        if (hit.transform.CompareTag(RoadTag) || hit.transform.CompareTag(BuildingTag))
                            continue;

                        _buildingSize = GetBuildingSize(buildingPrefab);
                        _buildingRotation = GetRandomRotation();

                        // Check if the building area is fully within the Buildable area
                        if (IsAreaBuildable(hit.point))
                        {
                            CreateSingleBuild(buildingPrefab, hit.point, _buildingRotation, _levelMap.transform);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private Quaternion GetRandomRotation()
        {
            var randomChoice = Random.Range(0, 4);
            switch (randomChoice)
            {
                case 0:
                    return Quaternion.Euler(0, 0, 0); // No rotation
                case 1:
                    return Quaternion.Euler(0, 90, 0); // Rotated by 90 degrees
                case 2:
                    return Quaternion.Euler(0, 180, 0); // Flipped backwards
                case 3:
                    return Quaternion.Euler(0, 270, 0); // Rotated by 270 degrees
                default:
                    return Quaternion.identity; // Default to no rotation
            }
        }


        private Vector3 GetBuildingSize(GameObject prefab)
        {
            // Create a temporary instance of the prefab far away from the visible area
            var tempInstance = Instantiate(prefab, Vector3.one * 10000, Quaternion.identity); // TODO: Use object pooling instead of Instantiate and Destroy for performance
            var coll = tempInstance.GetComponent<Collider>();
            if (coll == null || coll.bounds.size == Vector3.zero)
            {
                Debug.LogError("No collider found on the building prefab or the size is zero.");
                Destroy(tempInstance);
                return Vector3.zero;
            }

            var size = coll.bounds.size;
            Destroy(tempInstance);
            return size;
        }

        private bool IsAreaBuildable(Vector3 point)
        {
            var corners = GetBuildingCorners(point);

            // Check if all corners are within the Buildable area
            foreach (var corner in corners)
            {
                var rayOrigin = corner + Vector3.up * RayUpperDiff;
                var ray = new Ray(rayOrigin, Vector3.down);

                if (Physics.Raycast(ray, out var hit))
                {
                    if (!hit.collider.CompareTag(BuildableTag))
                    {
                        return false; // If the ray hits something other than Buildable
                    }
                }
                else
                {
                    return false; // If the ray does not hit anything
                }
            }

            // Check for overlapping buildings
            var halfExtents = new Vector3(_buildingSize.x / 2, _buildingSize.y / 2, _buildingSize.z / 2);
            var center = point + new Vector3(0, _buildingSize.y / 2, 0); // Adjust center to account for building height
            var colliders = Physics.OverlapBox(center, halfExtents, _buildingRotation);
            foreach (var coll in colliders)
            {
                if (coll.CompareTag(BuildingTag))
                {
                    return false; // If there is an overlapping building
                }
            }

            return true; // All checks passed
        }

        // Get the corners of the building area
        private Vector3[] GetBuildingCorners(Vector3 point)
        {
            Vector3[] corners =
            {
                point + new Vector3(_buildingSize.x / 2, 0, _buildingSize.z / 2),
                point + new Vector3(-_buildingSize.x / 2, 0, _buildingSize.z / 2),
                point + new Vector3(_buildingSize.x / 2, 0, -_buildingSize.z / 2),
                point + new Vector3(-_buildingSize.x / 2, 0, -_buildingSize.z / 2)
            };
            return corners;
        }
    }
}
