using System;
using Metroidvania.InputSystem;
using UnityEngine;

namespace Metroidvania.Player
{
    [CreateAssetMenu(menuName = "Scriptables/Player/Data")]
    public class PlayerDataChannel : ScriptableObject
    {
        [Serializable]
        public struct Attack
        {
#if UNITY_EDITOR
            [Tooltip("Draw the gizmos of this attack if true")] [SerializeField]
            private bool m_drawGizmos;
#endif

            [Tooltip("Duration of the attack")] [SerializeField, Space]
            private float m_duration;

            [Tooltip("The offset that the player's move when the attack is triggered (don't be confused with started)")]
            [SerializeField]
            private float m_horizontalMoveOffset;

            [Tooltip("Time after the attack start that triggers the attack")] [SerializeField, Space]
            private float m_triggerTime;

            [Tooltip("The area to make the collision check")] [SerializeField]
            private Rect m_triggerCollider;

            [Tooltip("Raw damage of the attack")] [SerializeField, Space]
            private int m_damage;

            [Tooltip("Raw force of the attack")] [SerializeField]
            private float m_force;

#if UNITY_EDITOR
            /// <summary>Draw the gizmos of this attack if true</summary>
            public bool drawGizmos
            {
                get => m_drawGizmos;
                set => m_drawGizmos = value;
            }
#endif
            /// <summary>Duration of the attack</summary>
            public float duration
            {
                get => m_duration;
                set => m_duration = value;
            }

            /// <summary>The offset that the player's move when the attack is triggered (don't be confused with started)</summary>
            public float horizontalMoveOffset
            {
                get => m_horizontalMoveOffset;
                set => m_horizontalMoveOffset = value;
            }

            /// <summary>Time after the attack start that triggers the attack</summary>
            public float triggerTime
            {
                get => m_triggerTime;
                set => m_triggerTime = value;
            }

            /// <summary>The area to make the collision check</summary>
            public Rect triggerCollider
            {
                get => m_triggerCollider;
                set => m_triggerCollider = value;
            }

            /// <summary>Raw damage of the attack</summary>
            public int damage
            {
                get => m_damage;
                set => m_damage = value;
            }

            /// <summary>Raw force of the attack</summary>
            public float force
            {
                get => m_force;
                set => m_force = value;
            }
        }

        [Tooltip("Max life of the player")] [Header("Properties")] [SerializeField]
        private int m_maxLife;

        [Tooltip("Default invincibility time")] [SerializeField]
        private int m_defaultInvincibilityTime;

        [Tooltip("Input reader for input handling")] [Header("Input")] [SerializeField]
        private InputReader m_inputReader;

        [Tooltip("Ground layer for collisions check")] [Header("Ground Check")] [SerializeField]
        private LayerMask m_groundLayer;

        [Tooltip("Feet offset based on center of player position")] [SerializeField]
        private Vector2 m_feetOffset;

        [SerializeField] [Tooltip("Size of the feet")]
        private Vector2 m_feetRadius;

        [Header("Wall Check")] [Tooltip("Wall layer for collisions check")] [SerializeField]
        private LayerMask m_wallLayer;

        [Tooltip("Left hand offset based on center of the player position")] [SerializeField]
        private Vector2 m_leftHandOffset;

        [Tooltip("Size of the left hand")] [SerializeField]
        private Vector2 m_leftHandSize;

        [Tooltip("Right hand offset based on center of the player position")] [SerializeField]
        private Vector2 m_rightHandOffset;

        [Tooltip("Size of the right hand")] [SerializeField]
        private Vector2 m_rightHandSize;

        [Tooltip("Offset based on center of the player position")] [SerializeField]
        private Vector2 m_ledgeCheckOffset;

        [Tooltip("Size of the ledge check")] [SerializeField]
        private float m_ledgeCheckLenght;

        [Tooltip("Default movement speed")] [Header("Movement")] [SerializeField]
        private float m_moveSpeed;

        [Tooltip("Default jump speed")] [Header("Jump")] [SerializeField]
        private float m_jumpSpeed;

        [Tooltip("Jump duration")] [SerializeField]
        private float m_jumpDuration;

        [Tooltip("A curve for smooth jump velocity")] [SerializeField]
        private AnimationCurve m_jumpCurve;

        [Tooltip("Default crouch speed")] [Header("Crouch")] [SerializeField]
        private float m_crouchSpeed;

        [Tooltip("Slide duration")] [Header("Slide")] [SerializeField]
        private float m_slideDuration;

        [Tooltip("Slide speed")] [SerializeField]
        private float m_slideSpeed;

        [Tooltip("The slide cooldown")] [SerializeField]
        private float m_slideCooldown;

        [Tooltip("A curve for smooth slide speed")] [SerializeField]
        private AnimationCurve m_slideCurve;

        [Tooltip("Roll duration")] [Header("Roll")] [SerializeField]
        private float m_rollDuration;

        [Tooltip("Roll speed")] [SerializeField]
        private float m_rollSpeed;

        [Tooltip("The roll cooldown")] [SerializeField]
        private float m_rollCooldown;

