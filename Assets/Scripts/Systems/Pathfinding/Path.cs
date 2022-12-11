using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Metroidvania.Pathfinding {
    public class Path {
        public List<Vector2> vectorPath { get; private set; }

        public Path() {
            vectorPath = new List<Vector2>();
        }

        public void Setup(GridGraph graph, NativeArray<int> nodesIndex, Vector2 start, Vector2 end) {
            vectorPath.Clear();
            foreach (int nodeIndex in nodesIndex) {
                PathNode node = graph.GetNodeByIndex(nodeIndex);
                vectorPath.Add(node.worldPosition);
            }
            vectorPath.Reverse();
            vectorPath[0] = start;
            vectorPath[vectorPath.Count - 1] = end;
        }

        public void SetupSinglePoint(GridGraph graph, Vector2 position) {
            vectorPath.Clear();
            vectorPath.Add(position);
        }
    }
}