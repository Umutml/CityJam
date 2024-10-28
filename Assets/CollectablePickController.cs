using UnityEngine;

/// <summary>
///     Handles the picking of collectable items in the game by clicking on them
/// </summary>
public class CollectablePicker : MonoBehaviour
{
    #region Fields

    private int _collectablesLayerMask;

    private Collectable _selectedItem;

    private Camera _mainCamera;
    private readonly float cooldownTime = 0.15f;
    private float lastClickTime;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        _mainCamera = Camera.main;
        _collectablesLayerMask = LayerMask.GetMask("Collectables");
        // _collectableTilesController = FindObjectOfType<CollectableTilesController>();
        // _levelLogicController = FindObjectOfType<LevelLogicController>();
    }

    private void Update()
    {
        // if (_levelLogicController.GameStopped)
        //     return;

        if (Input.GetMouseButtonDown(0) && Time.time > lastClickTime + cooldownTime)
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _collectablesLayerMask))
            {
                var collectableItem = hit.transform.GetComponent<Collectable>();
                _selectedItem = collectableItem;
                collectableItem.Highlight(true);
                lastClickTime = Time.time;
            }
        }
        else if (Input.GetMouseButtonUp(0) && _selectedItem != null)
        {
            // var unawaitedTask = ReserveAndGoToTile();
        }
    }

    #endregion
}