        [Tooltip("A curve for smooth roll speed")] [SerializeField]
        private AnimationCurve m_rollCurve;

        [Tooltip("Layer for collisions check of hits")] [Header("Attacks")] [SerializeField]
        private LayerMask m_hittableLayer;

        [Tooltip("First attack data")] [SerializeField]
        private Attack m_attackOne;

        [Tooltip("Second attack data")] [SerializeField]
        private Attack m_attackTwo;

        [Tooltip("Crouch attack data")] [SerializeField]
        private Attack m_crouchAttack;

        [Tooltip("Duration of the crouch transition animation")] [Header("Animations")] [SerializeField]
        private float m_crouchTransitionTime;

        [Tooltip("Duration of the slide transition animation")] [SerializeField]
        private float m_slideTransitionTime;

        [Tooltip("Vertical velocity that will be applied on player's velocity when sliding down a wall")]
        [Header("Wall Abilities")]
        [SerializeField]
        private float m_wallSlideSpeed;

        [Tooltip("Time which the player stay on hurt state")] [Header("Hurt")] [SerializeField]
        private float m_hurtTime;

        public int maxLife
        {
            get => m_maxLife;
            set => m_maxLife = value;
        }

        public int defaultInvincibilityTime
        {
            get => m_defaultInvincibilityTime;
            set => m_defaultInvincibilityTime = value;
        }

        public InputReader inputReader
        {
            get => m_inputReader;
            set => m_inputReader = value;
        }

        public LayerMask groundLayer
        {
            get => m_groundLayer;
            set => m_groundLayer = value;
        }

        public Vector2 feetOffset
        {
            get => m_feetOffset;
            set => m_feetOffset = value;
        }

        public Vector2 feetRadius
        {
            get => m_feetRadius;
            set => m_feetRadius = value;
        }

        public LayerMask wallLayer
        {
            get => m_wallLayer;
            set => m_wallLayer = value;
        }

        public Vector2 leftHandOffset
        {
            get => m_leftHandOffset;
            set => m_leftHandOffset = value;
        }

        public Vector2 leftHandSize
        {
            get => m_leftHandSize;
            set => m_leftHandSize = value;
        }

        public Vector2 rightHandOffset
        {
            get => m_rightHandOffset;
            set => m_rightHandOffset = value;
        }

        public Vector2 rightHandSize
        {
            get => m_rightHandSize;
            set => m_rightHandSize = value;
        }

        public Vector2 ledgeCheckOffset
        {
            get => m_ledgeCheckOffset;
            set => m_ledgeCheckOffset = value;
        }

        public float ledgeCheckLenght
        {
            get => m_ledgeCheckLenght;
            set => m_ledgeCheckLenght = value;
        }

        public float moveSpeed
        {
            get => m_moveSpeed;
            set => m_moveSpeed = value;
        }

        public float jumpSpeed
        {
            get => m_jumpSpeed;
            set => m_jumpSpeed = value;
        }

        public float jumpDuration
        {
            get => m_jumpDuration;
            set => m_jumpDuration = value;
        }

        public AnimationCurve jumpCurve
        {
            get => m_jumpCurve;
            set => m_jumpCurve = value;
        }

        public float crouchSpeed
        {
            get => m_crouchSpeed;
            set => m_crouchSpeed = value;
        }

        public float slideDuration
        {
            get => m_slideDuration;
            set => m_slideDuration = value;
        }

        public float slideSpeed
        {
            get => m_slideSpeed;
            set => m_slideSpeed = value;
        }

        public float slideCooldown
        {
            get => m_slideCooldown;
            set => m_slideCooldown = value;
        }

        public AnimationCurve slideCurve
        {
            get => m_slideCurve;
            set => m_slideCurve = value;
        }

        public float rollDuration
        {
            get => m_rollDuration;
            set => m_rollDuration = value;
        }

        public float rollSpeed
        {
            get => m_rollSpeed;
            set => m_rollSpeed = value;
        }

        public AnimationCurve rollCurve
        {
            get => m_rollCurve;
            set => m_rollCurve = value;
        }

        public float rollCooldown
        {
            get => m_rollCooldown;
            set => m_rollCooldown = value;
        }

        public LayerMask hittableLayer
        {
            get => m_hittableLayer;
            set => m_hittableLayer = value;
        }

        public Attack attackOne
        {
            get => m_attackOne;
            set => m_attackOne = value;
        }

        public Attack attackTwo
        {
            get => m_attackTwo;
            set => m_attackTwo = value;
        }

        public Attack crouchAttack
        {
            get => m_crouchAttack;
            set => m_crouchAttack = value;
        }

        public float crouchTransitionTime
        {
            get => m_crouchTransitionTime;
            set => m_crouchTransitionTime = value;
        }

        public float slideTransitionTime
        {
            get => m_slideTransitionTime;
            set => m_slideTransitionTime = value;
        }

        public float wallSlideSpeed
        {
            get => m_wallSlideSpeed;
            set => m_wallSlideSpeed = value;
        }

        public float hurtTime
        {
            get => m_hurtTime;
            set => m_hurtTime = value;
        }
    }
}