using DG.Tweening;
using Metroidvania.Animations;
using Metroidvania.Combat;
using Metroidvania.Entities;
using Metroidvania.InputSystem;
using Metroidvania.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Metroidvania.Characters.Knight
{
    public class KnightCharacterController : CharacterBase, ISceneTransistor, IEntityHittable
    {
        public static readonly int IdleAnimHash = Animator.StringToHash("Idle");
        public static readonly int RunAnimHash = Animator.StringToHash("Run");

        public static readonly int JumpAnimHash = Animator.StringToHash("Jump");
        public static readonly int FallAnimHash = Animator.StringToHash("Fall");

        public static readonly int RollAnimHash = Animator.StringToHash("Roll");

        public static readonly int SlideAnimHash = Animator.StringToHash("Slide");
        public static readonly int SlideEndAnimHash = Animator.StringToHash("SlideEnd");

        public static readonly int WallslideAnimHash = Animator.StringToHash("Wallslide");

        public static readonly int CrouchIdleAnimHash = Animator.StringToHash("CrouchIdle");
        public static readonly int CrouchWalkAnimHash = Animator.StringToHash("CrouchWalk");
        public static readonly int CrouchTransitionAnimHash = Animator.StringToHash("CrouchTransition");
        public static readonly int CrouchAttackAnimHash = Animator.StringToHash("CrouchAttack");

        public static readonly int FirstAttackAnimHash = Animator.StringToHash("FirstAttack");
        public static readonly int SecondAttackAnimHash = Animator.StringToHash("SecondAttack");

        public static readonly int HurtAnimHash = Animator.StringToHash("Hurt");
        public static readonly int DieAnimHash = Animator.StringToHash("Die");

#if UNITY_EDITOR
        [SerializeField] private bool m_DrawGizmos;
#endif

        [SerializeField] private KnightData m_Data;
        public KnightData data => m_Data;

        [SerializeField] private Particles m_Particles;
        public Particles particles => m_Particles;

        [SerializeField] private GameObject m_gfxGameObject;

        public Rigidbody2D rb { get; private set; }

        private SpriteSheetAnimator _animator;
        private BoxCollider2D _collider;
        private SpriteRenderer _renderer;

        private int currentAnimationHash { get; set; }

        public float horizontalMove { get; private set; }

        public bool canStand { get; private set; }

        private KnightData.ColliderBounds colliderBoundsSource { get; set; }
        private Collider2D[] attackHits { get; set; }

        private int _invincibilityCount;
        private int _invincibilityAnimationsCount;
        private Coroutine _invincibilityAnimationCoroutine;

        public bool isInvincible => _invincibilityCount > 0 || stateMachine.currentState.isInvincible;
        public bool isDied => stateMachine.currentState is KnightDieState;

        public readonly CollisionChecker collisionChecker = new CollisionChecker();

        public KnightStateMachine stateMachine { get; private set; }

        public CharacterAttribute<float> lifeAttribute { get; private set; }

        public InputAction crouchAction => InputReader.instance.inputActions.Gameplay.Crouch;
        public InputAction dashAction => InputReader.instance.inputActions.Gameplay.Dash;
        public InputAction attackAction => InputReader.instance.inputActions.Gameplay.Attack;
        public InputAction jumpAction => InputReader.instance.inputActions.Gameplay.Jump;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _animator = m_gfxGameObject.GetComponent<SpriteSheetAnimator>();
            _renderer = m_gfxGameObject.GetComponent<SpriteRenderer>();

            facingDirection = 1;

            lifeAttribute = new CharacterAttribute<float>(data.lifeAttributeData, at => at.data.startValue + at.currentLevel * at.data.stepPerLevel);

            attackHits = new Collider2D[8];
            stateMachine = new KnightStateMachine(this);
        }

        private void Start()
        {
            CharacterStatusBar.instance.ConnectLife(lifeAttribute);
            CharacterStatusBar.instance.SetLife(lifeAttribute.currentValue);
        }

        private void OnEnable()
        {
            InputReader.instance.MoveEvent += ReadMoveInput;
            InputReader.instance.JumpEvent += HandleJump;
            InputReader.instance.DashEvent += HandleDash;
            InputReader.instance.AttackEvent += HandleAttack;
        }

        private void OnDisable()
        {
            InputReader.instance.MoveEvent -= ReadMoveInput;
            InputReader.instance.JumpEvent -= HandleJump;
            InputReader.instance.DashEvent -= HandleDash;
            InputReader.instance.AttackEvent -= HandleAttack;
        }

        private void Update()
        {
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            collisionChecker.EvaluateCollisions();
            Vector2 charPosition = transform.position;
            Vector2 boundsPosition = data.crouchHeadRect.position * transform.localScale;
            canStand = !Physics2D.OverlapBox(charPosition + boundsPosition, data.crouchHeadRect.size, 0, data.groundLayer);
            stateMachine.PhysicsUpdate();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.TryGetComponent<ITouchHit>(out ITouchHit touchHit) || (!touchHit.ignoreInvincibility && isInvincible))
                return;
            OnTakeHit(touchHit.OnHitCharacter(this));
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            collisionChecker.CollisionEnter(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            collisionChecker.CollisionStay(other);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            collisionChecker.CollisionExit(other);
        }

        public void SwitchAnimation(int animationHash, bool force = false)
        {
            if (!force && currentAnimationHash == animationHash)
                return;

            _animator.SetSheet(animationHash);
            currentAnimationHash = animationHash;
        }

        public void FlipFacingDirection(float velocityX)
        {
            if ((velocityX < 0 && facingDirection == 1) || (velocityX > 0 && facingDirection == -1))
                Flip();
        }

        public void SetColliderBounds(KnightData.ColliderBounds colliderBounds)
        {
            colliderBoundsSource = colliderBounds;
            _collider.offset = colliderBounds.bounds.min;
            _collider.size = colliderBounds.bounds.size;
        }

        public void AddInvincibility(float time, bool shouldAnim)
        {
            StartCoroutine(StartInvincibility(time, shouldAnim));
        }

        private IEnumerator StartInvincibility(float time, bool shouldAnim)
        {
            if (shouldAnim)
                _invincibilityAnimationsCount++;
            _invincibilityCount++;

            if (_invincibilityAnimationsCount > 0 && _invincibilityAnimationCoroutine == null)
                _invincibilityAnimationCoroutine = StartCoroutine(StartInvincibilityAnimation());

            yield return Helpers.GetYieldSeconds(time);

            _invincibilityCount--;
            if (shouldAnim)
                _invincibilityAnimationsCount--;
        }

        private IEnumerator StartInvincibilityAnimation()
        {
            float elapsedTime = 0;
            while (_invincibilityAnimationsCount > 0)
            {
                elapsedTime += Time.deltaTime * data.invincibilityFadeSpeed;
                _renderer.SetAlpha(1 - Mathf.PingPong(elapsedTime, data.invincibilityAlphaChange));
                yield return null;
            }

            _renderer.SetAlpha(1);
            _invincibilityAnimationCoroutine = null;
        }

        public void PerformAttack(KnightData.Attack attackData)
        {
            rb.Slide(new Vector2(attackData.horizontalMoveOffset * facingDirection, 0), 1, data.slideMovement);

            var contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(data.hittableLayer);
            int hitCount = Physics2D.OverlapBox(rb.position + (attackData.triggerCollider.center * transform.localScale), attackData.triggerCollider.size, 0, contactFilter, attackHits);

            if (hitCount <= 0)
                return;

            CharacterHitData hitData = new CharacterHitData(attackData.damage, attackData.force, this);
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = attackHits[i];
                if (hit.TryGetComponent<IHittableTarget>(out IHittableTarget hittableTarget))
                    hittableTarget.OnTakeHit(hitData);
            }
        }

        public void TryDropPlatform()
        {
            foreach (var collision in collisionChecker.collisions)
                if (collision.Key.usedByEffector && collision.Key.TryGetComponent(out PlatformEffector2D _))
                    DropPlatform(collision.Key);
        }

        public void DropPlatform(Collider2D platform)
        {
            Physics2D.IgnoreCollision(_collider, platform);
            DOVirtual.DelayedCall(.25f, () => Physics2D.IgnoreCollision(_collider, platform, false));
        }

        private void ReadMoveInput(float move) => horizontalMove = move;

        private void HandleJump()
        {
            stateMachine.currentState.HandleJump();
        }

        private void HandleDash()
        {
            stateMachine.currentState.HandleDash();
        }

        private void HandleAttack()
        {
            stateMachine.currentState.HandleAttack();
        }

        public override void OnTakeHit(EntityHitData hitData)
        {
            if (isInvincible || isDied)
                return;

            lifeAttribute.currentValue -= hitData.damage;
            data.onHurtChannel.Raise(this, hitData);

            if (lifeAttribute.currentValue <= 0)
                stateMachine.EnterState(stateMachine.dieState);
            else
            {
                AddInvincibility(data.defaultInvincibilityTime, true);
                stateMachine.hurtState.EnterHurtState(hitData);
            }
        }

        public override void OnSceneTransition(SceneLoader.SceneTransitionData transitionData)
        {
            CharacterSpawnPoint spawnPoint = GetSceneSpawnPoint(transitionData);

            transform.position = spawnPoint.position;
            FlipTo(spawnPoint.facingToRight ? 1 : -1);

            FocusCameraOnThis();


            if (transitionData.gameData.ch_knight_died)
            {
                transitionData.gameData.ch_knight_died = false;
                lifeAttribute.currentValue = transitionData.gameData.ch_knight_life;
            }
            else
            {
                lifeAttribute.currentValue = transitionData.gameData.ch_knight_life;
            }
            CharacterStatusBar.instance.SetLife(lifeAttribute.currentValue);

            if (spawnPoint.isHorizontalDoor)
                stateMachine.fakeWalkState.EnterFakeWalk(data.fakeWalkOnSceneTransitionTime);
        }

        public override void BeforeUnload(SceneLoader.SceneUnloadData unloadData)
        {
            unloadData.gameData.ch_knight_life = lifeAttribute.currentValue;
            unloadData.gameData.ch_knight_died = isDied;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!m_DrawGizmos || !data)
                return;

            Transform t = transform;
            Vector2 position = (Vector2)t.position;
            Vector2 scale = (Vector2)t.localScale;

            GizmosDrawer drawer = new GizmosDrawer();

            drawer.SetColor(GizmosColor.instance.knight.attack);
            DrawAttack(data.firstAttack);
            DrawAttack(data.secondAttack);
            DrawAttack(data.crouchAttack);

            if (data.crouchColliderBounds.drawGizmos)
                drawer.SetColor(GizmosColor.instance.knight.feet)
                    .DrawWireSquare(position + (data.crouchHeadRect.min * scale), data.crouchHeadRect.size);

            void DrawAttack(KnightData.Attack attack)
            {
                if (!attack.drawGizmos)
                    return;

                drawer.DrawWireSquare(position + (attack.triggerCollider.center * scale), attack.triggerCollider.size);
            }
        }
#endif

        [System.Serializable]
        public class Particles
        {
            public ParticleSystem jump;
            public ParticleSystem wallslide;
            public ParticleSystem walljump;
            public ParticleSystem landing;
            public ParticleSystem slide;
        }
    }
}
