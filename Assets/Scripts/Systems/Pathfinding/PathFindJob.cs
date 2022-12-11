using Unity.Collections;

namespace Metroidvania.Pathfinding {
    [Unity.Burst.BurstCompile]
    public struct PathFindJob : Unity.Jobs.IJob {
        private const int k_MoveStraightCost = 10; // sqrt(1) * 10 
        private const int k_MoveDiagonalCost = 14; // sqrt(2) * 10

        public CellPosition start;
        public CellPosition end;
        public CellPosition gridSize;

        [ReadOnly]
        public NativeArray<CellPosition> neighborOffsets;

        public NativeArray<PathNodeReference> pathNodes;

        [WriteOnly]
        public NativeList<int> generatedPath;

        public void Execute() {
            // Reset nodes properties
            for (int i = 0; i < pathNodes.Length; i++) {
                PathNodeReference node = pathNodes[i];
                node.g = int.MaxValue;
                node.cameFromNodeIndex = -1;
                pathNodes[i] = node;
            }

            int endNodeIndex = CalculateIndex(end);

            // Setup start node
            PathNodeReference startNode = pathNodes[CalculateIndex(start)];
            startNode.g = 0;
            startNode.h = CalculateDistanceCost(start, end);
            pathNodes[startNode.index] = startNode;

            // list containing the index of discovered nodes that may need to be (re-)expanded
            NativeList<int> openList = new NativeList<int>(Allocator.Temp);

            // list containing the index of found nodes
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.index);

            // loop for each open node
            while (openList.Length > 0) {
                // get the node with the lowest f score in the list of open nodes
                PathNodeReference currentNode = GetLowestFNode(openList, pathNodes);

                // reached the end
                if (currentNode.index == endNodeIndex)
                    break;

                // remove the index of the current node in the open list
                for (int i = 0; i < openList.Length; i++) {
                    if (openList[i] == currentNode.index) {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closedList.Add(currentNode.index);

                // loops between all current node neighbors
                for (int i = 0; i < neighborOffsets.Length; i++) {
                    // get the offset and apply it to the current node position
                    CellPosition neighborOffset = neighborOffsets[i];
                    CellPosition neighborPosition = currentNode.position + neighborOffset;

                    // the neighbor position isn't inside the grid
                    if (!ContainsPosition(neighborPosition, gridSize))
                        continue;

                    int neighborNodeIndex = CalculateIndex(neighborPosition);

                    // the neighbor node has already been searched
                    if (closedList.Contains(neighborNodeIndex))
                        continue;

                    PathNodeReference neighborNode = pathNodes[neighborNodeIndex];
                    // the neighbor node isn't walkable
                    if (!neighborNode.walkable)
                        continue;

                    // tentative.gCost is the distance from start to the neighbor through current
                    int tentativeGCost = currentNode.g + CalculateDistanceCost(currentNode.position, neighborPosition);
                    if (tentativeGCost < neighborNode.g) {
                        // recording this path because is better than any previous one.
                        neighborNode.g = tentativeGCost;
                        neighborNode.h = CalculateDistanceCost(neighborPosition, end);
                        neighborNode.cameFromNodeIndex = currentNode.index;
                        pathNodes[neighborNodeIndex] = neighborNode;

                        if (!openList.Contains(neighborNodeIndex))
                            openList.Add(neighborNodeIndex);
                    }
                }
            }

            PathNodeReference endNode = pathNodes[endNodeIndex];

            CalculatePath(pathNodes, endNode, generatedPath);

            openList.Dispose();
            closedList.Dispose();
        }

        private void CalculatePath(NativeArray<PathNodeReference> pathNodes, PathNodeReference endNode, NativeList<int> path) {
            // if endNode.cameFromNodeIndex is equals to -1, it means that no path was found
            if (endNode.cameFromNodeIndex != -1) {
                path.Add(endNode.index);

                PathNodeReference currentNode = endNode;
                while (currentNode.cameFromNodeIndex != -1) {
                    PathNodeReference cameFromNode = pathNodes[currentNode.cameFromNodeIndex];
                    path.Add(cameFromNode.index);
                    currentNode = cameFromNode;
                }
            }
        }

        private int CalculateDistanceCost(CellPosition a, CellPosition b) {
            int xDistance = System.Math.Abs(a.x - b.x);
            int yDistance = System.Math.Abs(a.y - b.y);
            int remaining = System.Math.Abs(xDistance - yDistance);
            return (k_MoveDiagonalCost * System.Math.Min(xDistance, yDistance)) + (k_MoveStraightCost * remaining);
        }

        private PathNodeReference GetLowestFNode(NativeList<int> openList, NativeArray<PathNodeReference> pathNodes) {
            PathNodeReference lowest = pathNodes[openList[0]];
            for (int i = 1; i < openList.Length; i++) {
                PathNodeReference testPathNode = pathNodes[openList[i]];
                if (testPathNode.f < lowest.f)
                    lowest = testPathNode;
            }
            return lowest;
        }

        private bool ContainsPosition(CellPosition gridPosition, CellPosition gridSize) {
            return gridPosition.x >= 0 && gridPosition.x < gridSize.x && gridPosition.y >= 0 && gridPosition.y < gridSize.y;
        }

        private int CalculateIndex(CellPosition position) {
            return position.x + (position.y * gridSize.x);
        }
    }
}