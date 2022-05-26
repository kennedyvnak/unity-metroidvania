using Metroidvania.Combat;
using Metroidvania.Entities;
using Metroidvania.Player.States;
using UnityEngine;

namespace Metroidvania.Player
{
    /// <summary>Player component for handle combat</summary>
    public class PlayerCombat : PlayerComponent
    {
        public PlayerCombat(PlayerController player) : base(player)
        {
            player.TriggerStay += TriggerStay;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            player.TriggerStay -= TriggerStay;
        }

        /// <summary>Called when a trigger stay in the player collider</summary>
        private void TriggerStay(Collider2D col)
        {
            if (!col.TryGetComponent<ITouchHit>(out var touchHit) || (!touchHit.ignoreInvincibility && player.invincibility.isInvincible)) return;
            TakeHit(touchHit.OnHitPlayer(player));
        }

        /// <summary>Call this to hit the player</summary>
        /// <param name="entityHit">A hit data</param>
        public void TakeHit(EntityHitData entityHit)
        {
            player.life -= entityHit.damage;
            player.invincibility.AddInvincibility(player.data.defaultInvincibilityTime, true);

            if (player.life <= 0)
                player.stateMachine.deathState.SetActive();
            else
                player.stateMachine.EnterHurt(entityHit.knockbackForce);
        }
    }
}