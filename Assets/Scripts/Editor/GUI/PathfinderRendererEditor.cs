using UnityEngine;
using UnityEditor;
using Metroidvania.Pathfinding;

namespace MetroidvaniaEditor.Pathfinding
{
    [CustomEditor(typeof(PathfinderRenderer))]
    public class PathfinderRendererEditor : Editor
    {
        private PathfinderRenderer code;

        private SerializedProperty m_DrawGizmos;
        private SerializedProperty m_NodeTransparency;
        private SerializedProperty m_WalkableColor;
        private SerializedProperty m_UnWalkableColor;

        private SerializedProperty m_LineTransparency;
        private SerializedProperty m_LineColor;

        private void OnEnable()
        {
            code = target as PathfinderRenderer;

            m_DrawGizmos = serializedObject.FindProperty(nameof(m_DrawGizmos));
            m_NodeTransparency = serializedObject.FindProperty(nameof(m_NodeTransparency));
            m_WalkableColor = serializedObject.FindProperty(nameof(m_WalkableColor));
            m_UnWalkableColor = serializedObject.FindProperty(nameof(m_UnWalkableColor));
            m_LineTransparency = serializedObject.FindProperty(nameof(m_LineTransparency));
            m_LineColor = serializedObject.FindProperty(nameof(m_LineColor));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_DrawGizmos);

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(m_NodeTransparency);
                EditorGUILayout.PropertyField(m_WalkableColor);
                EditorGUILayout.PropertyField(m_UnWalkableColor);

                if (scope.changed)
                    code.UpdateColors();

                EditorGUILayout.PropertyField(m_LineTransparency);
                EditorGUILayout.PropertyField(m_LineColor);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}