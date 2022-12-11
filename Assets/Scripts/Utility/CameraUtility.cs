using Cinemachine;
using UnityEngine;

namespace Metroidvania {
    public static class CameraUtility {
        private static Camera s_mainCamera;
        public static Camera mainCamera {
            get {
                if (!s_mainCamera)
                    s_mainCamera = Camera.main;
                return s_mainCamera;
            }
        }

        private static CinemachineVirtualCamera s_vCam;
        public static CinemachineVirtualCamera vCam {
            get {
                if (!s_vCam)
                    s_vCam = mainCamera.GetComponentInChildren<CinemachineVirtualCamera>(true);
                return s_vCam;
            }
        }
    }
}