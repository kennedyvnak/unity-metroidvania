using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Metroidvania.Project.Editor
{
    // TODO: Set project validator as editor window
    /// <summary>Class for handle project validation</summary>
    public static class ProjectValidator
    {
        [MenuItem("Utility/Validate Project")]
        public static void ValidateProject()
        {
            ValidateScriptablesSingleton();
        }

        /// <summary>
        /// Validates all scriptable objects that derive from <see cref="Metroidvania.ScriptableSingleton{T}"/>
        /// </summary>
        public static void ValidateScriptablesSingleton()
        {
            var result = typeof(ScriptableSingleton<>).Assembly.GetTypes()
                .Where(t => t.BaseType is { IsGenericType: true } && t.BaseType.GetGenericTypeDefinition() ==
                    typeof(ScriptableSingleton<>));

            foreach (var type in result)
            {
                var assetsOfType = AssetDatabase.FindAssets($"t:{type.Name}");

                if (assetsOfType.Length == 1) continue;
                if (assetsOfType.Length > 1)
                {
                    string foundedAssets = string.Empty;
                    for (int i = 0; i < assetsOfType.Length; i++)
                        foundedAssets += $"{AssetDatabase.GUIDToAssetPath(assetsOfType[i])}\n";
                    GameDebugger.LogWarning($"More than one instance of singleton type '{type.Name}' fount in project. Founded assets path:\n{foundedAssets}");
                    continue;
                }

                var path = type.Name;

                var persistentDirectory = $"Assets/Data/Settings/{Path.GetDirectoryName(path)}";
                var persistentPath = $"{persistentDirectory}/{Path.GetFileName(path)}.asset";
                if (!Directory.Exists(persistentDirectory))
                    Directory.CreateDirectory(persistentDirectory);
                string fullPath = $"{persistentDirectory}/{Path.GetFileName(path)}.asset";
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(type), fullPath);
                GameDebugger.Log($"Created a instance of {type.Name} at '{fullPath}'");
            }
        }
    }
}