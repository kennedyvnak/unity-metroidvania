using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Metroidvania {
    /// <summary>Base class for handle scriptable objects singleton</summary>
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T> {
        private static T s_instance;

        /// <summary>
        /// The instance of type(<see cref="T"/>) located in the resources,
        /// if it does not exist in the project, creates a new one
        /// </summary>
        public static T instance {
            get {
                if (s_instance)
                    return s_instance;

                AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(typeof(T).FullName);
                if (handle.Status == AsyncOperationStatus.Failed) {
                    Debug.LogWarning("Singleton instance load failed. Creating a new default instance.");
                    return s_instance = ScriptableObject.CreateInstance<T>();
                }
                return s_instance = handle.WaitForCompletion();
            }
        }
    }
}