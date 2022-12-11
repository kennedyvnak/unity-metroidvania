using Metroidvania;
using Metroidvania.Characters.SafePoints;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace MetroidvaniaEditor.CharacterSafePoints {
    [CustomEditor(typeof(CharacterSafePoint), true)]
    public class CharacterSafePointEditor : Editor {
        private CharacterSafePoint _code;

        private BoxBoundsHandle _boundsHandle;

        private SerializedProperty _triggerSize, _triggerOffset, _position;

        private void OnEnable() {
            _code = target as CharacterSafePoint;
            _triggerSize = serializedObject.FindProperty("m_triggerSize");
            _position = serializedObject.FindProperty("m_position");
            _triggerOffset = serializedObject.FindProperty("m_triggerOffset");

            _boundsHandle = new BoxBoundsHandle {
                axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y
            };
            _boundsHandle.SetColor(GizmosColor.instance.safePoints.handles);
        }

        private void OnSceneGUI() {
            _boundsHandle.center = _triggerOffset.vector2Value + (Vector2)_code.transform.position;
            _boundsHandle.size = _triggerSize.vector2Value;
            _boundsHandle.DrawHandle();
            _triggerOffset.vector2Value = _boundsHandle.center - _code.transform.position;
            _triggerSize.vector2Value = _boundsHandle.size;

            Handles.color = GizmosColor.instance.safePoints.handles;
            _position.vector2Value = Handles.FreeMoveHandle(_position.vector2Value, Quaternion.identity, 0.25f, Vector3.zero, Handles.DotHandleCap);

            if (serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}