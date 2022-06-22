using Metroidvania.SceneManagement;
using UnityEngine;

namespace Metroidvania.Player.SafePoints
{
    public class PlayerSafePointsArea : Singleton<PlayerSafePointsArea>
    {
        [SerializeField] private SceneChannel m_scene;
        public string sceneGUID => m_scene.channelReference.AssetGUID;

        [SerializeField] private PlayerSafePoint _defaultPlayerPoint;
        public PlayerSafePoint defaultPlayerPoint => _defaultPlayerPoint;

        private PlayerSafePoint[] _safePoints;
        public PlayerSafePoint[] safePoints => _safePoints;

        private void Start()
        {
            _safePoints = GetComponentsInChildren<PlayerSafePoint>();

            foreach (PlayerSafePoint safePoint in _safePoints)
            {
                safePoint.area = this;
                CreateBoxTrigger(safePoint);
            }
        }

        public BoxCollider2D CreateBoxTrigger(PlayerSafePoint point)
        {
            BoxCollider2D trigger = point.gameObject.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true;
            trigger.size = point.triggerSize;
            trigger.offset = point.triggerOffset;
            return trigger;
        }

        public PlayerSafePoint GetSafePoint(System.Guid safePointGUID)
        {
            for (int i = 0; i < safePoints.Length; i++)
            {
                PlayerSafePoint safePoint = safePoints[i];
                if (safePoint.guid.Equals(safePointGUID))
                    return safePoint;
            }
            return defaultPlayerPoint;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            foreach (PlayerSafePoint safePoint in transform.GetComponentsInChildren<PlayerSafePoint>())
            {
                Rect safePointTriggerBounds = new Rect();
                Vector2 safePointPosition = (Vector2)safePoint.transform.position + safePoint.triggerOffset;
                safePointTriggerBounds.size = safePoint.triggerSize;
                safePointTriggerBounds.position = safePointPosition - (safePoint.triggerSize * .5f);
                UnityEditor.Handles.DrawSolidRectangleWithOutline(safePointTriggerBounds, GizmosColor.instance.safePointArea, Color.white);
                UnityEditor.Handles.Label(safePointPosition, safePoint.name);
            }
        }
#endif
    }
}