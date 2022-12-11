using UnityEngine;

namespace Metroidvania.SceneManagement {
    [RequireComponent(typeof(Collider2D))]
    public class SceneLoaderTrigger : MonoBehaviour {
        [SerializeField] private SceneLoadTransition transition;

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                SceneLoader.instance.LoadScene(transition.channel, transition.CreateData());
            }
        }
    }
}