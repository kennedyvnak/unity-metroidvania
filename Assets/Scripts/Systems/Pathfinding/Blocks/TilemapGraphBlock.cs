using UnityEngine;
using UnityEngine.Tilemaps;

namespace Metroidvania.Pathfinding.Blocks {
    public class TilemapGraphBlock : GraphBlockBase {
        [SerializeField] private Tilemap m_Tilemap;

        public override bool IsBlocked(PathNode node) {
            return m_Tilemap.GetTile(m_Tilemap.WorldToCell(node.worldPosition));
        }
    }
}