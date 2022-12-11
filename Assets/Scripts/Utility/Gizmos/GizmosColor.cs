#if UNITY_EDITOR
using UnityEngine;

namespace Metroidvania {
    /// <summary>Object for handle all gizmos color used in game</summary>
    public class GizmosColor : ScriptableSingleton<GizmosColor> {
        [System.Serializable]
        public class Ch_Knight {
            public Color feet = Color.green;
            public Color hand = Color.yellow;
            public Color attack = Color.cyan;
            public Color colliderData = Color.green;
        }

        [System.Serializable]
        public class Pathfinding {
            public Color pathColor = Color.green;
        }

        [System.Serializable]
        public class Entities {
            public Color targetFinderFindRange = Color.yellow;
            public Color targetPosition = Color.cyan;
            public Color targetFinderVisibleTargetsLine = Color.red;
        }

        [System.Serializable]
        public class SafePoints {
            public Color area = new Color(0, 1, 0, 0.3f);
            public Color handles = Color.green;
        }

        [System.Serializable]
        public class Fog {
            public Color main = Color.cyan;
            public Color secondary = Color.red;
        }

        public Ch_Knight knight;

        public SafePoints safePoints;

        public Entities entities;

        public Pathfinding pathfinding;

        public Fog fog;
    }
}
#endif