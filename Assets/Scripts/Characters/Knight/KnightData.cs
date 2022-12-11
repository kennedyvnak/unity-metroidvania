using Metroidvania.Events;
using System;
using UnityEngine;

namespace Metroidvania.Characters.Knight {
    [CreateAssetMenu(fileName = "KnightData", menuName = "Scriptables/Characters/Knight Data")]
    public class KnightData : ScriptableObject {
        [Serializable]
        public class Attack {
#if UNITY_EDITOR
            public bool drawGizmos;
#endif

            [Space]
            public float duration;

            public float horizontalMoveOffset;

            [Space]
            public float triggerTime;

            public float attackEndOffset;

            public Rect triggerCollider;

            [Space]
            public int damage;
            public float force;
        }

        [Serializable]
        public class ColliderBounds {
#if UNITY_EDITOR
            public bool drawGizmos;
#endif
            public Rect bounds;

            public Rect feetRect;

            public Rect handRect;
        }

        [Header("Properties")]
        public float maxLife;

        [Header("Events")]
        public ObjectEventChannel onDieChannel;
        public CharacterHurtEventChannel onHurtChannel;

        [Header("Ground Check")]
        public LayerMask groundLayer;

        [Header("Movement")]
        public float moveSpeed;

        [Header("Jump")]
        public float jumpSpeed;
        public float jumpDuration;
        public AnimationCurve jumpCurve;

        [Header("Fall")]
        public float fallParticlesDistance;

        [Header("Crouch")]
        public float crouchWalkSpeed;
        public float crouchTransitionTime;

        [Header("Slide")]
        public float slideDuration;
        public float slideSpeed;
        public float slideCooldown;
        public AnimationCurve slideMoveCurve;
        public float slideTransitionTime;

        [Header("Roll")]
        public float rollDuration;
        public float rollSpeed;
        public float rollCooldown;
        public AnimationCurve rollHorizontalMoveCurve;

        [Header("Attacks")]
        public LayerMask hittableLayer;
        public float attackComboMaxDelay;
        public Attack firstAttack;
        public Attack secondAttack;
        public Attack crouchAttack;

        [Header("Wall Abilities")]
        public float wallSlideSpeed;
        public Vector2 wallJumpForce;
        public float wallJumpDuration;

        [Header("Hurt")]
        public float hurtTime;

        [Header("Fake Walk")]
        public float fakeWalkOnSceneTransitionTime;

        [Header("Invincibility")]
        public float invincibilityAlphaChange;
        public float invincibilityFadeSpeed;
        public float defaultInvincibilityTime;

        [Header("Colliders")]
        public ColliderBounds standColliderBounds;
        public ColliderBounds crouchColliderBounds;
        public Rect crouchHeadRect;
    }
}