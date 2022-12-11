using Metroidvania;
using UnityEditor;
using UnityEngine;

namespace MetroidvaniaEditor {

    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidDrawer : PropertyDrawer {
        private static readonly GUIContent k_NewGuidContent = new GUIContent("Generate New GUID");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            Rect labelRect = new Rect(position) { width = EditorGUIUtility.labelWidth };
            Rect fieldRect = EditorGUI.PrefixLabel(position, label);

            SerializedProperty guidStr = property.FindPropertyRelative("str");

            Event evt = Event.current;
            bool isMouseDown = evt.type == EventType.MouseDown;
            bool isRightClick = evt.button == 1;
            bool isInsideField = labelRect.Contains(evt.mousePosition);

            if (isMouseDown && isRightClick && isInsideField) {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(k_NewGuidContent, false, () => {
                    guidStr.stringValue = System.Guid.NewGuid().ToString("N");
                    guidStr.serializedObject.ApplyModifiedProperties();
                });
                menu.ShowAsContext();
            }

            EditorGUI.PropertyField(fieldRect, guidStr, GUIContent.none);
        }
    }
}