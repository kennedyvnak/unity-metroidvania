using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Metroidvania.SceneManagement {
    [CreateAssetMenu(menuName = "Scriptables/Scene Management/Scene Channel")]
    public class SceneChannel : ScriptableObject {
        public enum SceneType { Gameplay, MainMenu, GameOver }

        [TextArea()] public string description;
        public SceneType sceneType;

        public AssetReference sceneReference;
        public SceneSpawnPoints spawnPoints;

        public AsyncOperationHandle<SceneInstance> operation;

        [HideInInspector] public AssetReferenceSceneChannel channelReference;

#if UNITY_EDITOR
        private void OnValidate() {
            channelReference = new AssetReferenceSceneChannel(UnityEditor.AssetDatabase.AssetPathToGUID(UnityEditor.AssetDatabase.GetAssetPath(this)));
        }
#endif
    }
}