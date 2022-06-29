#if UNITY_EDITOR
using UnityEngine;

namespace Metroidvania
{
    /// <summary>Object for handle all gizmos color used in game</summary>
    public class GizmosColor : ScriptableSingleton<GizmosColor>
    {
        [Header("Player")]
        public Color playerFeet = Color.green;
        public Color playerHand = Color.yellow;
        public Color playerAttack = Color.cyan;
        public Color playerColliderData = Color.green;

        [Header("Safe Player Points Area")]
        public Color safePointArea = Color.green;

        [Header("Entities")]
        public Color entityTargetFinderFindRange = Color.yellow;
        public Color entityTargetFinderMaxRange = Color.red;
    }
}
#endif