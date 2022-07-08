using Metroidvania.Combat;
using Metroidvania.Pathfinding;
using Metroidvania.Player;
using UnityEngine;

namespace Metroidvania.Entities.Units
{
    [RequireComponent(typeof(EntityTargetFinder), typeof(Rigidbody2D), typeof(Animator))]
    public class UndeadExecutionerBehaviour : EntityStateMachine<UndeadExecutionerBehaviour>, IHittableTarget, ITouchHit
    {
        public Rigidbody2D rb { get; private set; }
        public EntityTargetFinder targetFinder { get; private set; }
        public Animator anim { get; private set; }

        [SerializeField] private float m_touchDamage;
        [SerializeField] private Vector2 m_knockbackForce;

        [SerializeField] private float m_foundRate = 1;

        public bool ignoreInvincibility => false;

        private IdleState _idleState;
        private HurtState _hurtState;

        private Path _currentPath;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            targetFinder = GetComponent<EntityTargetFinder>();
            anim = GetComponent<Animator>();

            _idleState = new IdleState(this);
            _hurtState = new HurtState(this);

            SwitchState(_idleState);
        }

        public void OnTakeHit(PlayerHitData hitData)
        {
            // TODO: Implement this method
            if (GameDebugger.instance.enableEntitiesLogs)
                GameDebugger.Log((hitData.damage, hitData.force), gameObject);
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

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_currentPath != null)
                new GizmosDrawer().SetColor(GizmosColor.instance.pathfinding.pathColor).DrawPath(_currentPath);
        }
#endif

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
            private float lastFoundTime { get; set; } = int.MaxValue;

            public IdleState(UndeadExecutionerBehaviour target) : base(target)
            {
            }

            public override void Enter()
            {
            }

            public override void LogicUpdate()
            {
                if (entity.targetFinder.hasFocusedTarget)
                {
                    lastFoundTime += Time.deltaTime;

                    if (lastFoundTime > entity.m_foundRate)
                    {
                        lastFoundTime = 0;
                        if (entity._currentPath != null)
                            Pathfinder.instance.ReleasePath(ref entity._currentPath);
                        entity._currentPath = Pathfinder.instance.FindPath(entity.targetFinder.position, entity.targetFinder.focusedTarget.position);
                    }
                }
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