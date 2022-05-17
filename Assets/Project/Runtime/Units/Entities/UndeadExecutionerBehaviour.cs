using Metroidvania.Combat;
using Metroidvania.Player;
using UnityEngine;

namespace Metroidvania.Entities.Units
{
    public class UndeadExecutionerBehaviour : EntityBehaviour, IHittableTarget, ITouchHit
    {
        [SerializeField] private int m_touchDamage;
        [SerializeField] private Vector2 m_knockbackForce;

        public bool ignoreInvincibility => false;

        public void OnTakeHit(PlayerHitData hitData)
        {
            // TODO: Implement this method
            Debug.Log((hitData.damage, hitData.force), gameObject);
        }

        public EntityHitData OnHitPlayer(PlayerController playerController)
        {
            return new EntityHitData(m_touchDamage,
                new Vector2(m_knockbackForce.x * playerController.facingDirection, m_knockbackForce.y));
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