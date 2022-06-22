using Metroidvania.Player;

namespace Metroidvania.Combat
{
    /// <summary>Used for handle the player's hits</summary>
    public readonly struct PlayerHitData
    {
        /// <summary>The damage of the attack</summary>
        public readonly int damage;

        /// <summary>The force of the attack, used in knockback</summary>
        public readonly float force;

        /// <summary>The player that attacked</summary>
        public readonly PlayerController player;

        public PlayerHitData(int damage, float force, PlayerController player)
        {
            this.damage = damage;
            this.force = force;
            this.player = player;
        }
    }
}