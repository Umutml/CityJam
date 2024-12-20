using DG.Tweening;
using UnityEngine;

namespace Gamecore.Gamebar
{
    public class GamebarSlot : MonoBehaviour
    {
        private GameObject _occupyingObject;
        private bool _sequenceActive;
        private RectTransform _rectTransform;
        private bool _isAnimating;

        public bool IsOccupied { get; private set; }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetAnimating(bool isAnimating)
        {
            _isAnimating = isAnimating;
        }

        public bool GetIsAnimating()
        {
            return _isAnimating;
        }


        public void SetOccupyingObject(GameObject occupyingObject)
        {
            _occupyingObject = occupyingObject;
        }

        public void ClearOccupyingObject()
        {
            _occupyingObject = null;
        }

        public GameObject GetOccupyingObject()
        {
            return _occupyingObject;
        }

        public void SetOccupied(bool isOccupied)
        {
            IsOccupied = isOccupied;
        }

        public void Bounce()
        {
            if (_sequenceActive)
                return;

            _sequenceActive = true;

            var downPosition = _rectTransform.position - new Vector3(0, 0.2f, 0);
            var upPosition = _rectTransform.position;

            var sequence = DOTween.Sequence();
            sequence.Append(_rectTransform.DOMove(downPosition, 0.1f));
            sequence.Append(_rectTransform.DOMove(upPosition, 0.1f).SetEase(Ease.InOutBounce)).OnComplete(() =>
            {
                _sequenceActive = false;
                _isAnimating = false;
            });
        }
    }
}
