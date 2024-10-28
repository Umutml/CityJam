using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    private const float RayUpperDiff = 5f;
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private Transform areaObject; // Reference to the 3D object
    [SerializeField] private float raySpacing = 2f; // Ray spacing
    [SerializeField] private float margin = 1f; // Margin from the edges

    private Vector3 _areaEnd;
    private Vector3 _areaStart;

    private void Start()
    {
        // Calculate areaStart and areaEnd based on the given platform object
        var bounds = areaObject.GetComponent<MeshRenderer>().bounds;
        _areaStart = bounds.min;
        _areaEnd = bounds.max;

        PlaceBuildings();
    }

    private void PlaceBuildings()
    {
        for (var x = _areaStart.x + margin; x <= _areaEnd.x - margin; x += raySpacing)
        {
            for (var z = _areaStart.z + margin; z <= _areaEnd.z - margin; z += raySpacing)
            {
                var rayOrigin = new Vector3(x, _areaStart.y + RayUpperDiff, z);
                var ray = new Ray(rayOrigin, Vector3.down); // Cast ray downwards

                // Draw the ray for debugging
                Debug.DrawRay(rayOrigin, Vector3.down * RayUpperDiff * 2, Color.red, 1f);

                if (Physics.Raycast(ray, out var hit))
                {
                    // Skip if it hits a Road
                    if (hit.transform.CompareTag("Road"))
                    {
                        Debug.Log("Hit Road");
                        continue;
                    }

                    // Place building if it hits Buildable
                    if (hit.collider.CompareTag("Buildable"))
                    {
                        Instantiate(buildingPrefab, hit.point, Quaternion.identity);
                    }
                }
            }
        }
    }
}
