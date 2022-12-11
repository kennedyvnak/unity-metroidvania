using System.Collections.Generic;
using UnityEngine;

namespace Metroidvania.Environment.Parallax {
    public class ParallaxEffectGroup : MonoBehaviour {
        private Camera _cam;
        private Vector2 _lastCameraPosition;

        private Dictionary<Transform, Vector3> _startPositions;

        private void Start() {
            _cam = Camera.main;
            _lastCameraPosition = _cam.transform.position;

            _startPositions = new Dictionary<Transform, Vector3>();
            foreach (Transform child in transform)
                _startPositions.Add(child, child.transform.position);
        }

        private void LateUpdate() {
            Vector2 camPosition = _cam.transform.position;

            foreach (KeyValuePair<Transform, Vector3> keyValue in _startPositions)
                keyValue.Key.position += ParallaxUtility.GetDeltaMove(_cam, keyValue.Value, _lastCameraPosition);

            _lastCameraPosition = camPosition;
        }
    }
}
