using Metroidvania.SceneManagement;
using UnityEngine;

namespace Metroidvania.Characters.SafePoints {
    public class CharacterSafePointsArea : Singleton<CharacterSafePointsArea> {
        [SerializeField] private SceneChannel m_scene;
        public string sceneGUID => m_scene.channelReference.AssetGUID;

        [SerializeField] private CharacterSafePoint _defaultPlayerPoint;
        public CharacterSafePoint defaultPlayerPoint => _defaultPlayerPoint;

        private CharacterSafePoint[] _safePoints;
        public CharacterSafePoint[] safePoints => _safePoints;

        private void Start() {
            _safePoints = GetComponentsInChildren<CharacterSafePoint>();

            foreach (CharacterSafePoint safePoint in _safePoints) {
                safePoint.area = this;
                CreateBoxTrigger(safePoint);
            }
        }

        public BoxCollider2D CreateBoxTrigger(CharacterSafePoint point) {
            BoxCollider2D trigger = point.gameObject.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true;
            trigger.size = point.triggerSize;
            trigger.offset = point.triggerOffset;
            return trigger;
        }

        public CharacterSafePoint GetSafePoint(System.Guid safePointGUID) {
            for (int i = 0; i < safePoints.Length; i++) {
                CharacterSafePoint safePoint = safePoints[i];
                if (safePoint.guid.Equals(safePointGUID))
                    return safePoint;
            }
            return defaultPlayerPoint;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            foreach (CharacterSafePoint safePoint in transform.GetComponentsInChildren<CharacterSafePoint>()) {
                Rect safePointTriggerBounds = new Rect();
                Vector2 safePointPosition = (Vector2)safePoint.transform.position + safePoint.triggerOffset;
                safePointTriggerBounds.size = safePoint.triggerSize;
                safePointTriggerBounds.position = safePointPosition - (safePoint.triggerSize * .5f);
                UnityEditor.Handles.DrawSolidRectangleWithOutline(safePointTriggerBounds, GizmosColor.instance.safePoints.area, Color.white);
                UnityEditor.Handles.Label(safePointPosition, safePoint.name);
            }
        }
#endif
    }
}