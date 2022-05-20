using UnityEngine;

namespace Metroidvania.Entities
{
    /// <summary>Base class for handle all entity behaviours</summary>
    public abstract class EntityBehaviour : MonoBehaviour
    {
        public EntityObject entityObject { get; set; }
    }
}