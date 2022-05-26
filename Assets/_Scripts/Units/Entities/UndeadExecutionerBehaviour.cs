using Metroidvania.Combat;
using Metroidvania.Player;
using UnityEngine;

namespace Metroidvania.Entities.Units
{
    public class UndeadExecutionerBehaviour : EntityStateMachine<UndeadExecutionerBehaviour>, IHittableTarget,
        ITouchHit
    {
        [SerializeField] private int m_touchDamage;
        [SerializeField] private Vector2 m_knockbackForce;

        public bool ignoreInvincibility => false;

        private IdleState _idleState;
        private HurtState _hurtState;

        private void Awake()
        {
            _idleState = new IdleState(this);
            _hurtState = new HurtState(this);

            SwitchState(_idleState);
        }

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

        private void EnterHurt(PlayerHitData hitData)
        {
            _hurtState.hitData = hitData;
            SwitchState(_hurtState);
        }

        public abstract class BaseState : EntityBehaviourState<UndeadExecutionerBehaviour>
        {
            protected BaseState(UndeadExecutionerBehaviour entity) : base(entity)
            {
            }

            public virtual void OnTakeKit(PlayerHitData hitData)
            {
                entity.EnterHurt(hitData);
            }
        }

        public class IdleState : BaseState
        {
            public IdleState(UndeadExecutionerBehaviour target) : base(target)
            {
            }
        }

        public class HurtState : BaseState
        {
            public PlayerHitData hitData { get; set; }

            public HurtState(UndeadExecutionerBehaviour target) : base(target)
            {
            }
        }
    }
}