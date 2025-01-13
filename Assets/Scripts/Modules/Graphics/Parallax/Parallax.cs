using UnityEngine;

namespace Metroidvania.Graphics
{
    public readonly struct ParallaxObjectData
    {
        public readonly bool fixedY;
        public readonly Vector3 startPos;
        public readonly float absStartZ;
        public readonly bool positiveZ;

        public ParallaxObjectData(Vector3 startPos, bool fixedY = false)
        {
            this.startPos = startPos;
            this.fixedY = fixedY;
            absStartZ = Mathf.Abs(startPos.z);
            positiveZ = startPos.z > 0.0f;
        }
    }

    public static class Parallax
    {
        private static float _farClipPlane, _nearClipPlane;
        private static Vector3 _camPos;

        public static void FlushBuffer(float farClipPlane, float nearClipPlane, Vector3 camPos)
        {
            _farClipPlane = farClipPlane;
            _nearClipPlane = nearClipPlane;
            _camPos = camPos;
        }

        public static Vector3 GetParallaxPosition(ParallaxObjectData objData, Vector3 position)
        {
            float parallaxFactor = GetParallaxFactor(objData);

            float distance = _camPos.x * parallaxFactor;

            return new Vector3(objData.startPos.x + distance, objData.fixedY ? _camPos.y + objData.startPos.y : position.y, position.z);
        }

        public static float GetParallaxFactor(ParallaxObjectData objData)
        {
            float clipPlane = objData.positiveZ ? _farClipPlane : _nearClipPlane;
            float clippingPlane = _camPos.z + clipPlane;
            return objData.absStartZ / clippingPlane;
        }
    }
}