using DG.Tweening;
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
        
        // Convert 3D world position to screen position
        Vector3 screenBarPosition = _mainCamera.ScreenToWorldPoint(gamebarElements[Random.Range(0,gamebarElements.Length)].transform.position);

        // Move the selected item to the UI position
        collectableItem.MoveToUIPosition(screenBarPosition);
        // collectableItem.transform.DOMove(screenPosition, 1f).SetEase(Ease.InOutSine);
    }

    #endregion
}
