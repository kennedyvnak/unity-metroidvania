using UnityEngine;

namespace Metroidvania {
    // TODO: Add in-game debug screen
    public class GameDebugger : ScriptableSingleton<GameDebugger> {
        [SerializeField] private bool m_forceDebug = true;

        [Header("Instances")]
        [SerializeField] private bool m_debugSerialization;
        [SerializeField] private bool m_enableEntitiesLogs;
        [SerializeField] private bool m_debugInput;

        public bool debugSerialization => m_debugSerialization && debugEnabled;
        public bool enableEntitiesLogs => m_enableEntitiesLogs && debugEnabled;
        public bool debugInput => m_debugInput && debugEnabled;

        public static bool debugEnabled => Application.isEditor || instance.m_forceDebug;

        public static void Log(object message, Object target = null) {
            Debug.Log(message, target);
        }

        public static void LogWarning(object message, Object target = null) {
            Debug.LogWarning(message, target);
        }

        public static void LogError(object message, Object target = null) {
            Debug.LogError(message, target);
        }
    }
}