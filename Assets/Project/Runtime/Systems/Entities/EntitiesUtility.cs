using UnityEngine;

namespace Metroidvania.Combat
{
    public static class EntitiesUtility
    {
        public static Vector2 CalculateKnockback(Vector2 from, Vector2 to, Vector2 force)
        {
            return (to - from).normalized * force;
        }
    }
}