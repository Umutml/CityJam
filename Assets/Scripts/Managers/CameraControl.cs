using UnityEngine;

namespace Managers
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed = 10f;
        [SerializeField] private float zoomSpeed = 0.2f;
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 20f;

        private Camera _camera;
        private float _lastTapTime;
        private float _currentZoom;
        private readonly float _zoomLerpSpeed = 10f;

        private void Start()
        {
            _camera = Camera.main;
            if (_camera != null) _currentZoom = _camera.orthographicSize;
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouseScroll();
#endif

#if UNITY_ANDROID || UNITY_IOS
            HandleTouchZoom();
#endif
        }

        private void HandleMouseScroll()
        {
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0.0f)
            {
                _currentZoom -= scroll * scrollSpeed;
                _currentZoom = Mathf.Clamp(_currentZoom, minZoom, maxZoom);
            }

            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _currentZoom, Time.deltaTime * _zoomLerpSpeed);
        }

        private void HandleTouchZoom()
        {
            if (Input.touchCount == 2)
            {
                var touchZero = Input.GetTouch(0);
                var touchOne = Input.GetTouch(1);

                var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                var touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                var deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                _currentZoom += deltaMagnitudeDiff * zoomSpeed;
                _currentZoom = Mathf.Clamp(_currentZoom, minZoom, maxZoom);
            }

            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _currentZoom, Time.deltaTime * _zoomLerpSpeed);
        }
    }
}
