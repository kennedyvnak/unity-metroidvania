using UnityEngine;

namespace Metroidvania.Entities
{
    public class EntityTarget : MonoBehaviour
    {
        /// <summary>Cached transform of this object</summary>
        public Transform t { get; private set; }

        private void Awake()
        {
            EntitiesManager.instance.AddTarget(this);
            t = transform;
        }

        private void OnDestroy()
        {
            EntitiesManager.instance.RemoveTarget(this);
        }
    }
}