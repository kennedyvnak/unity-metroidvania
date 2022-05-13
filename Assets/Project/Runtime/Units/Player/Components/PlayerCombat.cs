using Metroidvania.Combat;
using Metroidvania.Player.States;
using UnityEngine;

namespace Metroidvania.Player
{
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

        private void OnTriggerEnter(Collider2D col)
        {
            if (!col.TryGetComponent<ITouchHit>(out var touchHit)) return;
            TakeHit(touchHit.GetHit(target));
            touchHit.OnHitPlayer();
        }

        private void TakeHit(EntityHitData entityHit)
        {
            target.life -= entityHit.damage;
            target.invincibility.AddInvincibility(entityHit.invincibility);

            if (target.life <= 0)
                target.stateMachine.deathState.SetActive();
            else
                target.stateMachine.EnterHurt(entityHit.knockbackForce);
        }
    }
}