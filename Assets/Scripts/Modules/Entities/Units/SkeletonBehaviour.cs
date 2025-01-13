using Metroidvania.Combat;
using System;
using UnityEngine;

namespace Metroidvania.Entities.Units
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(EntityTargetFinder), typeof(Rigidbody2D), typeof(Animator))]
    public class SkeletonBehaviour : EntityStateMachine<SkeletonBehaviour>, IHittableTarget
    {
        [System.Serializable]
        private struct AttackData
        {
#if UNITY_EDITOR
            public bool drawGizmos;
#endif
            public float moveOffset;
            public Rect attackCollision;
            public float damage;
            public Vector2 knockbackForce;
        }

        [Header("Properties")]
        [SerializeField] private float m_StartLife;
        [SerializeField] private bool m_StartFacingRight;
        [SerializeField, RangedValue(0, 1)] private RangedFloat m_RngStrength;

        [Header("Collisions")]
        [SerializeField] private Collider2D m_Collider;
        [SerializeField] private Rect m_LeftLedge;
        [SerializeField] private Rect m_RightLedge;
        [SerializeField] private Rect m_LeftHand;
        [SerializeField] private Rect m_RightHand;
        [SerializeField] private LayerMask m_GroundLayer;

        [Header("Follow Target")]
        [SerializeField] private float m_FollowTargetSpeed;
        [UnityEngine.Serialization.FormerlySerializedAs("m_FollowTargetYOffset")]
        [SerializeField] private RangedFloat m_FollowTargetVerticalView;

        [Header("Patrol")]
        [SerializeField, Range(0, 100)] private float m_PatrolChance;
        [SerializeField, RangedValue(0, 30)] private RangedFloat m_PatrolTime;
        [SerializeField] private float m_PatrolMoveSpeed;

        [Header("Attack")]
        [SerializeField] private TouchHitBehaviour m_TouchHitBehaviour;
        [UnityEngine.Serialization.FormerlySerializedAs("m_PlayerLayer")]
        [SerializeField] private LayerMask m_CharactersLayer;
        [SerializeField] private float m_DistanceToAttack;
        [SerializeField] private float m_AttackDuration;
        [SerializeField] private float m_FirstAttackTime;
        [SerializeField] private AttackData m_FirstAttack;
        [SerializeField] private float m_SecondAttackTime;
        [SerializeField] private AttackData m_SecondAttack;

        [Header("Hurt")]
        [SerializeField] private float m_HurtDuration;
        [SerializeField] private Vector2 m_HurtOffset;

        [Header("Death")]
        [SerializeField] private float m_DeathDuration;
        [SerializeField] private float m_DeathFadeDelay;

        public Rigidbody2D rb { get; private set; }
        public EntityTargetFinder targetFinder { get; private set; }
        public Animator anim { get; private set; }
        public SpriteRenderer spriteRenderer { get; private set; }

        private int facingDirection { get; set; }
        private float life { get; set; }
        private float normalizedSpeedFactor { get; set; }

        private bool onLeftLedge { get; set; }
        private bool onRightLedge { get; set; }
        private bool onLeftWall { get; set; }
        private bool onRightWall { get; set; }

        private bool leftObstructed => !onLeftLedge || onLeftWall;
        private bool rightObstructed => !onRightLedge || onRightWall;

        private bool targetObstructed { get; set; }

        private IdleState _idleState;
        private PatrolState _patrolState;
        private FollowTargetState _followTargetState;
        private AttackState _attackState;
        private HurtState _hurtState;
        private DieState _dieState;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            targetFinder = GetComponent<EntityTargetFinder>();
            anim = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            Flip(m_StartFacingRight ? 1 : -1);
            life = m_StartLife;
            normalizedSpeedFactor = m_RngStrength.RandomRange();

            _idleState = new IdleState(this);
            _patrolState = new PatrolState(this);
            _followTargetState = new FollowTargetState(this);
            _attackState = new AttackState(this);
            _hurtState = new HurtState(this);
            _dieState = new DieState(this);

            SwitchState(_idleState);
        }

        private void FixedUpdate()
        {
            onLeftLedge = Physics2D.OverlapBox(rb.position + m_LeftLedge.center, m_LeftLedge.size, 0, m_GroundLayer);
            onRightLedge = Physics2D.OverlapBox(rb.position + m_RightLedge.center, m_RightLedge.size, 0, m_GroundLayer);
            onLeftWall = Physics2D.OverlapBox(rb.position + m_LeftHand.center, m_LeftHand.size, 0, m_GroundLayer);
            onRightWall = Physics2D.OverlapBox(rb.position + m_RightHand.center, m_RightHand.size, 0, m_GroundLayer);

            targetObstructed = targetFinder.hasFocusedTarget && targetFinder.IsObstructed(targetFinder.focusedTarget.position);
        }

        public void OnTakeHit(CharacterHitData hitData)
        {
            life -= hitData.damage;
            if (life > 0)
            {
                _hurtState.lastHitData = hitData;
                SwitchState(_hurtState);
            }
            else
            {
                SwitchState(_dieState);
            }
        }

        private void PlayAnimation(string key)
        {
            anim.Play(key);
        }

        private bool TryEnterAttackState()
        {
            if (targetFinder.hasFocusedTarget)
            {
                Vector2 pos = rb.position;
                Vector2 targetPos = targetFinder.focusedTarget.position;
                if ((pos - targetPos).sqrMagnitude < m_DistanceToAttack * m_DistanceToAttack)
                {
                    SwitchState(_attackState);
                    return true;
                }
            }
            return false;
        }

        private void FlipIfShould(float lookingAt)
        {
            float posX = rb.position.x;
            if ((posX < lookingAt && facingDirection == -1) || (posX > lookingAt && facingDirection == 1))
                Flip(facingDirection * -1);
        }

        private void Flip(int direction)
        {
            facingDirection = direction;
            Vector3 scale = transform.localScale;
            if ((scale.x < 0 && facingDirection == 1) || (scale.x > 0 && facingDirection == -1))
                scale.x *= -1;
            transform.localScale = scale;
        }

        private void LookAtFocusedTarget()
        {
            if (targetFinder.hasFocusedTarget)
                FlipIfShould(targetFinder.focusedTarget.position.x);
        }

        private bool ShouldFollowFocusedTarget() => !targetObstructed && targetFinder.hasFocusedTarget && CanWalkToPosition(targetFinder.focusedTarget.position);

        private bool CanWalkToPosition(Vector2 targetPosition) => CanWalkToDirection(targetPosition.x - rb.position.x) && IsInsideVerticalView(targetPosition.y);

        private bool CanWalkToDirection(float direction)
        {
            bool leftObsolete = direction < 0 && leftObstructed;
            bool rightObsolete = direction > 0 && rightObstructed;
            return !leftObsolete && !rightObsolete;
        }

        private bool IsInsideVerticalView(float position) => m_FollowTargetVerticalView.Contains(position - rb.position.y);

