using Metroidvania.Serialization;
using UnityEngine;

namespace Metroidvania.Characters.SafePoints {
    public class CharacterSafePoint : MonoBehaviour {
        public CharacterSafePointsArea area { get; internal set; }

        [SerializeField] private Vector2 m_triggerSize;
        public Vector2 triggerSize => m_triggerSize;

        [SerializeField] private Vector2 m_triggerOffset;
        public Vector2 triggerOffset => m_triggerOffset;

        [UnityEngine.Serialization.FormerlySerializedAs("m_relativePoint")]
        [SerializeField] private Vector2 m_position;
        public Vector2 position => m_position;

        [SerializeField] private bool m_facingRight;
        public bool facingRight => m_facingRight;

        [SerializeField] private SerializableGuid m_guid;
        public System.Guid guid => m_guid;

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.transform.CompareTag("Player") && other.TryGetComponent<CharacterBase>(out CharacterBase character)) {
                GameData gameData = DataManager.instance.gameData;
                gameData.lastCharacterSafePoint.pointGUID = m_guid;
                gameData.lastCharacterSafePoint.sceneGUID = area.sceneGUID;
            }
        }

#if UNITY_EDITOR
        private void Reset() {
            GenerateGUID();
        }

        private void GenerateGUID() {
            m_guid = System.Guid.NewGuid();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}