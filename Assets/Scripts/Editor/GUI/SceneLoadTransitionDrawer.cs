using Metroidvania.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace MetroidvaniaEditor.SceneManagement {
    [CustomPropertyDrawer(typeof(SceneLoadTransition))]
    public class SceneLoadTransitionDrawer : PropertyDrawer {
        private static readonly GUIContent k_SpawnPointDropdownLabel = new GUIContent("Spawn Point");
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GetProperties(property, out SerializedProperty channelProp, out SerializedProperty spawnPointKeyProp);
            position.height = 18;

            EditorGUI.PropertyField(position, channelProp);

            SceneChannel channel;
            if (channel = GetChannel(channelProp)) {
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                position = EditorGUI.PrefixLabel(position, k_SpawnPointDropdownLabel);

                if (EditorGUI.DropdownButton(position, new GUIContent(spawnPointKeyProp.stringValue), FocusType.Keyboard)) {
                    GenericMenu menu = new GenericMenu();
                    foreach (SceneSpawnPoints.SceneSpawnPoint spawnPoint in channel.spawnPoints.spawnPoints) {
                        menu.AddItem(new GUIContent(spawnPoint.key), spawnPoint.key == spawnPointKeyProp.stringValue, () => {
                            spawnPointKeyProp.stringValue = spawnPoint.key;
                            spawnPointKeyProp.serializedObject.ApplyModifiedProperties();
                        });
                    }
                    menu.ShowAsContext();
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            SerializedProperty channelProp = property.FindPropertyRelative("channel");

            return !GetChannel(channelProp) ? EditorGUIUtility.singleLineHeight : (EditorGUIUtility.singleLineHeight * 2) + EditorGUIUtility.standardVerticalSpacing;
        }

        private void GetProperties(SerializedProperty property, out SerializedProperty channel, out SerializedProperty spawnPoint) {
            channel = property.FindPropertyRelative(nameof(channel));
            spawnPoint = property.FindPropertyRelative(nameof(spawnPoint));
        }

        private SceneChannel GetChannel(SerializedProperty referenceProperty) {
            SerializedProperty channelAssetGUID = referenceProperty.FindPropertyRelative("m_AssetGUID");
            return AssetDatabase.LoadAssetAtPath<SceneChannel>(AssetDatabase.GUIDToAssetPath(channelAssetGUID.stringValue));
        }
    }
}