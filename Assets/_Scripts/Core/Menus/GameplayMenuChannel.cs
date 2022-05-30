using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Metroidvania.UI.Menus
{
    [CreateAssetMenu(fileName = "New Gameplay Menu", menuName = "Scriptables/Menus/Menu Channel")]
    public class GameplayMenuChannel : ScriptableObject
    {
        [SerializeField] protected AssetReferenceGameObject prefabReference;

        protected AsyncOperationHandle<GameObject> currentOperation { get; set; }
        protected GameObject runingInstance { get; set; }

        public AssetReferenceGameObject PrefabReference => prefabReference;

        public System.Action MenuReleased;

        public IEnumerator LoadMenu(System.Action<GameplayMenuInstance> onCompleteOperation = null)
        {
            if (runingInstance != null) yield break;

            currentOperation = prefabReference.InstantiateAsync(UIUtility.mainCanvas.transform, false);
            currentOperation.Completed += (op) => runingInstance = op.Result;
            yield return currentOperation;

            var menuInstance = currentOperation.Result.GetComponent<GameplayMenuInstance>();

            if (menuInstance == null)
                throw new System.Exception("The variable 'Prefab Reference' don't is a GameplayMenu.");

            yield return menuInstance.InitOperation(this);

            onCompleteOperation?.Invoke(menuInstance);
        }

        public IEnumerator UnloadMenuInstance()
        {
            if (runingInstance == null) yield break;

            yield return runingInstance.GetComponent<GameplayMenuInstance>()?.ReleaseOperation();

            Addressables.ReleaseInstance(currentOperation);
            Addressables.ReleaseInstance(runingInstance.gameObject);
            runingInstance = null;
            MenuReleased?.Invoke();
        }
    }
}