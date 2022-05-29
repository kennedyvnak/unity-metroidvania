using UnityEngine;

namespace Metroidvania
{
    // TODO: Add in-game debug screen
    [ResourceObjectPath("Data/Game Debugger")]
    public class GameDebugger : ScriptableSingleton<GameDebugger>
    {
        [SerializeField] private bool m_forceDebug = true;

        [Header("Instances")]
        [SerializeField] private bool m_debugSerialization;
        [SerializeField] private bool m_enableEntitiesLogs;

        public bool debugSerialization
        {
            get
            {
                if (Application.isEditor || m_forceDebug)
                    return m_debugSerialization;
                return false;
            }
        }
        public bool enableEntitiesLogs
        {
            get
            {
                if (Application.isEditor || m_forceDebug)
                    return m_enableEntitiesLogs;
                return false;
            }
        }

        public static void Log(object message, Object target = null)
        {
            Debug.Log(message, target);
        }

        public static void LogWarning(object message, Object target = null)
        {
            Debug.LogWarning(message, target);
        }

        public static void LogError(object message, Object target = null)
        {
            Debug.LogError(message, target);
        }
    }
}