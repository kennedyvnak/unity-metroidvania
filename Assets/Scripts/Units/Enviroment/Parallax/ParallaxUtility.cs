using UnityEngine;

namespace Metroidvania.Environment.Parallax {
    public static class ParallaxUtility {
        public static Vector3 GetDeltaMove(Camera cam, Vector3 startPosition, Vector2 lastCameraPosition) {
            float clipPlane = startPosition.z > 0 ? cam.farClipPlane : cam.nearClipPlane;
            Vector2 camPosition = cam.transform.position;
            float clippingPlane = cam.transform.position.z + clipPlane;
            float parallaxFactor = Mathf.Abs(startPosition.z) / clippingPlane;

            return (camPosition - lastCameraPosition) * parallaxFactor;
        }
    }
}