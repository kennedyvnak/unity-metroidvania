using Metroidvania.Player.SafePoints;
using Metroidvania.Player.States;
using Metroidvania.SceneManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Metroidvania.Player
{
    /// <summary>The main player class which controls all components and actions</summary>
    public class PlayerController : MonoBehaviour, ISceneTransistor
    {
        [Serializable]
        public class Particles
        {
            public ParticleSystem jump;
            public ParticleSystem wallSlide;
            public ParticleSystem wallJump;
            public ParticleSystem landing;
            public ParticleSystem slide;
        }

#if UNITY_EDITOR
        [Tooltip("If true, draw gizmos in the scene view. (Editor only)")]
        [SerializeField]
        private bool m_drawGizmos;
#endif

        [Tooltip("The data active for this player. This field cannot be null")]
        [SerializeField]
        private PlayerDataChannel m_data;

        [Tooltip("The GFX GameObject. This field cannot be null")]
        [SerializeField]
        private GameObject m_gfxGameObject;

        [SerializeField]
        private Particles m_particles;

        /// <summary>The data active for this player. This field cannot be null</summary>
        public PlayerDataChannel data => m_data;

        /// <summary>The GFX GameObject</summary>
        public GameObject gfxGameObject => m_gfxGameObject;

        public Particles particles => m_particles;

        /// <summary>The Rigidbody2D attached to the player</summary>
        public Rigidbody2D rb { get; private set; }

        public bool isDied => stateMachine.currentState is PlayerDiedState;

        // Player components
        public PlayerAnimator animator { get; private set; }
        public PlayerCombat combat { get; private set; }
        public PlayerStateMachine stateMachine { get; private set; }
        public PlayerInput input { get; private set; }
        public PlayerInvincibility invincibility { get; private set; }
        public PlayerCollisions collisions { get; private set; }

        public List<PlayerComponent> playerComponents { get; private set; }

        /// <summary>The direction that the player is facing (1: right, -1: left)</summary>
        public int facingDirection { get; set; }

        /// <summary>Called when <see cref="Update"/> is called</summary>
        public event UnityAction LogicUpdated;

        /// <summary>Called when <see cref="FixedUpdate"/> is called</summary>
        public event UnityAction PhysicsUpdate;

        /// <summary>Called when <see cref="OnEnable"/> is called</summary>
        public event UnityAction Enabled;

        /// <summary>Called when <see cref="OnDisable"/> is called</summary>
        public event UnityAction Disabled;

        /// <summary>Called when <see cref="OnTriggerEnter2D"/> is called</summary>
        public event UnityAction<Collider2D> TriggerEntered;

        /// <summary>Called when <see cref="OnTriggerStay2D"/> is called</summary>
        public event UnityAction<Collider2D> TriggerStay;

        /// <summary>Called when <see cref="OnCollisionEnter2D"/> is called</summary>
        public event UnityAction<Collision2D> CollisionEntered;

        private void Awake()
        {
            if (data is null)
                throw new NullReferenceException($"The field {nameof(m_data)} cannot be null.");

            // Setup GameObject Components
            rb = GetComponent<Rigidbody2D>();

            // Setup Player Data
            facingDirection = 1;

            // Setup Player Components
            playerComponents = new List<PlayerComponent>();
            animator = new PlayerAnimator(this);
            input = new PlayerInput(this);
            collisions = new PlayerCollisions(this);
            invincibility = new PlayerInvincibility(this);
            combat = new PlayerCombat(this);
            stateMachine = new PlayerStateMachine(this);
        }

        private void Update()
        {
            LogicUpdated?.Invoke();
        }

        private void FixedUpdate()
        {
            PhysicsUpdate?.Invoke();
        }

        private void OnEnable()
        {
            Enabled?.Invoke();
        }

        private void OnDisable()
        {
            Disabled?.Invoke();
        }

        private void OnDestroy()
        {
            foreach (PlayerComponent component in playerComponents)
                component.OnDestroy();
            playerComponents.Clear();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            TriggerEntered?.Invoke(col);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TriggerStay?.Invoke(other);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            CollisionEntered?.Invoke(other);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!m_drawGizmos || !data)
                return;

            Transform t = transform;
            Vector2 position = (Vector2)t.position;
            Vector2 scale = (Vector2)t.localScale;

            GizmosDrawer drawer = new GizmosDrawer();

            drawer.SetColor(GizmosColor.instance.playerAttack);
            DrawAttack(data.attackOne);
            DrawAttack(data.attackTwo);
            DrawAttack(data.crouchAttack);

            DrawColliderData(data.standColliderData);
            DrawColliderData(data.crouchColliderData);

            if (data.crouchColliderData.drawGizmos)
                drawer.SetColor(GizmosColor.instance.playerFeet)
                    .DrawWireSquare(position + (data.crouchHeadRect.min * scale), data.crouchHeadRect.size);

            void DrawAttack(PlayerDataChannel.Attack attack)
            {
                if (!attack.drawGizmos)
                    return;

                drawer.DrawWireSquare(position + (attack.triggerCollider.center * scale), attack.triggerCollider.size);
            }

            void DrawColliderData(PlayerDataChannel.ColliderData colliderData)
            {
                if (!colliderData.drawGizmos)
                    return;

                drawer.SetColor(GizmosColor.instance.playerColliderData)
                    .DrawWireSquare(position + (colliderData.bounds.min * scale), colliderData.bounds.size)
                    .SetColor(GizmosColor.instance.playerFeet)
                    .DrawWireSquare(position + (colliderData.feetRect.min * scale), colliderData.feetRect.size)
                    .SetColor(GizmosColor.instance.playerHand)
                    .DrawWireSquare(position + (colliderData.handRect.min * scale), colliderData.handRect.size);
            }
        }
#endif

        /// <summary>Shortcut for move the player using the input.horizontalMove * speed</summary>
        /// <param name="speed">The move speed</param>
        /// <param name="autoFlip">If true, checks the flip after apply the velocity</param>
        public void MoveHorizontalAxesUsingInput(float speed, bool autoFlip = true)
        {
            SetHorizontalVelocity(input.horizontalMove * speed);
            if (autoFlip)
                animator.FlipCheck();
        }

        /// <summary>Apply the horizontal velocity to the rigidbody without change the vertical velocity</summary>
        /// <param name="x">The new horizontal velocity</param>
        public void SetHorizontalVelocity(float x)
        {
            rb.velocity = new Vector2(x, rb.velocity.y);
        }

        public void SetFacingDirection(int newFacingDirection)
        {
            if (facingDirection != newFacingDirection) animator.Flip();
        }

        public void OnSceneTransition(SceneLoader.SceneTransitionData transitionData)
        {
            bool doFakeWalk = false;
            SceneSpawnPoints spawnPoints = transitionData.currentScene.spawnPoints;
            SceneSpawnPoints.SceneSpawnPoint spawnPoint = spawnPoints.defaultSpawnPoint;

            switch (transitionData.spawnPoint)
            {
                case SceneLoader.SceneTransitionData.UseGameDataKey:
                case SceneLoader.SceneTransitionData.GameOverKey:
                    PlayerSafePointsArea safePoints = PlayerSafePointsArea.instance;
                    PlayerSafePoint safePoint = safePoints.GetSafePoint(transitionData.gameData.lastPlayerSafePoint.pointGuid);
                    spawnPoint.facingRight = safePoint.facingRight;
                    spawnPoint.position = safePoint.relativePoint;
                    doFakeWalk = false;
                    break;
#if UNITY_EDITOR
                case SceneLoader.SceneTransitionData.EditorInitializationKey:
                    spawnPoint.position = transform.position;
                    spawnPoint.facingRight = facingDirection == 1;
                    doFakeWalk = false;
                    break;
#endif
                default:
                    spawnPoints.TryGetSpawnPoint(transitionData.spawnPoint, ref spawnPoint);
                    doFakeWalk = true;
                    break;
            }

            transform.position = spawnPoint.position;
            SetFacingDirection(spawnPoint.facingRight ? 1 : -1);

            // This code sets cinemachine's target and resets its position
            CameraUtility.vCam.Follow = transform;
            CameraUtility.vCam.PreviousStateIsValid = false;

            data.lifeField.SetValue(transitionData.gameData.playerLife, RuntimeFields.RuntimeFieldSetMode.Setup);

            if (doFakeWalk)
                stateMachine.EnterFakeWalk(data.fakeWalkOnSceneTransitionTime);
        }

        public void BeforeUnload(SceneLoader.SceneUnloadData unloadData)
        {
            if (isDied)
            {
                unloadData.gameData.isPlayerDied = true;
                return;
            }

            unloadData.gameData.playerLife = data.lifeField.value;
            unloadData.gameData.isPlayerDied = false;
        }
    }
}