#if UNITY_EDITOR
        private void OnValidate()
        {
            Flip(m_StartFacingRight ? 1 : -1);
        }

        private void OnDrawGizmosSelected()
        {
            GizmosDrawer gizmos = new GizmosDrawer();

            gizmos.SetColor(Color.yellow);

            DrawAttack(m_FirstAttack);
            DrawAttack(m_SecondAttack);

            gizmos.SetColor(Color.red).DrawWireDisc(transform.position, m_DistanceToAttack);

            gizmos.SetColor(Color.blue)
                .DrawWireDisc(transform.position + new Vector3(0, m_FollowTargetVerticalView.min), 0.25f)
                .DrawWireDisc(transform.position + new Vector3(0, m_FollowTargetVerticalView.max), 0.25f);

            gizmos.SetColor(Color.green)
                .DrawWireSquare(m_LeftLedge.center + (Vector2)transform.position, m_LeftLedge.size)
                .DrawWireSquare(m_RightLedge.center + (Vector2)transform.position, m_RightLedge.size)
                .DrawWireSquare(m_LeftHand.center + (Vector2)transform.position, m_LeftHand.size)
                .DrawWireSquare(m_RightHand.center + (Vector2)transform.position, m_RightHand.size);

            void DrawAttack(AttackData attack)
            {
                if (attack.drawGizmos)
                    gizmos.DrawWireSquare((Vector2)transform.position + (attack.attackCollision.center * transform.localScale), attack.attackCollision.size);
            }
        }
