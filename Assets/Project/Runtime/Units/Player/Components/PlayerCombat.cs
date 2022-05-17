using Metroidvania.Combat;
using Metroidvania.Entities;
using Metroidvania.Player.States;
using UnityEngine;

namespace Metroidvania.Player
{
    /// <summary>Player component for handle combat</summary>
    public class PlayerCombat : PlayerComponent
    {
        public PlayerCombat(PlayerController target) : base(target)
        {
            target.TriggerStay += TriggerStay;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            target.TriggerStay -= TriggerStay;
        }

        /// <summary>Called when a trigger stay in the player collider</summary>
        private void TriggerStay(Collider2D col)
        {
            if (!col.TryGetComponent<ITouchHit>(out var touchHit) || (!touchHit.ignoreInvincibility && target.invincibility.isInvincible)) return;
            TakeHit(touchHit.OnHitPlayer(target));
        }

        /// <summary>Call this to hit the player</summary>
        /// <param name="entityHit">A hit data</param>
        public void TakeHit(EntityHitData entityHit)
        {
            target.life -= entityHit.damage;
            target.invincibility.AddInvincibility(target.data.defaultInvincibilityTime, true);

            if (target.life <= 0)
                target.stateMachine.deathState.SetActive();
            else
                target.stateMachine.EnterHurt(entityHit.knockbackForce);
        }
    }
}