using Cinemachine;
using UnityEngine;

namespace Metroidvania {
    [RequireComponent(typeof(Collider2D))]
    public class CameraBounds : MonoBehaviour {
        private void Awake() {
            if (CameraUtility.vCam) {
                CinemachineConfiner2D confiner2D = CameraUtility.vCam.gameObject.AddComponent<CinemachineConfiner2D>();
                confiner2D.m_BoundingShape2D = GetComponent<Collider2D>();
                CameraUtility.vCam.AddExtension(confiner2D);
            }
        }
    }
}