#endif

        public abstract class StateBase : EntityBehaviourState<SkeletonBehaviour>
        {
            protected StateBase(SkeletonBehaviour entity) : base(entity)
            {
            }
        }

        public class IdleState : StateBase
        {
            private float currentTick { get; set; }

            public IdleState(SkeletonBehaviour entity) : base(entity)
            {
            }

            public override void Enter()
            {
                entity.rb.linearVelocity = Vector2.zero;
                currentTick = 0;
                entity.PlayAnimation("Idle");
            }

            public override void LogicUpdate()
            {
                if (entity.ShouldFollowFocusedTarget())
                {
                    entity.SwitchState(entity._followTargetState);
                    return;
                }

                currentTick += Time.deltaTime;
                if (currentTick >= 1)
                {
                    if (UnityEngine.Random.Range(0.0f, 100.0f) <= entity.m_PatrolChance)
                        entity.SwitchState(entity._patrolState);
                    currentTick -= 1;
                }
            }
        }

        public class PatrolState : StateBase
        {
            private float elapsedTime { get; set; }
            private float walkTime { get; set; }

            public PatrolState(SkeletonBehaviour entity) : base(entity)
            {
            }

            public override void Enter()
            {
                walkTime = entity.m_PatrolTime.RandomRange();
                elapsedTime = 0;
                entity.PlayAnimation("Walk");
            }

            public override void LogicUpdate()
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime >= walkTime)
                {
                    entity.SwitchState(entity._idleState);
                    return;
                }

                if (entity.ShouldFollowFocusedTarget())
                {
                    entity.SwitchState(entity._followTargetState);
                    return;
                }

                if (!entity.CanWalkToDirection(entity.facingDirection))
                    entity.Flip(entity.facingDirection * -1);

                entity.rb.linearVelocity = new Vector2(entity.facingDirection * entity.m_PatrolMoveSpeed * entity.normalizedSpeedFactor, entity.rb.linearVelocity.y);
            }
        }

        public class FollowTargetState : StateBase
        {
            public FollowTargetState(SkeletonBehaviour entity) : base(entity)
            {
            }

            public override void Enter()
            {
                entity.targetFinder.LockFocusedTarget();
                entity.PlayAnimation("Walk");
            }

            public override void LogicUpdate()
            {
                Vector2 entityPosition = entity.rb.position;
                Vector2 focusedTargetPosition = entity.targetFinder.focusedTarget.position;

                entity.FlipIfShould(focusedTargetPosition.x);

                float direction = Mathf.Sign(focusedTargetPosition.x - entityPosition.x);

                if (!entity.ShouldFollowFocusedTarget())
                {
                    entity.SwitchState(entity._idleState);
                    return;
                }

                if (entity.TryEnterAttackState())
                    return;

                entity.rb.linearVelocity = new Vector2(entity.m_FollowTargetSpeed * direction * entity.normalizedSpeedFactor, entity.rb.linearVelocity.y);
            }

            public override void Exit()
            {
                entity.targetFinder.UnlockFocusedTarget();
                entity.targetFinder.UpdateVisibleTargets();
            }
        }

        public class AttackState : StateBase
        {
            private Collider2D[] _hits = new Collider2D[8];
            private float _elapsedTime;
            private bool _performedFirstAttack;
            private bool _performedSecondAttack;

            public AttackState(SkeletonBehaviour entity) : base(entity)
            {
            }

            public override void Enter()
            {
                entity.rb.linearVelocity = Vector2.zero;
                _performedFirstAttack = false;
                _performedSecondAttack = false;
                _elapsedTime = 0;
                entity.LookAtFocusedTarget();
                entity.PlayAnimation("Attack");
            }

            public override void LogicUpdate()
            {
                _elapsedTime += Time.deltaTime;

                if (!_performedFirstAttack && _elapsedTime >= entity.m_FirstAttackTime)
                {
                    PerformAttack(entity.m_FirstAttack);
                    _performedFirstAttack = true;
                }

                if (!_performedSecondAttack && _elapsedTime >= entity.m_SecondAttackTime)
                {
                    PerformAttack(entity.m_SecondAttack);
                    _performedSecondAttack = true;
                }

                if (_elapsedTime >= entity.m_AttackDuration)
                    entity.SwitchState(entity._idleState);
            }

            private void PerformAttack(AttackData attackData)
            {
                entity.LookAtFocusedTarget();

                float hitDistance = attackData.moveOffset * entity.m_RngStrength.RandomRange();
                RaycastHit2D hit = Physics2D.Raycast(entity.rb.position, new Vector2(entity.facingDirection, 0).normalized, hitDistance, entity.m_GroundLayer);
                if (hit)
                    entity.rb.MovePosition(new Vector2(hit.point.x - (0.5f * entity.facingDirection * entity.m_Collider.bounds.size.x), hit.point.y));
                else
                    entity.rb.MovePosition(new Vector2(entity.rb.position.x + (hitDistance * entity.facingDirection), entity.rb.position.y));

                CombatUtility.CastEntityBoxHit(entity.rb.position + (attackData.attackCollision.center * entity.transform.localScale),
                    attackData.attackCollision.size, _hits, entity.m_CharactersLayer, attackData.damage,
                    CombatUtility.FromFacingDirection(attackData.knockbackForce, entity.facingDirection));
            }
        }

        public class HurtState : StateBase
        {
            private float elapsedTime { get; set; }

            public CharacterHitData lastHitData;

            public HurtState(SkeletonBehaviour entity) : base(entity)
            {
            }

            public override void Enter()
            {
                elapsedTime = 0;
                entity.rb.linearVelocity = Vector2.zero;
                entity.rb.AddForce(GetAttackForce(), ForceMode2D.Impulse);
                entity.PlayAnimation("Hurt");
            }

            public override void LogicUpdate()
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= entity.m_HurtDuration)
                    entity.SwitchState(entity._idleState);
            }

            private Vector2 GetAttackForce()
            {
                return new Vector2(entity.m_HurtOffset.x * lastHitData.character.facingDirection, entity.m_HurtOffset.y) * lastHitData.force * entity.m_RngStrength.RandomRange();
            }
        }

        public class DieState : StateBase
        {
            private float elapsedTime { get; set; }

            public DieState(SkeletonBehaviour entity) : base(entity)
            {
            }

            public override void Enter()
            {
                elapsedTime = 0;
                entity.rb.linearVelocity = Vector2.zero;
                entity.m_TouchHitBehaviour.enabled = false;
                entity.PlayAnimation("Die");
            }

            public override void LogicUpdate()
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime >= entity.m_DeathFadeDelay)
                {
                    float normalizedElapsedTime = (elapsedTime - entity.m_DeathFadeDelay) / (entity.m_DeathDuration - entity.m_DeathFadeDelay);
                    Color c = entity.spriteRenderer.color;
                    c.a = 1 - normalizedElapsedTime;
                    entity.spriteRenderer.color = c;
                }

                if (elapsedTime >= entity.m_DeathDuration)
                    UnityEngine.Object.Destroy(entity.gameObject);
            }
        }
    }
}
