using Metroidvania;
using Metroidvania.Player.SafePoints;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace MetroidvaniaEditor.SafePlayerPoints
{
    [CustomEditor(typeof(PlayerSafePoint), true)]
    public class PlayerSafePointEditor : Editor
    {
        private PlayerSafePoint _code;

        private BoxBoundsHandle _boundsHandle;

        private SerializedProperty _triggerSize, _triggerOffset, _relativePoint;

        private void OnEnable()
        {
            _code = target as PlayerSafePoint;
            _triggerSize = serializedObject.FindProperty("m_triggerSize");
            _relativePoint = serializedObject.FindProperty("m_relativePoint");
            _triggerOffset = serializedObject.FindProperty("m_triggerOffset");

            _boundsHandle = new BoxBoundsHandle
            {
                axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y
            };
            _boundsHandle.SetColor(GizmosColor.instance.safePointArea);
        }

        private void OnSceneGUI()
        {
            _boundsHandle.center = _triggerOffset.vector2Value + (Vector2)_code.transform.position;
            _boundsHandle.size = _triggerSize.vector2Value;
            _boundsHandle.DrawHandle();
            _triggerOffset.vector2Value = _boundsHandle.center - _code.transform.position;
            _triggerSize.vector2Value = _boundsHandle.size;

            Handles.color = GizmosColor.instance.safePointArea;
            _relativePoint.vector2Value = Handles.FreeMoveHandle(_relativePoint.vector2Value, _code.transform.rotation, .25f, Vector3.one, Handles.DotHandleCap);

            if (serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}