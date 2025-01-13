using Unity.Cinemachine;
using UnityEngine;

namespace Metroidvania
{
    [RequireComponent(typeof(Collider2D))]
    public class CameraBounds : MonoBehaviour
    {
        private void Awake()
        {
            if (Helpers.virtualCamera)
            {
                CinemachineConfiner2D confiner2D = Helpers.virtualCamera.gameObject.AddComponent<CinemachineConfiner2D>();
                confiner2D.BoundingShape2D = GetComponent<Collider2D>();
            }
        }
    }
}
