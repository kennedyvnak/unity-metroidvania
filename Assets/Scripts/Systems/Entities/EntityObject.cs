using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Metroidvania.Entities {
    [CreateAssetMenu(fileName = "New entity object", menuName = "Scriptables/Entity")]
    public class EntityObject : ScriptableObject {
        public AssetReferenceGameObject prefab;
    }
}