using UnityEngine;

namespace Metroidvania.Pathfinding.Blocks {
    public abstract class GraphBlockBase : MonoBehaviour {
        private Pathfinder _Pathfinder;
        public Pathfinder pathfinder => _Pathfinder;

        private void Awake() {
            _Pathfinder = GetComponentInParent<Pathfinder>();
        }

        public abstract bool IsBlocked(PathNode node);
    }
}