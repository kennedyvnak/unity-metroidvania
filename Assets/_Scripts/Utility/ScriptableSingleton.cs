using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Metroidvania
{
    /// <summary>Base class for handle scriptable objects singleton</summary>
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static T s_instance;

        /// <summary>
        /// The instance of type(<see cref="T"/>) located in the resources,
        /// if it does not exist in the project, creates a new one
        /// </summary>
        public static T instance
        {
            get
            {
                if (s_instance != null) return s_instance;

#if UNITY_EDITOR
                T[] results = System.Array.ConvertAll(UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}"),
                    x => UnityEditor.AssetDatabase.LoadAssetAtPath<T>(UnityEditor.AssetDatabase.GUIDToAssetPath(x)));
#else
                T[] results = Resources.FindObjectsOfTypeAll<T>();
#endif
                if (results.Length == 1)
                    return s_instance = results[0];
                else if (results.Length > 1)
                    GameDebugger.LogError($"More than one instance of singleton type {typeof(T).Name} was found in project.");

                GameDebugger.LogWarning($"Creating an instance of singleton type {typeof(T).Name}");
                return s_instance = CreateInstance<T>();
            }
        }
    }
}