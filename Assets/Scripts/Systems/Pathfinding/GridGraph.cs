using Metroidvania.Pathfinding.Blocks;
using Unity.Collections;
using UnityEngine;

namespace Metroidvania.Pathfinding {
    public class GridGraph {
        public event System.Action<PathNode> NodeChanged;

        public readonly int width;
        public readonly int height;

        public readonly float cellSize;

        public readonly Vector2 offset;

        public readonly GraphBlockBase[] blocks;

        public PathNode[] nodes;
        public NativeArray<PathNodeReference> nativeNodes;

        public GridGraph(int width, int height, float cellSize, Vector2 offset, Blocks.GraphBlockBase[] blocks) {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.offset = offset;
            this.blocks = blocks;

            int size = width * height;
            nodes = new PathNode[size];
            nativeNodes = new NativeArray<PathNodeReference>(size, Allocator.Persistent);

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int i = x + (y * width);
                    PathNode node = nodes[i] = new PathNode(this, new CellPosition(x, y));
                    nativeNodes[i] = node.GetReference();
                    for (int j = 0; j < blocks.Length; j++) {
                        if (blocks[j].IsBlocked(node)) {
                            node.walkable = false;
                            break;
                        }
                    }
                }
            }
        }

        public void InvokeNodeChanged(PathNode node) => NodeChanged?.Invoke(node);

        public CellPosition GetLocalPosition(Vector2 worldPosition) {
            return new CellPosition(Mathf.FloorToInt((worldPosition.x - offset.x) / cellSize), Mathf.FloorToInt((worldPosition.y - offset.y) / cellSize));
        }

        public Vector2 GetWorldPosition(CellPosition cell) => GetWorldPosition(cell, cellSize, offset);

        public static Vector2 GetWorldPosition(CellPosition cell, float cellSize, Vector2 offset) => (new Vector2(cell.x, cell.y) * cellSize) + offset;

        public PathNode GetNode(CellPosition cell) {
            return !Contains(cell) ? null : nodes[cell.x + (cell.y * width)];
        }

        public PathNode GetNode(Vector2 worldPosition) {
            return GetNode(GetLocalPosition(worldPosition));
        }

        public PathNode GetNodeByIndex(int index) {
            return index < 0 || index >= nodes.Length ? null : nodes[index];
        }

        public bool Contains(CellPosition cell) {
            return cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;
        }
    }
}