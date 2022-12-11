using UnityEngine;

namespace Metroidvania.Environment.Parallax {
    public class ParallaxEffect : MonoBehaviour {
        private Camera _cam;
        private Vector3 _startPosition;
        private Vector2 _lastCameraPosition;

        private void Start() {
            _startPosition = transform.position;
            _cam = Camera.main;
            _lastCameraPosition = _cam.transform.position;
        }

        private void LateUpdate() {
            transform.position += ParallaxUtility.GetDeltaMove(_cam, _startPosition, _lastCameraPosition);
            _lastCameraPosition = _cam.transform.position;
        }
    }
}
