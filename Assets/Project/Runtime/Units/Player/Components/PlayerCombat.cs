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
            target.TriggerEntered += OnTriggerEnter;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            target.TriggerEntered -= OnTriggerEnter;
        }
        
        /// <summary>Called when a trigger enter in the player collider</summary>
        private void OnTriggerEnter(Collider2D col)
        {
            if (!col.TryGetComponent<ITouchHit>(out var touchHit)) return;
            TakeHit(touchHit.OnHitPlayer(target));
        }
        
        /// <summary>Call this to hit the player</summary>
        /// <param name="entityHit">A hit data</param>
        public void TakeHit(EntityHitData entityHit)
        {
            target.life -= entityHit.damage;
            target.invincibility.AddInvincibility(target.data.defaultInvincibilityTime);

            if (target.life <= 0)
                target.stateMachine.deathState.SetActive();
            else
                target.stateMachine.EnterHurt(entityHit.knockbackForce);
        }
    }
}