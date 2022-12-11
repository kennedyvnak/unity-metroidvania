using UnityEngine;

namespace Metroidvania.Pathfinding {
    public class PathNode {
        public readonly GridGraph graph;

        public readonly CellPosition position;

        public readonly Vector2 worldPosition;

        public readonly int index;

        private bool _walkable;
        public bool walkable {
            get => _walkable;
            set {
                PathNodeReference nativeNode = graph.nativeNodes[index];
                nativeNode.walkable = value;
                graph.nativeNodes[index] = nativeNode;
                _walkable = value;
                graph.InvokeNodeChanged(this);
            }
        }

        public PathNode(GridGraph graph, CellPosition cell) {
            this.graph = graph;
            position = cell;
            index = cell.x + (cell.y * graph.width);
            _walkable = true;
            worldPosition = graph.GetWorldPosition(cell) + (new Vector2(.5f, .5f) * graph.cellSize);
        }

        public PathNodeReference GetReference() => new PathNodeReference(position, walkable, index);
    }
}