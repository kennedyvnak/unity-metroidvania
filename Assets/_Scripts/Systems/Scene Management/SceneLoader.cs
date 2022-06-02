using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Metroidvania.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader instance { get; private set; }

        [SerializeField] private AssetReference m_mainMenu;

        [Header("Transition")]
        [SerializeField] private CanvasGroup m_loadScreenGroup;
        [SerializeField] private Slider m_progressSlider;

        public void Initialize()
        {
            var go = gameObject;
            if (instance)
            {
                Destroy(go);
                return;
            }

            instance = this;
            go.name = "[SceneLoader]";
            DontDestroyOnLoad(go);
        }

        public void LoadMainMenu()
        {
            StartCoroutine(DOLoad(m_mainMenu));
        }

        public void LoadScene(AssetReference sceneRef)
        {
            DOLoad(sceneRef);
        }

        private IEnumerator DOLoad(AssetReference sceneRef)
        {
            var handle = sceneRef.LoadSceneAsync();
            yield return handle;
        }
    }
}