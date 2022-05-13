using System;
using Metroidvania.InputSystem;
using UnityEngine;

namespace Metroidvania.Player
{
    [CreateAssetMenu(menuName = "Scriptables/Player/Data")]
    public class PlayerDataChannel : ScriptableObject
    {
        [Header("Properties")] public int m_maxLife;

        public int m_defaultInvincibilityTime;

        [Header("Input")] public InputReader m_inputReader;

        [Header("Ground Check")] public LayerMask m_groundLayer;

        public Vector2 m_feetOffset;
        public Vector2 m_feetRadius;

        [Header("Wall Check")] public LayerMask m_wallLayer;

        public Vector2 m_leftHandOffset;
        public Vector2 m_leftHandSize;
        public Vector2 m_rightHandOffset;
        public Vector2 m_rightHandSize;
        public Vector2 m_ledgeCheckOffset;
        public float m_ledgeCheckLenght;

        [Header("Movement")] public float m_moveSpeed;

        [Header("Jump")] public float m_jumpSpeed;

        public float m_jumpDuration;
        public AnimationCurve m_jumpCurve;

        [Header("Crouch")] public float m_crouchSpeed;

        [Header("Slide")] public float m_slideDuration;

        public float m_slideSpeed;
        public float m_slideCooldown;
        public AnimationCurve m_slideCurve;

        [Header("Roll")] public float m_rollDuration;

        public float m_rollSpeed;
        public AnimationCurve m_rollCurve;
        public float m_rollCooldown;

        [Header("Attacks")] public LayerMask m_hittableLayer;

        public Attack m_attackOne;
        public Attack m_attackTwo;
        public Attack m_crouchAttack;

        [Header("Animations")] public float m_crouchTransitionTime;

        public float m_slideTransitionTime;

        [Header("Wall Abilities")] public float m_wallSlideSpeed;

        [Header("Hurt")] public float m_hurtTime;

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

        [Serializable]
        public struct Attack
        {
            [SerializeField] private bool m_drawGizmos;

            [Space] [SerializeField] private float m_duration;

            [SerializeField] private float m_horizontalMoveOffset;

            [Space] [SerializeField] private float m_triggerTime;

            [SerializeField] private Rect m_triggerCollider;

            [Space] [SerializeField] private int m_damage;

            [SerializeField] private float m_force;

            public bool drawGizmos
            {
                get => m_drawGizmos;
                set => m_drawGizmos = value;
            }

            public float duration
            {
                get => m_duration;
                set => m_duration = value;
            }

            public float horizontalMoveOffset
            {
                get => m_horizontalMoveOffset;
                set => m_horizontalMoveOffset = value;
            }

            public float triggerTime
            {
                get => m_triggerTime;
                set => m_triggerTime = value;
            }

            public Rect triggerCollider
            {
                get => m_triggerCollider;
                set => m_triggerCollider = value;
            }

            public int damage
            {
                get => m_damage;
                set => m_damage = value;
            }

            public float force
            {
                get => m_force;
                set => m_force = value;
            }
        }
    }
}