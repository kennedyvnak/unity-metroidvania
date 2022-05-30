using UnityEngine;

namespace Metroidvania
{
    public static class CameraUtility
    {
        private static Camera s_mainCamera;
        public static Camera mainCamera
        {
            get
            {
                if (s_mainCamera == null) s_mainCamera = Camera.main;
                return s_mainCamera;
            }
        }
    }
}