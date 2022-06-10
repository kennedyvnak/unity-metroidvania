using System;
using UnityEngine;

namespace Metroidvania.Player
{
    [CreateAssetMenu(menuName = "Scriptables/Player/Data")]
    public class PlayerDataChannel : ScriptableObject
    {
        [Serializable]
        public class Attack
        {
#if UNITY_EDITOR
            [Tooltip("Draw the gizmos of this attack if true")]
            public bool drawGizmos;
#endif

            [Space]
            [Tooltip("Duration of the attack")]
            public float duration;

            [Tooltip("The offset that the player's move when the attack is triggered (don't be confused with started)")]
            public float horizontalMoveOffset;

            [Space]
            [Tooltip("Time after the attack start that triggers the attack")]
            public float triggerTime;

            [Tooltip("The time before attack end that the player can use an ability (perform other attack, use roll...)")]
            public float attackEndOffset;

            [Tooltip("The area to make the collision check")]
            public Rect triggerCollider;

            [Space]
            [Tooltip("Raw damage of the attack")]
            public int damage;

            [Tooltip("Raw force of the attack")] public float force;
        }

        [Serializable]
        public class ColliderData
        {
#if UNITY_EDITOR
            public bool drawGizmos;
#endif
            public Rect bounds;

            public Rect feetRect;

            [UnityEngine.Serialization.FormerlySerializedAs("rightHandRect")]
            public Rect handRect;
        }


        [Header("Properties")]
        [Tooltip("Max life of the player")]
        public int maxLife;

        [Header("Ground Check")]
        [Tooltip("Ground layer for collisions check")]
        public LayerMask groundLayer;

        [Header("Movement")]
        [Tooltip("Default movement speed")]
        public float moveSpeed;

        [Header("Jump")]
        [Tooltip("Default jump speed")]
        public float jumpSpeed;
        [Tooltip("Jump duration")]
        public float jumpDuration;
        [Tooltip("Time considering that the jump input is pressed")]
        public float jumpInputDelay;
        [Tooltip("A curve for smooth jump velocity")]
        public AnimationCurve jumpCurve;

        [Header("Fall")]
        public float fallParticlesDistance;


        [Header("Crouch")]
        [Tooltip("Default crouch speed")]
        public float crouchSpeed;
        [Tooltip("Duration of the crouch transition animation")]
        public float crouchTransitionTime;

        [Header("Slide")]
        [Tooltip("Slide duration")]
        public float slideDuration;
        [Tooltip("Slide speed")]
        public float slideSpeed;
        [Tooltip("The slide cooldown")]
        public float slideCooldown;
        [Tooltip("A curve for smooth slide speed")]
        public AnimationCurve slideCurve;
        [Tooltip("Duration of the slide transition animation")]
        public float slideTransitionTime;

        [Header("Roll")]
        [Tooltip("Roll duration")]
        public float rollDuration;
        [Tooltip("Roll speed")]
        public float rollSpeed;
        [Tooltip("The roll cooldown")]
        public float rollCooldown;
        [Tooltip("A curve for smooth roll speed")]
        public AnimationCurve rollCurve;

        [Header("Attacks")]
        [Tooltip("Layer for collisions check of hits")]
        public LayerMask hittableLayer;

        [Tooltip("First attack data")]
        public Attack attackOne;
        [Tooltip("Second attack data")]
        public Attack attackTwo;
        [Tooltip("Crouch attack data")]
        public Attack crouchAttack;

        [Header("Wall Abilities")]
        [Tooltip("Vertical velocity that will be applied on player's velocity when sliding down a wall")]
        public float wallSlideSpeed;
        public Vector2 wallJumpForce;
        public float wallJumpDuration;

        [Header("Hurt")]
        [Tooltip("Time which the player stay on hurt state")]
        public float hurtTime;

        [Header("Invincibility")]
        [Tooltip("The alpha used in the invincibility animation to set the limits")]
        public float invincibilityAlphaChange;
        [Tooltip("The speed that interpolates the alpha when in invincibility")]
        public float invincibilityFadeSpeed;
        [Tooltip("Default invincibility time")]
        public float defaultInvincibilityTime;

        [Header("Colliders")]
        public ColliderData standColliderData;
        public ColliderData crouchColliderData;
        public Rect crouchHeadRect;
    }
}