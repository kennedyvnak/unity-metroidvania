using UnityEngine;

namespace Metroidvania
{
    public class PlatformerShadow : MonoBehaviour
    {
        [SerializeField] private float m_distance;
        [SerializeField] private Color m_color;
        [SerializeField] private LayerMask m_groundLayer;

        private SpriteRenderer _sRender;

        private void Awake()
        {
            _sRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            var ray = Physics2D.Raycast(transform.position, Vector2.down, m_distance, m_groundLayer);
            if (ray)
            {
                _sRender.enabled = true;
                _sRender.transform.position = ray.point;
                float fadeOut = 1 - ray.distance / m_distance;
                _sRender.transform.localScale = Vector2.one * fadeOut;
                _sRender.color = new Color(m_color.r, m_color.g, m_color.b, m_color.a * fadeOut);
            }
            else
            {
                _sRender.enabled = false;
            }
        }
    }
}