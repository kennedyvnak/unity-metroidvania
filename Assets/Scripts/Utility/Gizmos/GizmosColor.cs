#if UNITY_EDITOR
using UnityEngine;

namespace Metroidvania
{
    /// <summary>Object for handle all gizmos color used in game</summary>
    public class GizmosColor : ScriptableSingleton<GizmosColor>
    {
        [System.Serializable]
        public class Player
        {
            public Color feet = Color.green;
            public Color hand = Color.yellow;
            public Color attack = Color.cyan;
            public Color colliderData = Color.green;
        }

        [System.Serializable]
        public class Pathfinding
        {
            public Color pathColor = Color.green;
        }

        [System.Serializable]
        public class Entities
        {
            public Color targetFinderFindRange = Color.yellow;
            public Color targetPosition = Color.cyan;
            public Color targetFinderVisibleTargetsLine = Color.red;
        }

        [System.Serializable]
        public class SafePoints
        {
            public Color area = Color.green;
        }

        public Player player;

        public SafePoints safePoints;

        public Entities entities;

        public Pathfinding pathfinding;
    }
}
#endif