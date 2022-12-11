using Metroidvania.Serialization;
using Metroidvania.Serialization.Handlers;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace MetroidvaniaEditor.Serialization {
    [CustomEditor(typeof(GameDataImporter))]
    public class GameDataEditor : ScriptedImporterEditor {
        private static GUIStyle s_LabelStyle => EditorStyles.whiteLabel;

        private GUIContent _gameDataJsonContent;
        private Vector2 _previewScrollPosition;

        public override void OnEnable() {
            base.OnEnable();
            string assetPath = AssetDatabase.GetAssetPath(assetTarget);
            string assetFileJson = File.ReadAllText(assetPath);
            string decryptedJson = DataHandler.EncryptDecrypt(assetFileJson);
            string formattedJson = JsonUtility.ToJson(JsonUtility.FromJson(decryptedJson, typeof(GameData)), true);
            _gameDataJsonContent = new GUIContent(formattedJson);
        }

        public override void OnInspectorGUI() {
            GameDataAsset gameDataAsset = GetGameDataAsset();
            serializedObject.Update();

            if (gameDataAsset == null)
                EditorGUILayout.HelpBox("The currently selected object is not an editable game data asset.", MessageType.Info);

            using (new EditorGUI.DisabledScope(gameDataAsset == null)) {
                if (GUILayout.Button("Edit asset"))
                    GameDataEditorWindow.OpenEditor(gameDataAsset);
            }

            serializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();
        }

        public override void DrawPreview(Rect previewArea) {
            Vector2 size = s_LabelStyle.CalcSize(_gameDataJsonContent);
            Rect viewRect = new Rect(0, 0, size.x, size.y);

            _previewScrollPosition = GUI.BeginScrollView(previewArea, _previewScrollPosition, viewRect);
            EditorGUI.SelectableLabel(viewRect, _gameDataJsonContent.text, s_LabelStyle);
            GUI.EndScrollView();
        }

        public override bool HasPreviewGUI() => GetGameDataAsset();

        private GameDataAsset GetGameDataAsset() => assetTarget as GameDataAsset;
    }
}