using UnityEngine;

/// <summary>
///     Handles the picking of collectable items in the game by clicking on them
/// </summary>
public class CollectablePicker : MonoBehaviour
{
    #region Fields

    [SerializeField] private LayerMask collectablesLayerMask;
    private GamebarUIController _gamebarUIController;

    private Collectable _selectedItem;

    private Camera _mainCamera;
    private readonly float cooldownTime = 0.15f;
    private float lastClickTime;
    private RectTransform[] gamebarElements;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        _mainCamera = Camera.main;
        _gamebarUIController = FindObjectOfType<GamebarUIController>();
        gamebarElements = _gamebarUIController.GetGamebarElements();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > lastClickTime + cooldownTime)
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, collectablesLayerMask))
            {
                var collectableItem = hit.transform.GetComponent<Collectable>();
                _selectedItem = collectableItem;
                lastClickTime = Time.time;
                ProcessSelectedObject(collectableItem);
            }
        }
        else if (Input.GetMouseButtonUp(0) && _selectedItem != null)
        {
        }
    }

    private void ProcessSelectedObject(Collectable collectableItem)
    {
        collectableItem.Highlight(true);
        collectableItem.TurnObject();
        collectableItem.ScaleObject();
        collectableItem.DisableCollider();


        // Convert the UI element's position to a screen position
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(_mainCamera, gamebarElements[Random.Range(0, gamebarElements.Length)].transform.position);
        var centeredY = screenPosition.y - 25;
        // Convert the screen position to a world position
        var worldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, centeredY, _mainCamera.nearClipPlane + 1f));

        // Move the selected item to the calculated world position
        collectableItem.MoveToUIPosition(worldPosition);
    }

    #endregion
}
