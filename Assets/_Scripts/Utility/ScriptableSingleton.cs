using System.Reflection;
using UnityEngine;

namespace Metroidvania
{
    /// <summary>Base class for handle scriptable objects singleton</summary>
    public abstract class ScriptableSingleton<T> : ScriptableObject
        where T : ScriptableSingleton<T>
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

                var path = GetPath();
                s_instance = Resources.Load<T>(path);
                if (s_instance != null) return s_instance;

                s_instance = CreateInstance<T>();

                return s_instance;
            }
        }

        protected static string GetPath()
        {
            var at = typeof(T).GetCustomAttribute<ResourceObjectPathAttribute>();
            return at != null ? at.path : typeof(T).Name;
        }
    }
}