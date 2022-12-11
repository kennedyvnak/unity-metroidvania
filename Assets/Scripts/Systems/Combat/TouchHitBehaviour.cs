using Metroidvania.Entities;
using UnityEngine;

namespace Metroidvania.Combat {
    [RequireComponent(typeof(Collider2D))]
    public class TouchHitBehaviour : MonoBehaviour, ITouchHit {
        [SerializeField] private bool m_IgnoreInvincibility = false;
        [SerializeField] private float m_Damage = 10;
        [SerializeField] private Vector2 m_KnockbackForce = new Vector2(9.0f, -3.5f);
        [SerializeField] private bool m_UseFacingDirection = true;

        public Collider2D colliderTrigger { get; private set; }

        public bool ignoreInvincibility { get => m_IgnoreInvincibility; set => m_IgnoreInvincibility = value; }
        public float damage { get => m_Damage; set => m_Damage = value; }
        public Vector2 knockbackForce { get => m_KnockbackForce; set => m_KnockbackForce = value; }
        public bool useFacingDirection { get => m_UseFacingDirection; set => m_UseFacingDirection = value; }

        private void Awake() {
            colliderTrigger = GetComponent<Collider2D>();
        }

        private void OnEnable() {
            colliderTrigger.enabled = true;
        }

        private void OnDisable() {
            colliderTrigger.enabled = false;
        }

        public EntityHitData OnHitCharacter(Characters.CharacterBase character) {
            return m_UseFacingDirection
                ? new EntityHitData(m_Damage, new Vector2(m_KnockbackForce.x * -character.facingDirection, m_KnockbackForce.y))
                : new EntityHitData(m_Damage, m_KnockbackForce);
        }
    }
}