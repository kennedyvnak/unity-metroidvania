using UnityEngine;

namespace Metroidvania.Entities {
    /// <summary>Base class for handle all entity behaviours</summary>
    public abstract class EntityBehaviour : MonoBehaviour {
        [SerializeField] private EntityObject m_EntityObject;
        public EntityObject entityObject => m_EntityObject;
    }
}