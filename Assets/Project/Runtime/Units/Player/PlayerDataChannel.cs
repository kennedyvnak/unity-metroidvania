using System;
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
            [Tooltip("Draw the gizmos of this attack if true")]
            public bool drawGizmos;
#endif

            [Space] [Tooltip("Duration of the attack")]
            public float duration;

            [Tooltip("The offset that the player's move when the attack is triggered (don't be confused with started)")]
            public float horizontalMoveOffset;

            [Space] [Tooltip("Time after the attack start that triggers the attack")]
            public float triggerTime;

            [Tooltip("The area to make the collision check")]
            public Rect triggerCollider;

            [Space] [Tooltip("Raw damage of the attack")]
            public int damage;

            [Tooltip("Raw force of the attack")] public float force;
        }

        [Header("Properties")] [Tooltip("Max life of the player")]
        public int maxLife;
        
        [Header("Ground Check")] [Tooltip("Ground layer for collisions check")]
        public LayerMask groundLayer;

        [Tooltip("Feet offset based on center of player position")]
        public Vector2 feetOffset;

        [Tooltip("Size of the feet")] public Vector2 feetRadius;

        [Header("Wall Check")] [Tooltip("Wall layer for collisions check")]
        public LayerMask wallLayer;

        [Tooltip("Left hand offset based on center of the player position")]
        public Vector2 leftHandOffset;

        [Tooltip("Size of the left hand")] public Vector2 leftHandSize;

        [Tooltip("Right hand offset based on center of the player position")]
        public Vector2 rightHandOffset;

        [Tooltip("Size of the right hand")] public Vector2 rightHandSize;

        [Tooltip("Offset based on center of the player position")]
        public Vector2 ledgeCheckOffset;

        [Tooltip("Size of the ledge check")] public float ledgeCheckLenght;

        [Header("Movement")] [Tooltip("Default movement speed")]
        public float moveSpeed;

        [Header("Jump")] [Tooltip("Default jump speed")]
        public float jumpSpeed;

        [Tooltip("Jump duration")] public float jumpDuration;

        [Tooltip("A curve for smooth jump velocity")]
        public AnimationCurve jumpCurve;

        [Header("Crouch")] [Tooltip("Default crouch speed")]
        public float crouchSpeed;

        [Header("Slide")] [Tooltip("Slide duration")]
        public float slideDuration;

        [Tooltip("Slide speed")] public float slideSpeed;

        [Tooltip("The slide cooldown")] public float slideCooldown;

        [Tooltip("A curve for smooth slide speed")]
        public AnimationCurve slideCurve;

        [Header("Roll")] [Tooltip("Roll duration")]
        public float rollDuration;

        [Tooltip("Roll speed")] public float rollSpeed;

        [Tooltip("The roll cooldown")] public float rollCooldown;

        [Tooltip("A curve for smooth roll speed")]
        public AnimationCurve rollCurve;

        [Header("Attacks")] [Tooltip("Layer for collisions check of hits")]
        public LayerMask hittableLayer;

        [Tooltip("First attack data")] public Attack attackOne;

        [Tooltip("Second attack data")] public Attack attackTwo;

        [Tooltip("Crouch attack data")] public Attack crouchAttack;

        [Header("Animations")] [Tooltip("Duration of the crouch transition animation")]
        public float crouchTransitionTime;

        [Tooltip("Duration of the slide transition animation")]
        public float slideTransitionTime;

        [Header("Wall Abilities")]
        [Tooltip("Vertical velocity that will be applied on player's velocity when sliding down a wall")]
        public float wallSlideSpeed;

        [Tooltip("Time which the player stay on hurt state")] [Header("Hurt")]
        public float hurtTime;

        [Header("Invincibility")] [Tooltip("The alpha used in the invincibility animation to set the limits")]
        public float invincibilityAlphaChange;

        [Tooltip("The speed that interpolates the alpha when in invincibility")]
        public float invincibilityFadeSpeed;

        [Tooltip("Default invincibility time")]
        public float defaultInvincibilityTime;
    }
}