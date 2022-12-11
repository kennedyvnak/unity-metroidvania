using UnityEngine;

namespace Metroidvania.Environment.Fog {
    [RequireComponent(typeof(Collider2D))]
    public class BoundingFog : MonoBehaviour {
        private void OnTriggerEnter2D(Collider2D other) {
            // TODO: Add in-game visualisation of the warning
            Debug.Log("The fog is very dense, I can't walk through it.");
        }
    }
}
