using Metroidvania.Serialization;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace MetroidvaniaEditor.Serialization
{
    [CustomEditor(typeof(GameDataImporter))]
    public class GameDataEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            var gameDataAsset = GetGameDataAsset();
            serializedObject.Update();

            if (gameDataAsset == null)
                EditorGUILayout.HelpBox("The currently selected object is not an editable game data asset.",
                    MessageType.Info);

            using (new EditorGUI.DisabledScope(gameDataAsset == null))
            {
                if (GUILayout.Button("Edit asset"))
                    GameDataEditorWindow.OpenEditor(gameDataAsset);
            }

            serializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();
        }

        private GameDataAsset GetGameDataAsset() => assetTarget as GameDataAsset;
    }
}