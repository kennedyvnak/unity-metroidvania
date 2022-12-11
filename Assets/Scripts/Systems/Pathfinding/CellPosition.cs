using System;

namespace Metroidvania.Pathfinding {
    /// <summary>
    /// </summary>
    [Serializable]
    public struct CellPosition : IEquatable<CellPosition>, IFormattable {
        public int x, y;

        public CellPosition(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object o) => o is CellPosition other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(x, y);
        public bool Equals(CellPosition other) => other.x == x && other.y == y;

        public override string ToString() => $"({x}, {y})";
        public string ToString(string format) => $"({x.ToString(format)}, {y.ToString(format)})";
        public string ToString(string format, IFormatProvider formatProvider) => $"({x.ToString(format, formatProvider)}, {y.ToString(format, formatProvider)})";

        public static CellPosition operator +(CellPosition lhs, CellPosition rhs) => new CellPosition(lhs.x + rhs.x, lhs.y + rhs.y);
        public static CellPosition operator -(CellPosition lhs, CellPosition rhs) => new CellPosition(lhs.x - rhs.x, lhs.y - rhs.y);
        public static CellPosition operator *(CellPosition lhs, CellPosition rhs) => new CellPosition(lhs.x * rhs.x, lhs.y * rhs.y);
        public static CellPosition operator /(CellPosition lhs, CellPosition rhs) => new CellPosition(lhs.x / rhs.x, lhs.y / rhs.y);

        public static bool operator ==(CellPosition lhs, CellPosition rhs) => lhs.Equals(rhs);
        public static bool operator !=(CellPosition lhs, CellPosition rhs) => !lhs.Equals(rhs);
    }
}