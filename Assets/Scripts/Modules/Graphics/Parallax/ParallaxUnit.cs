using UnityEngine;

namespace Metroidvania.Graphics
{
    public class ParallaxUnit : MonoBehaviour
    {
        [SerializeField] private bool m_fixedY;

        private ParallaxObjectData _data;

        private void Start()
        {
            _data = new ParallaxObjectData(transform.position, m_fixedY);
        }

        private void LateUpdate()
        {
            transform.position = Parallax.GetParallaxPosition(_data, transform.position);
        }
    }
}