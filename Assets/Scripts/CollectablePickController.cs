using System.Threading.Tasks;
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
    private GamebarSlot[] gamebarSlots;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        _mainCamera = Camera.main;
        _gamebarUIController = FindObjectOfType<GamebarUIController>();
        gamebarSlots = _gamebarUIController.GetGamebarElements();
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

    private async void ProcessSelectedObject(Collectable collectableItem)
    {
        var emptySlot = _gamebarUIController.GetFirstEmptySlot();

        if (emptySlot == null)
        {
            collectableItem.Highlight(true, Color.red);
            await Task.Delay(50);
            collectableItem.Highlight(false);
            return;
        }
        
        collectableItem.Highlight(true);
        collectableItem.TurnObject();
        collectableItem.ScaleObject();
        collectableItem.DisableCollider();
        
        _gamebarUIController.AddCollectableToSlot(collectableItem);
    }

    #endregion
}
