using System;
using UnityEngine;
using UnityEngine.Events;

namespace Metroidvania.Entities {
    public class EntityTargetFinder : MonoBehaviour {
        [SerializeField] private float m_updateRate = 2;
        [SerializeField] private Vector2 m_offset;
        [SerializeField] private float m_viewRadius = 8;
        [SerializeField, Range(0, 360)] private float m_viewAngle = 90f;
        [SerializeField] private LayerMask m_obstaclesLayer = 1 << 6;

        [Header("Events")]
        [SerializeField] private UnityEvent<EntityTarget> m_targetFocused;
        [SerializeField] private UnityEvent m_targetLost;
        [SerializeField] private UnityEvent<EntityTarget> m_focusedTargetChanged;
        [SerializeField] private UnityEvent<EntityTarget> m_targetLocked;
        [SerializeField] private UnityEvent m_targetUnlocked;

        private Quaternion _cachedRotation;
        private float _rotationZ;
        private float _currentTick = float.MaxValue;
        private bool _lockedTarget;

        public Vector2 offset => m_offset;
        public float viewRadius => m_viewRadius;
        public float viewAngle => m_viewAngle;
        public LayerMask obstaclesLayer => m_obstaclesLayer;

        public UnityEvent<EntityTarget> TargetFocused => m_targetFocused;
        public UnityEvent<EntityTarget> FocusedTargetChanged => m_focusedTargetChanged;
        public UnityEvent TargetLost => m_targetLost;
        public UnityEvent<EntityTarget> TargetLocked => m_targetLocked;
        public UnityEvent TargetUnlocked => m_targetUnlocked;

        public Transform t { get; private set; }
        public Vector2 position => (Vector2)t.position + (offset * (Vector2)t.localScale);
        public Quaternion rotation => _cachedRotation;
        public float rotationZ {
            get => _rotationZ;
            set => _cachedRotation = Quaternion.Euler(0, 0, _rotationZ = value);
        }

        public int visibleTargetsCount { get; private set; } = 0;
        public EntityTarget[] visibleTargets { get; private set; } = new EntityTarget[8];
        public bool hasFocusedTarget { get; private set; } = false;
        public EntityTarget focusedTarget { get; private set; } = null;

        private void Start() {
            t = transform;
            EntitiesManager.instance.targetReleased.OnEventRaise += OnTargetRemoved;
        }

        private void LateUpdate() {
            _currentTick += Time.deltaTime;
            if (_currentTick >= m_updateRate) {
                UpdateVisibleTargets();
                if (!hasFocusedTarget)
                    FocusInNearest(false);
                _currentTick = 0;
            }
        }

        private void OnDestroy() {
            EntitiesManager.instance.targetReleased.OnEventRaise -= OnTargetRemoved;
        }

        public void UpdateVisibleTargets() {
            int i = 0;
            bool lostFocusedTarget = hasFocusedTarget;

            if (_lockedTarget) {
                lostFocusedTarget = false;
                visibleTargets[0] = focusedTarget;
                i++;
            }

            foreach (EntityTarget target in EntitiesManager.instance.targets) {
                float targetDistance = Vector2.Distance(position, target.position);
                if (targetDistance < viewRadius && IsInsideAngleView(target.position) && !IsObstructed(target.position)) {
                    visibleTargets[i] = target;
                    if (target == focusedTarget)
                        lostFocusedTarget = false;
                    i++;
                }
            }

            visibleTargetsCount = i;

            if (lostFocusedTarget)
                UnfocusTarget();
        }

        public void FocusTarget(EntityTarget target, bool force = false) {
            if (_lockedTarget && !force)
                return;

            focusedTarget = target;
            if (!hasFocusedTarget)
                TargetFocused?.Invoke(target);
            else
                FocusedTargetChanged?.Invoke(target);
            hasFocusedTarget = true;
        }

        public void UnfocusTarget(bool force = false) {
            if (_lockedTarget && !force)
                return;

            if (visibleTargetsCount == 0) {
                hasFocusedTarget = false;
                focusedTarget = null;
                TargetLost?.Invoke();
            } else {
                FocusInNearest(true);
            }
        }

        public EntityTarget GetNearestVisibleTarget() {
            EntityTarget nearest = null;
            float nearestDistance = int.MaxValue;
            for (int i = 0; i < visibleTargetsCount; i++) {
                EntityTarget target = visibleTargets[i];
                float distance = (target.position - position).sqrMagnitude;
                if (distance < nearestDistance) {
                    nearest = target;
                    nearestDistance = distance;
                }
            }
            return nearest;
        }

        public void LockFocusedTarget() {
            if (_lockedTarget || !hasFocusedTarget)
                return;

            _lockedTarget = true;
            m_targetLocked?.Invoke(focusedTarget);
        }

        public void UnlockFocusedTarget() {
            if (!_lockedTarget)
                return;

            _lockedTarget = false;
            TargetUnlocked.Invoke();
        }

        public void FocusInNearest(bool force) {
            EntityTarget nearest = GetNearestVisibleTarget();
            if (nearest)
                FocusTarget(nearest, force);
        }

        public bool IsInsideAngleView(Vector2 targetPosition) {
            float rotZ = rotation.z * 2f;
            Vector2 transformUp = new Vector2(rotation.w * -rotZ, 1f - (rotation.z * rotZ));
            Vector2 targetDir = (targetPosition - position).normalized;

            return Vector2.Angle(transformUp, targetDir) < viewAngle * .5f;
        }

        public bool IsObstructed(Vector2 targetPosition) {
            Vector2 targetDir = (targetPosition - position).normalized;
            float targetDistance = Vector2.Distance(position, targetPosition);

            return Physics2D.Raycast(position, targetDir, targetDistance, obstaclesLayer);
        }

        private void OnTargetRemoved(UnityEngine.Object obj) {
            if (!(obj is EntityTarget target))
                return;

            for (int i = 0; i < visibleTargetsCount; i++)
                if (target == visibleTargets[i])
                    RemoveTargetAt(i);

            if (target == focusedTarget)
                UnfocusTarget(true);
            _lockedTarget = false;
        }

        private void RemoveTargetAt(int index) {
            for (int i = index; i < visibleTargetsCount; i++)
                visibleTargets[i] = visibleTargets[i + 1];
            visibleTargetsCount--;
        }

#if UNITY_EDITOR
        private void OnValidate() {
            t = transform;
        }

        private void OnDrawGizmosSelected() {
            GizmosDrawer gizmos = new GizmosDrawer().SetColor(GizmosColor.instance.entities.targetFinderFindRange);

            Vector2 viewAngleA = DirFromAngle(-viewAngle * 0.5f);
            Vector2 viewAngleB = DirFromAngle(viewAngle * 0.5f);
            gizmos.DrawWireDisc(position, viewRadius);
            gizmos.DrawLine(position, position + (viewAngleA * viewRadius));
            gizmos.DrawLine(position, position + (viewAngleB * viewRadius));

            for (int i = 0; i < visibleTargetsCount; i++) {
                EntityTarget target = visibleTargets[i];
                gizmos.SetColor(target != focusedTarget ? GizmosColor.instance.entities.targetFinderVisibleTargetsLine : Color.blue);
                gizmos.DrawLine(target.position, position);
            }

            Vector2 DirFromAngle(float angleInDegrees) {
                angleInDegrees -= rotationZ;
                return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
            }
        }
#endif
    }
}
