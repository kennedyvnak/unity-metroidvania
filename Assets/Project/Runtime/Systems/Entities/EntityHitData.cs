using UnityEngine;

namespace Metroidvania.Combat
{
    public readonly struct EntityHitData
    {
        public readonly int damage;
        public readonly Vector2 knockbackForce;
        public readonly float invincibility;

        public EntityHitData(int damage, Vector2 knockbackForce, float invincibility)
        {
            this.damage = damage;
            this.knockbackForce = knockbackForce;
            this.invincibility = invincibility;
        }
    }
}