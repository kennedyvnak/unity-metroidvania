using UnityEngine;

namespace Metroidvania.Entities
{
    public readonly struct EntityHitData
    {
        /// <summary>The damage to be applied in the player life</summary>
        public readonly float damage;

        /// <summary>The force to be applied in the player velocity</summary>
        public readonly Vector2 knockbackForce;

        public EntityHitData(float damage, Vector2 knockbackForce)
        {
            this.damage = damage;
            this.knockbackForce = knockbackForce;
        }
    }
}