using Metroidvania.Combat;
using Metroidvania.Player;
using UnityEngine;

namespace Metroidvania.Entities.Units
{
    public class UndeadExecutionerBehaviour : EntityObject, IHittableTarget, ITouchHit
    {
        [SerializeField] private int m_touchDamage;
        [SerializeField] private Vector2 m_knockbackForce;

        public void OnTakeHit(PlayerHitData hitData)
        {
            Debug.Log((hitData.damage, hitData.force), gameObject);
        }

        public void OnHitPlayer()
        {
            Debug.Log("player is far...");
        }

        public EntityHitData GetHit(PlayerController playerController)
        {
            var playerPosition = (Vector2)playerController.transform.position;
            var position = (Vector2)transform.position;

            return new EntityHitData(m_touchDamage,
                EntitiesUtility.CalculateKnockback(position, playerPosition, m_knockbackForce),
                playerController.data.defaultInvincibilityTime);
        }

        public abstract class UndeadExecutionerStateBase
        {
            public readonly UndeadExecutionerBehaviour target;

            protected UndeadExecutionerStateBase(UndeadExecutionerBehaviour target)
            {
                this.target = target;
            }

            public virtual void Enter()
            {
            }

            public virtual void LogicUpdate()
            {
            }

            public virtual void Exit()
            {
            }

            public virtual void OnTakeKit(PlayerHitData hitData)
            {
            }
        }

        public class UndeadExecutionerIdleState : UndeadExecutionerStateBase
        {
            public UndeadExecutionerIdleState(UndeadExecutionerBehaviour target) : base(target)
            {
            }
        }
    }
}