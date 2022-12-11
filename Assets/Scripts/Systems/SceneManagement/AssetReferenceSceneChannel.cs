using UnityEngine.AddressableAssets;

namespace Metroidvania.SceneManagement {
    [System.Serializable]
    public class AssetReferenceSceneChannel : AssetReferenceT<SceneChannel> {
        public AssetReferenceSceneChannel(string guid) : base(guid) {
        }
    }
}