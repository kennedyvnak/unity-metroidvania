namespace Metroidvania.Pathfinding {
    public struct PathNodeReference {
        public CellPosition position;
        public int index;

        public int g, h;
        public int f => g + h;

        public int cameFromNodeIndex;

        public bool walkable;

        public PathNodeReference(CellPosition position, bool walkable, int index) {
            this.position = position;
            this.walkable = walkable;
            this.index = index;
            g = h = cameFromNodeIndex = -1;
        }
    }
}