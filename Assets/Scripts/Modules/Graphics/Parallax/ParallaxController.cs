using UnityEngine;

namespace Metroidvania.Graphics
{
    public class ParallaxController : MonoBehaviour
    {
        private void LateUpdate()
        {
            var mainCam = Helpers.mainCamera;
            var camPos = mainCam.transform.position;
            Parallax.FlushBuffer(mainCam.farClipPlane, mainCam.nearClipPlane, camPos);
        }
    }
}