using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Metroidvania.Project.Editor
{
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
                var at = type.GetCustomAttribute<ResourceObjectPathAttribute>();
                var path = at != null ? at.path : type.Name;

                var persistentDirectory = $"Assets/Resources/{Path.GetDirectoryName(path)}";
                var persistentPath = $"{persistentDirectory}/{Path.GetFileName(path)}.asset";
                if (File.Exists(persistentPath))
                    continue;

                if (!Directory.Exists(persistentDirectory))
                    Directory.CreateDirectory(persistentDirectory);
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(type),
                    $"{persistentDirectory}/{Path.GetFileName(path)}.asset");
            }
        }
    }
}