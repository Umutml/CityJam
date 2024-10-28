using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private float rayUpperDiff = 25f;
    [SerializeField] private GameObject[] buildingPrefabs;
    [SerializeField] private Transform areaObject; // Reference to the 3D object
    [SerializeField] private float raySpacing = 2f; // Ray spacing
    [SerializeField] private float margin = 1f; // Margin from the edges

    private Vector3 _areaEnd;
    private Vector3 _areaStart;
    private Vector3 _buildingSize;
    private GameObject _nextBuilding;

    private void Start()
    {
        // Calculate areaStart and areaEnd based on the given platform object
        var bounds = areaObject.GetComponent<MeshRenderer>().bounds;
        _areaStart = bounds.min;
        _areaEnd = bounds.max;

        PlaceBuildings();
    }

    private GameObject GetRandomBuildingPrefab()
    {
        var buildingPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
        return buildingPrefab;
    }

    private void PlaceBuildings()
    {
        for (var x = _areaStart.x + margin; x <= _areaEnd.x - margin; x += raySpacing)
        {
            for (var z = _areaStart.z + margin; z <= _areaEnd.z - margin; z += raySpacing)
            {
                var rayOrigin = new Vector3(x, _areaStart.y + rayUpperDiff, z);
                var ray = new Ray(rayOrigin, Vector3.down); // Cast ray downwards

                if (Physics.Raycast(ray, out var hit))
                {
                    // Skip if it hits a Road
                    if (hit.transform.CompareTag("Road") || hit.transform.CompareTag("Building"))
                    {
                        Debug.Log("Hit Road");
                        continue;
                    }

                    _nextBuilding = GetRandomBuildingPrefab();
                    _buildingSize = GetBuildingSize(_nextBuilding);

                    // Check if the building area is fully within the Buildable area
                    if (IsAreaBuildable(hit.point))
                    {
                        Instantiate(_nextBuilding, hit.point, GetRandomRotation());
                    }
                }
            }
        }
    }

    private Quaternion GetRandomRotation()
    {
        var randomChoice = Random.Range(0, 2);
        if (randomChoice == 0)
        {
            return Quaternion.Euler(0, 180, 0); // Flipped backwards
        }

        return Quaternion.Euler(0, 90, 0); // Rotated by 90 degrees
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
        Debug.Log("Building size: " + size);
        return size;
    }

    private bool IsAreaBuildable(Vector3 point)
    {
        var corners = GetBuildingCorners(point);

        // Check if all corners are within the Buildable area
        foreach (var corner in corners)
        {
            var rayOrigin = corner + Vector3.up * rayUpperDiff;
            var ray = new Ray(rayOrigin, Vector3.down);

            if (Physics.Raycast(ray, out var hit))
            {
                if (!hit.collider.CompareTag("Buildable"))
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
        var colliders = Physics.OverlapBox(center, halfExtents, Quaternion.identity);
        foreach (var coll in colliders)
        {
            if (coll.CompareTag("Building"))
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
