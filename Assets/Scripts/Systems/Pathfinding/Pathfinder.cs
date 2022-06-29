using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Metroidvania.Pathfinding
{
    public sealed class Pathfinder : MonoBehaviour
    {
        private NativeArray<CellPosition> _neighborsOffset;

        public GridGraph graph { get; private set; }
        public bool generatedGraph { get; private set; }

        private PathPool _pool;
        private Blocks.GraphBlockBase[] _blocks;

        [SerializeField] private int m_GraphWidth;
        [SerializeField] private int m_GraphHeight;
        [SerializeField] private float m_GraphCellSize;
        [SerializeField] private Vector2 m_GraphOffset;

        public int GraphWidth => generatedGraph ? graph.width : m_GraphWidth;
        public int GraphHeight => generatedGraph ? graph.height : m_GraphHeight;
        public float GraphCellSize => generatedGraph ? graph.cellSize : m_GraphCellSize;
        public Vector2 GraphOffset => generatedGraph ? graph.offset : m_GraphOffset;

        private void Awake()
        {
            _neighborsOffset = new NativeArray<CellPosition>(8, Allocator.Persistent);
            _neighborsOffset[0] = new CellPosition(-1, +0); // left 
            _neighborsOffset[1] = new CellPosition(+1, +0); // right 
            _neighborsOffset[2] = new CellPosition(+0, +1); // up
            _neighborsOffset[3] = new CellPosition(+0, -1); // down 
            _neighborsOffset[4] = new CellPosition(-1, -1); // left down
            _neighborsOffset[5] = new CellPosition(-1, +1); // left up
            _neighborsOffset[6] = new CellPosition(+1, -1); // right down
            _neighborsOffset[7] = new CellPosition(+1, +1); // right up
            _blocks = GetComponentsInChildren<Blocks.GraphBlockBase>();
            graph = new GridGraph(m_GraphWidth, m_GraphHeight, m_GraphCellSize, m_GraphOffset, _blocks);
            generatedGraph = true;
            _pool = new PathPool();
        }

        private void OnDestroy()
        {
            _neighborsOffset.Dispose();
            graph.nativeNodes.Dispose();
        }

        public Path FindPath(Vector2 start, Vector2 end)
        {
            return FindPath(graph.GetLocalPosition(start), graph.GetLocalPosition(end));
        }

        public Path FindPath(CellPosition start, CellPosition end)
        {
            // checks if start equals end, if true returns a path with a single point
            if (start.Equals(end))
            {
                Path singlePointPath = _pool.Get();
                singlePointPath.Setup(graph, start);
                return singlePointPath;
            }

            if (!graph.Contains(start) || !graph.Contains(end) // at least one of the positions is outside graph bounds
                || !graph.GetNode(end).walkable) // the end node isn't walkable 
                return null;

            NativeList<int> generatedPath = new NativeList<int>(Allocator.TempJob);

            // create, schedule and complete the pathfinding job
            new PathFindJob()
            {
                start = start,
                end = end,
                gridSize = new CellPosition(graph.width, graph.height),
                pathNodes = graph.nativeNodes,
                neighborOffsets = _neighborsOffset,
                generatedPath = generatedPath,
            }.Schedule().Complete();

            Path path = null;
            // A check if the pathfinding found a path, if not return null
            if (generatedPath.Length > 0)
            {
                // get a path in the pool and setup it
                path = _pool.Get();
                path.Setup(graph, generatedPath);
            }

            generatedPath.Dispose();

            return path;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_GraphWidth < 0)
                m_GraphWidth = 0;
            if (m_GraphHeight < 0)
                m_GraphHeight = 0;

            if (TryGetComponent<PathfinderRenderer>(out var renderer))
                renderer.GenerateMesh();
        }
#endif

        public void ReleasePath(ref Path p) => _pool.Release(ref p);

        public Vector2 GetCellWorldPosition(CellPosition cell)
        {
            return GridGraph.GetWorldPosition(cell, GraphCellSize, GraphOffset);
        }
    }
}