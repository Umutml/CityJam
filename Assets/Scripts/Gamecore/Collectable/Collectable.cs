using DG.Tweening;
using EPOOutline;
using Managers;
using UnityEngine;

namespace Gamecore
{
    public class Collectable : MonoBehaviour
    {
        private Outlinable _outlineable;
        private BoxCollider _boxCollider;
        private readonly float _duration = 0.5f;
        private readonly Vector3 _targetRotation = new(-90, 180, 0); // The target rotation need to be UI like rotation
        [SerializeField] private CollectableTypes collectableTypes;

        // Create a method to get the collectable type
        

        public CollectableTypes GetCollectableType()
        {
            return collectableTypes;
        }

        public void DestroyCollectable()
        {
            Destroy(gameObject);
        }

        private void Awake()
        {
            _outlineable = GetComponent<Outlinable>();
            _boxCollider = GetComponent<BoxCollider>();
            _outlineable.enabled = false;
            LevelManager.OnLevelCompleted += DestroyCollectable;
        }

        public void Highlight(bool highlight, Color color = default)
        {
            if (color == default)
            {
                color = Color.cyan;
            }

            if (highlight)
            {
                _outlineable.OutlineParameters.Color = color;
                _outlineable.enabled = true;
            }
            else
            {
                if (_outlineable) _outlineable.enabled = false;
            }
        }

        public void ChangeRenderLayerWithChilds(string layerName)
        {
            var layer = LayerMask.NameToLayer(layerName);
            SetLayerRecursively(gameObject, layer);
        }

        private void SetLayerRecursively(GameObject obj, int newLayer) // Change the layer of the collectable and its childrens
        {
            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

        public void TurnObject()
        {
            transform.DORotate(_targetRotation, _duration).SetEase(Ease.OutSine).OnComplete(() =>
            {
                transform.DORotate(Vector3.zero, _duration).SetEase(Ease.OutSine); // Reset the rotation
            });
        }

        public void ScaleObject()
        {
            var firstScale = transform.localScale;
            transform.DOScale(transform.localScale * 1.1f, _duration).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                transform.DOScale(firstScale, _duration).SetEase(Ease.OutQuad); // Reset the scale
            });
        }

        public void DisableCollider()
        {
            _boxCollider.enabled = false;
            // Disable the collider of the collectable
        }

        public void Bounce()
        {
            // Bounce the collectable
            var downPosition = transform.position - new Vector3(0, 0.3f, 0);
            var upPosition = transform.position;

            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(downPosition, 0.1f));
            sequence.Append(transform.DOMove(upPosition, 0.1f).SetEase(Ease.InOutBounce));
        }

        public void ResetObjectState()
        {
            // Reset the object state
            _outlineable.enabled = false;
            _boxCollider.enabled = true;
        }
        
        private void OnDestroy()
        {
            LevelManager.OnLevelCompleted -= DestroyCollectable;
        }
    }
}
