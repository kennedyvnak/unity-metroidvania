using System.Collections;
using Metroidvania.Animations;
using Metroidvania.Combat;
using Metroidvania.Entities;
using Metroidvania.InputSystem;
using Metroidvania.SceneManagement;
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

        public bool isGrounded { get; private set; }
        public bool canStand { get; private set; }
        public bool isTouchingWall { get; private set; }

        private KnightData.ColliderBounds colliderBoundsSource { get; set; }
        private Collider2D[] attackHits { get; set; }

        private int _invincibilityCount;
        private int _invincibilityAnimationsCount;
        private Coroutine _invincibilityAnimationCoroutine;

        public bool isInvincible => _invincibilityCount > 0 || stateMachine.inInvincibleState;
        public bool isDied => stateMachine.currentState is KnightDieState;

        public KnightStateMachine stateMachine { get; private set; }

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

            attackHits = new Collider2D[8];
            stateMachine = new KnightStateMachine(this);
        }

        private void Update()
        {
            stateMachine.currentState.Update();
        }

        private void OnEnable()
        {
            InputReader.instance.MoveEvent += ReadMoveInput;

        }

        private void OnDisable()
        {
            InputReader.instance.MoveEvent -= ReadMoveInput;
        }

        private void FixedUpdate()
        {
            CollisionsCheck();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.TryGetComponent<ITouchHit>(out ITouchHit touchHit) || (!touchHit.ignoreInvincibility && isInvincible))
                return;
            OnTakeHit(touchHit.OnHitCharacter(this));
        }

        public override void OnTakeHit(EntityHitData hitData)
        {
            if (isInvincible || isDied)
                return;

            life.currentLife -= hitData.damage;
            data.onHurtChannel.Raise(this, hitData);

            if (life.currentLife <= 0)
                stateMachine.EnterState(stateMachine.dieState);
            else
            {
                AddInvincibility(data.defaultInvincibilityTime, true);
                stateMachine.EnterHurt(hitData);
            }
        }

        public void SwitchAnimation(int animationHash, bool force = false)
        {
            if (!force && currentAnimationHash == animationHash)
                return;

            _animator.SetSheet(animationHash);
            currentAnimationHash = animationHash;
        }

        public void SetHorizontalVelocity(float velocity, bool doFlipCheck = true)
        {
            rb.velocity = new Vector2(velocity, rb.velocity.y);
            if (doFlipCheck)
                FlipByVelocity();
        }

        public void FlipByVelocity()
        {
            if (rb.velocity.x < 0 && facingDirection == 1 || rb.velocity.x > 0 && facingDirection == -1)
                Flip();
        }

        public void PerformAttack(KnightData.Attack attackData)
        {
            rb.MovePosition(rb.position + new Vector2(attackData.horizontalMoveOffset * facingDirection, 0));

            int hitCount = Physics2D.OverlapBoxNonAlloc(
                rb.position + (attackData.triggerCollider.center * transform.localScale),
                attackData.triggerCollider.size, 0, attackHits, data.hittableLayer);

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

        public void SetColliderBounds(KnightData.ColliderBounds colliderBounds)
        {
            colliderBoundsSource = colliderBounds;
            _collider.offset = colliderBounds.bounds.min;
            _collider.size = colliderBounds.bounds.size;
            CollisionsCheck();
        }

        public Collider2D OverlapBoxOnGround(Rect bounds)
        {
            Vector2 charPosition = transform.position;
            Vector2 boundsPosition = bounds.position * transform.localScale;
            return Physics2D.OverlapBox(charPosition + boundsPosition, bounds.size, 0, data.groundLayer);
        }

        public void AddInvincibility(float time, bool shouldAnim)
        {
            StartCoroutine(StartInvincibility(time, shouldAnim));
        }

        public override void OnSceneTransition(SceneLoader.SceneTransitionData transitionData)
        {
            CharacterSpawnPoint spawnPoint = GetSceneSpawnPoint(transitionData);

            transform.position = spawnPoint.position;
            FlipTo(spawnPoint.facingToRight ? 1 : -1);

            FocusCameraOnThis();

            life.SetMaxLife(data.maxLife);
            if (transitionData.gameData.ch_knight_died)
            {
                life.SetLife(data.maxLife, RuntimeFields.RuntimeFieldSetMode.Setup);
                transitionData.gameData.ch_knight_died = false;
            }
            else
                life.SetLife(transitionData.gameData.ch_knight_life, RuntimeFields.RuntimeFieldSetMode.Setup);

            if (spawnPoint.isHorizontalDoor)
                stateMachine.EnterFakeWalk(data.fakeWalkOnSceneTransitionTime);
        }

        public override void BeforeUnload(SceneLoader.SceneUnloadData unloadData)
        {
            unloadData.gameData.ch_knight_life = life.currentLife;
            unloadData.gameData.ch_knight_died = isDied;
        }

        private void CollisionsCheck()
        {
            isGrounded = OverlapBoxOnGround(colliderBoundsSource.feetRect);
            isTouchingWall = OverlapBoxOnGround(colliderBoundsSource.handRect);
            canStand = !OverlapBoxOnGround(data.crouchHeadRect);
        }

        private IEnumerator StartInvincibility(float time, bool shouldAnim)
        {
            if (shouldAnim)
                _invincibilityAnimationsCount++;
            _invincibilityCount++;

            if (_invincibilityAnimationsCount > 0 && _invincibilityAnimationCoroutine == null)
                _invincibilityAnimationCoroutine = StartCoroutine(StartInvincibilityAnimation());

            yield return CoroutinesUtility.GetYieldSeconds(time);

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

        private void ReadMoveInput(float move) => horizontalMove = move;

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

            DrawColliderData(data.standColliderBounds);
            DrawColliderData(data.crouchColliderBounds);

            if (data.crouchColliderBounds.drawGizmos)
                drawer.SetColor(GizmosColor.instance.knight.feet)
                    .DrawWireSquare(position + (data.crouchHeadRect.min * scale), data.crouchHeadRect.size);

            void DrawAttack(KnightData.Attack attack)
            {
                if (!attack.drawGizmos)
                    return;

                drawer.DrawWireSquare(position + (attack.triggerCollider.center * scale), attack.triggerCollider.size);
            }

            void DrawColliderData(KnightData.ColliderBounds colliderBounds)
            {
                if (!colliderBounds.drawGizmos)
                    return;

                drawer.SetColor(GizmosColor.instance.knight.colliderData)
                    .DrawWireSquare(position + (colliderBounds.bounds.min * scale), colliderBounds.bounds.size)
                    .SetColor(GizmosColor.instance.knight.feet)
                    .DrawWireSquare(position + (colliderBounds.feetRect.min * scale), colliderBounds.feetRect.size)
                    .SetColor(GizmosColor.instance.knight.hand)
                    .DrawWireSquare(position + (colliderBounds.handRect.min * scale), colliderBounds.handRect.size);
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