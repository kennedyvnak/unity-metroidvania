using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Metroidvania.Player
{
    /// <summary>
    /// The main player clas
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
#if UNITY_EDITOR
        [Tooltip("If true, draw gizmos in the scene view. (Editor only)")]
        [SerializeField] private bool m_drawGizmos;
#endif
        
        [Tooltip("The data active for this player. This field cannot be null")]
        [SerializeField] private PlayerDataChannel m_data;
        
        [Tooltip("The GFX GameObject")]
        [SerializeField] private GameObject m_gfxGameObject;
        
        /// <summary>The data active for this player. This field cannot be null</summary>
        public PlayerDataChannel data => m_data;
        
        /// <summary>The GFX GameObject</summary>
        public GameObject gfxGameObject => m_gfxGameObject;

        /// <summary>The Rigidbody2D attached to the player</summary>
        public Rigidbody2D rb { get; private set; }
        
        // Player components
        public PlayerAnimator animator { get; private set; }
        public PlayerCombat combat { get; private set; }
        public PlayerStateMachine stateMachine { get; private set; }
        public PlayerInput input { get; private set; }
        public PlayerInvincibility invincibility { get; private set; }
        public PlayerCollisions collisions { get; private set; }

        public List<PlayerComponent> playerComponents { get; private set; }

        public int life { get; set; }
        
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

        private void Awake()
        {
            if (data is null)
                throw new NullReferenceException($"The field {nameof(m_data)} cannot be null.");

            // Setup GameObject Components
            rb = GetComponent<Rigidbody2D>();

            // Setup Player Data
            life = data.maxLife;
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
            foreach (var component in playerComponents)
                component.OnDestroy();
            playerComponents.Clear();
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            TriggerEntered?.Invoke(col);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!m_drawGizmos) return;

            if (data == null)
                return;

            var t = transform;
            var position = (Vector2)t.position;
            var scale = (Vector2)t.localScale;

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(position + data.feetOffset * scale, data.feetRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(position + data.leftHandOffset * scale, data.leftHandSize);
            Gizmos.DrawWireCube(position + data.rightHandOffset * scale, data.rightHandSize);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(position + data.ledgeCheckOffset * scale, new Vector2(data.ledgeCheckLenght * scale.x, 0));

            Gizmos.color = Color.cyan;
            DrawAttack(data.attackOne);
            DrawAttack(data.attackTwo);
            DrawAttack(data.crouchAttack);

            void DrawAttack(PlayerDataChannel.Attack attack)
            {
                if (attack.drawGizmos)
                    Gizmos.DrawWireCube(position + attack.triggerCollider.center * scale, attack.triggerCollider.size);
            }
        }
#endif

        /// <summary>Shortcut for move the player using the input.horizontalMove * speed</summary>
        /// <param name="speed">The move speed</param>
        /// <param name="autoFlip">If true, checks the flip after apply the velocity</param>
        public void MoveHorizontalAxes(float speed, bool autoFlip = true)
        {
            SetHorizontalVelocity(input.horizontalMove * speed);
            if (autoFlip)
                animator.FlipCheck();
        }
        
        /// <summary>Apply the horizontal velocity to the rigidbody without change the vertical velocity</summary>
        /// <param name="x">The horizontal velocity</param>
        public void SetHorizontalVelocity(float x)
        {
            rb.velocity = new Vector2(x, rb.velocity.y);
        }
    }
}