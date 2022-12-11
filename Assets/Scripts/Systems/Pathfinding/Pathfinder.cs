using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Metroidvania.Pathfinding {
    public sealed class Pathfinder : Singleton<Pathfinder> {
        [SerializeField] private GraphRenderer m_GraphRenderer;
        public GraphRenderer graphRenderer {
            get {
                m_GraphRenderer.pathfinder = this;
                return m_GraphRenderer;
            }
        }

        private NativeArray<CellPosition> _neighborsOffset;

        public GridGraph graph { get; private set; }
        public bool generatedGraph { get; private set; }

        private PathPool _pool;
        private Blocks.GraphBlockBase[] _blocks;

        [Header("Properties")]
        [SerializeField, Min(0)] private int m_GraphWidth;
        [SerializeField, Min(0)] private int m_GraphHeight;
        [SerializeField, Min(0)] private float m_GraphCellSize;
        [SerializeField] private Vector2 m_GraphOffset;

        public int GraphWidth => generatedGraph ? graph.width : m_GraphWidth;
        public int GraphHeight => generatedGraph ? graph.height : m_GraphHeight;
        public float GraphCellSize => generatedGraph ? graph.cellSize : m_GraphCellSize;
        public Vector2 GraphOffset => generatedGraph ? graph.offset : m_GraphOffset;

        protected sealed override void Awake() {
            base.Awake();
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
            graphRenderer.Start();
        }

        protected sealed override void OnDestroy() {
            _neighborsOffset.Dispose();
            graph.nativeNodes.Dispose();
            graphRenderer.Dispose();
            base.OnDestroy();
        }

        public Path FindPath(Vector2 start, Vector2 end) {
            CellPosition startCell = graph.GetLocalPosition(start);
            CellPosition endCell = graph.GetLocalPosition(end);
            // checks if start equals end, if true returns a path with a single point
            if (startCell.Equals(endCell)) {
                Path singlePointPath = _pool.Get();
                singlePointPath.SetupSinglePoint(graph, start);
                return singlePointPath;
            }

            if (!graph.Contains(startCell) || !graph.Contains(endCell) // at least one of the positions is outside graph bounds
                || !graph.GetNode(endCell).walkable) // the end node isn't walkable 
                return null;

            NativeList<int> generatedPath = new NativeList<int>(Allocator.TempJob);

            // create, schedule and complete the pathfinding job
            new PathFindJob() {
                start = startCell,
                end = endCell,
                gridSize = new CellPosition(graph.width, graph.height),
                pathNodes = graph.nativeNodes,
                neighborOffsets = _neighborsOffset,
                generatedPath = generatedPath,
            }.Schedule().Complete();

            Path path = null;
            // A check if the pathfinding found a path, if not return null
            if (generatedPath.Length > 0) {
                // get a path in the pool and setup it
                path = _pool.Get();
                path.Setup(graph, generatedPath, start, end);
            }

            generatedPath.Dispose();

            return path;
        }

        public void ReleasePath(ref Path p) => _pool.Release(ref p);

        public Vector2 GetCellWorldPosition(CellPosition cell) {
            return GridGraph.GetWorldPosition(cell, GraphCellSize, GraphOffset);
        }

#if UNITY_EDITOR
        private void OnValidate() {
            graphRenderer.Validate();
        }

        private void OnDrawGizmos() {
            graphRenderer.DrawGizmos();
        }
#endif

        [System.Serializable]
        public class GraphRenderer {
            private static readonly Quaternion k_vr0 = Quaternion.Euler(0, 0, -270);
            private static readonly Quaternion k_vr1 = Quaternion.Euler(0, 0, -180);
            private static readonly Quaternion k_vr2 = Quaternion.Euler(0, 0, -90);
            private static readonly Quaternion k_vr3 = Quaternion.Euler(0, 0, 0);

            [SerializeField] private bool m_DrawGizmos;

            [Header("Nodes")]
            [SerializeField, Range(0, 1)] private float m_NodeTransparency = 0.5f;
            [SerializeField, ColorUsage(false, false)] private Color m_WalkableColor = new Color(0.14f, 0.4f, 0.9f);
            [SerializeField, ColorUsage(false, false)] private Color m_UnWalkableColor = new Color(1, 0, 0);

            [Header("Lines")]
            [SerializeField, ColorUsage(true, false)] private Color m_LineColor = new Color(0.7f, 0.6f, 0.2f, 0.5f);

            [System.NonSerialized] public Pathfinder pathfinder;

            private bool initedData { get; set; }

            private Mesh _mesh;
            public Mesh generatedMesh => _mesh;

            private Material _mat;

            private Vector3[] _vertices;
            private Vector2[] _uv;
            private Vector3[] _normals;
            private int[] _tris;
            private Color[] _colors;

            public void Start() {
                if (pathfinder.graph != null)
                    pathfinder.graph.NodeChanged += UpdateNode;
            }

            public void Dispose() {
                if (initedData) {
                    DestroyImmediate(_mat);
                    DestroyImmediate(_mesh);
                }
            }

            private void InitMeshData() {
                if (!_mesh) {
                    _mat = new Material(Shader.Find("Sprites/Default"));
                    _mesh = new Mesh {
                        name = "Pathfinder-graph"
                    };

                    _mat.hideFlags = _mesh.hideFlags = HideFlags.HideAndDontSave;
                }

                int width = pathfinder.GraphWidth;
                int height = pathfinder.GraphHeight;

                int quadCount = width * height;
                _vertices = new Vector3[4 * quadCount];
                _uv = new Vector2[_vertices.Length];
                _normals = new Vector3[_vertices.Length];
                _tris = new int[6 * quadCount];
                _colors = new Color[_vertices.Length];

                for (int i = 0; i < _normals.Length; i++)
                    _normals[i] = Vector3.back;

                initedData = true;

                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        UpdateNode(x, y);

                RebuildMesh();
            }

            private void RebuildMesh() {
                _mesh.Clear(false);
                _mesh.vertices = _vertices;
                _mesh.uv = _uv;
                _mesh.triangles = _tris;
                _mesh.colors = _colors;
                _mesh.normals = _normals;
            }

            private void UpdateNode(int x, int y, bool updateColor = true, bool updateUV = true, bool updateVertices = true, bool updateTris = true) {
                if (!initedData)
                    InitMeshData();

                CellPosition cell = new CellPosition(x, y);
                int index = x + (y * pathfinder.GraphWidth);

                int vIndex = index * 4;
                int vIndex0 = vIndex;
                int vIndex1 = vIndex + 1;
                int vIndex2 = vIndex + 2;
                int vIndex3 = vIndex + 3;

                if (updateVertices) {
                    Vector3 position = pathfinder.GetCellWorldPosition(cell);
                    position += new Vector3(.5f, .5f) * pathfinder.GraphCellSize;
                    Vector2 baseSize = new Vector2(pathfinder.GraphCellSize, pathfinder.GraphCellSize) * 0.5f;

                    _vertices[vIndex0] = position + (k_vr0 * baseSize);
                    _vertices[vIndex1] = position + (k_vr1 * baseSize);
                    _vertices[vIndex2] = position + (k_vr2 * baseSize);
                    _vertices[vIndex3] = position + (k_vr3 * baseSize);
                }

                if (updateColor) {
                    bool walkable = pathfinder.graph == null || pathfinder.graph.GetNode(cell).walkable;
                    Color color = walkable ? m_WalkableColor : m_UnWalkableColor;
                    color.a = m_NodeTransparency;
                    _colors[vIndex0] = color;
                    _colors[vIndex1] = color;
                    _colors[vIndex2] = color;
                    _colors[vIndex3] = color;
                }

                if (updateUV) {
                    _uv[vIndex0] = new Vector2(0, 0);
                    _uv[vIndex1] = new Vector2(0, 1);
                    _uv[vIndex2] = new Vector2(1, 0);
                    _uv[vIndex3] = new Vector2(1, 1);
                }

                if (updateTris) {
                    int tIndex = index * 6;

                    _tris[tIndex + 0] = vIndex0;
                    _tris[tIndex + 1] = vIndex3;
                    _tris[tIndex + 2] = vIndex1;

                    _tris[tIndex + 3] = vIndex1;
                    _tris[tIndex + 4] = vIndex3;
                    _tris[tIndex + 5] = vIndex2;
                }
            }

            public void UpdateNode(PathNode node) {
                UpdateNode(node.position.x, node.position.y);
                RebuildMesh();
            }

            private void UpdateColors() {
                for (int x = 0; x < pathfinder.GraphWidth; x++)
                    for (int y = 0; y < pathfinder.GraphHeight; y++)
                        UpdateNode(x, y, updateColor: true, updateTris: false, updateUV: false, updateVertices: false);

                _mesh.colors = _colors;
            }

#if UNITY_EDITOR
            public void Validate() {
                if (m_DrawGizmos && initedData)
                    UpdateColors();
            }

            public void DrawGizmos() {
                if (!m_DrawGizmos || !pathfinder.enabled)
                    return;

                if (!initedData)
                    InitMeshData();

                _mat.SetPass(0);
                Graphics.DrawMeshNow(_mesh, Vector3.zero, Quaternion.identity);

                GizmosDrawer gizmos = new GizmosDrawer().SetColor(m_LineColor);

                int width = pathfinder.GraphWidth;
                int height = pathfinder.GraphHeight;
                float cellSize = pathfinder.GraphCellSize;
                Vector2 offset = pathfinder.GraphOffset;

                Vector3 start = pathfinder.GetCellWorldPosition(new CellPosition(0, 0));
                Vector3 end = pathfinder.GetCellWorldPosition(new CellPosition(width, height));

                for (int x = 0; x < width; x++) {
                    float xPos = (x * cellSize) + offset.x;
                    gizmos.DrawLine(new Vector3(xPos, start.y), new Vector3(xPos, end.y));
                }
                for (int y = 0; y < height; y++) {
                    float yPos = (y * cellSize) + offset.y;
                    gizmos.DrawLine(new Vector3(start.x, yPos), new Vector3(end.x, yPos));
                }

                gizmos.DrawLine(new Vector3(start.x, end.y), new Vector3(end.x, end.y));
                gizmos.DrawLine(new Vector3(end.x, start.y), new Vector3(end.x, end.y));
            }
#endif
        }
    }
}