using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Metroidvania.Pathfinding
{
    public class Path
    {
        public List<Vector2> vectorPath { get; private set; }

        public Path()
        {
            vectorPath = new List<Vector2>();
        }

        public void Setup(GridGraph graph, NativeArray<int> nodesIndex)
        {
            vectorPath.Clear();
            foreach (int nodeIndex in nodesIndex)
            {
                PathNode node = graph.GetNodeByIndex(nodeIndex);
                vectorPath.Add(node.worldPosition);
            }
            vectorPath.Reverse();
        }

        public void Setup(GridGraph graph, CellPosition cell)
        {
            vectorPath.Clear();
            vectorPath.Add(graph.GetWorldPosition(cell));
        }
    }
}