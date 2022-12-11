using UnityEngine;

namespace Metroidvania.Entities {
    public class EntityTarget : MonoBehaviour {
        /// <summary>Cached transform of this object</summary>
        public Transform t { get; private set; }

        [SerializeField] private Vector2 m_offset;

        public Vector2 position => (Vector2)t.position + (m_offset * t.localScale);

        private void Awake() {
            EntitiesManager.instance.AddTarget(this);
            t = transform;
        }

        private void OnDestroy() {
            EntitiesManager.instance.RemoveTarget(this);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            new GizmosDrawer()
                .SetColor(GizmosColor.instance.entities.targetPosition)
                .DrawWireDisc((Vector2)transform.position + (m_offset * transform.localScale), 0.1f);
        }
#endif
    }
}