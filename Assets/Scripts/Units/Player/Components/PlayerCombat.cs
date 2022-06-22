using Metroidvania.Combat;
using Metroidvania.Entities;
using Metroidvania.Player.States;
using UnityEngine;

namespace Metroidvania.Player
{
    /// <summary>Player component for handle combat</summary>
    public class PlayerCombat : PlayerComponent
    {
        /// <summary>Colliders hit on last trigger. Used for allocate hits array only once</summary>
        private readonly Collider2D[] _hits = new Collider2D[8];

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
            if (!col.TryGetComponent<ITouchHit>(out ITouchHit touchHit) || (!touchHit.ignoreInvincibility && player.invincibility.isInvincible)) return;
            TakeHit(touchHit.OnHitPlayer(player));
        }

        /// <summary>Call this to hit the player</summary>
        /// <param name="entityHit">A hit data</param>
        public void TakeHit(EntityHitData entityHit)
        {
            player.data.lifeField.value -= entityHit.damage;
            player.invincibility.AddInvincibility(player.data.defaultInvincibilityTime, true);

            if (player.data.lifeField.value <= 0)
                player.stateMachine.deathState.SetActive();
            else
            {
                player.stateMachine.EnterHurt(entityHit.knockbackForce);
                player.data.onHurtChannel?.Raise(player, entityHit);
            }
        }

        public void PerformAttack(PlayerDataChannel.Attack attackData)
        {
            // Moves the player's position by the offset defined in the attack data
            player.rb.MovePosition(player.rb.position +
                new Vector2(attackData.horizontalMoveOffset * player.facingDirection, 0));

            // Get the colliders in the attack rect without allocate a new array in the memory
            int hitCount = Physics2D.OverlapBoxNonAlloc(
                player.rb.position + (attackData.triggerCollider.center * player.transform.localScale),
                attackData.triggerCollider.size, 0, _hits, player.data.hittableLayer);

            // Do nothing if don't hit any object
            if (hitCount <= 0) return;

            PlayerHitData hitData = new PlayerHitData(attackData.damage, attackData.force, player);
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = _hits[i];
                // If the hit contains an IHittableTarget component, it will call the OnTakeHit method.  
                if (hit.TryGetComponent<IHittableTarget>(out IHittableTarget hittableTarget))
                    hittableTarget.OnTakeHit(hitData);
            }
        }
    }
}