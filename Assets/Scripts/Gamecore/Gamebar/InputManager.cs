using System.Threading.Tasks;
using UnityEngine;

namespace Gamecore.Gamebar
{
    /// <summary>
    ///     Handles the picking of collectable items in the game by clicking on them
    /// </summary>
    public class InputController : MonoBehaviour
    {
        #region Fields

        private LayerMask _collectablesLayerMask;
        private GamebarController _gamebarController;

        private Collectable.Collectable _selectedItem;
        private Camera _mainCamera;
        private const float CooldownTime = 0.15f; // Little cooldown time to prevent double clicks and click spamming
        private float _lastClickTime;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _mainCamera = Camera.main;
            _gamebarController = FindObjectOfType<GamebarController>();
            _collectablesLayerMask = LayerMask.GetMask("Collectable");
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && Time.time > _lastClickTime + CooldownTime)
            {
                var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, _collectablesLayerMask))
                {
                    var collectableItem = hit.transform.GetComponent<Collectable.Collectable>();
                    if (collectableItem == null)
                        return;
                    _selectedItem = collectableItem;
                    _lastClickTime = Time.time;
                    ProcessSelectedObject(collectableItem);
                }
            }
            else if (Input.GetMouseButtonUp(0) && _selectedItem != null)
            {
            }
        }

        private async void ProcessSelectedObject(Collectable.Collectable collectable)
        {
            var emptySlot = _gamebarController.GetFirstEmptySlot();

            if (emptySlot == null)
            {
                // Highlight the collectable item with red color for a short time all slots full indication
                collectable.Highlight(true, Color.red);
                await Task.Delay(50);
                collectable.Highlight(false);
                return;
            }

            collectable.Highlight(true);
            collectable.TurnObject();
            collectable.ScaleObject(); // Scale effect for object when clicked
            collectable.DisableCollider();
            collectable.ChangeRenderLayerWithChilds("UICamera");

            _gamebarController.AddCollectableToSlot(collectable);
        }

        #endregion
    }
}
