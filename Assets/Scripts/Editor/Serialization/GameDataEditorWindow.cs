using Metroidvania.Serialization;
using Metroidvania.Serialization.Handlers;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace MetroidvaniaEditor.Serialization {
    public class GameDataEditorWindow : EditorWindow {
        [SerializeField] private GameDataAsset m_asset;
        [SerializeField] private GameData m_editingData;

        private static readonly GUIContent k_EditingDataContent = new GUIContent("Editing Data");

        private SerializedObject so { get; set; }
        private SerializedProperty editingDataProp { get; set; }

        private const string k_FileExtension = "." + DataHandler.FileExtension;

        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceId, int line) {
            string path = AssetDatabase.GetAssetPath(instanceId);
            if (!path.EndsWith(k_FileExtension, StringComparison.InvariantCultureIgnoreCase))
                return false;

            UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceId);
            if (obj is not GameDataAsset asset)
                return false;
            OpenEditor(asset);
            return true;
        }

        public static void OpenEditor(GameDataAsset asset) {
            GameDataEditorWindow window = GetWindow<GameDataEditorWindow>();
            window.titleContent = new GUIContent(asset.name + " (Game Data)");
            window.minSize = new Vector2(450, 600);
            window.m_asset = asset;
            window.m_editingData = window.m_asset.gameData.Clone();
            window.Show();
        }

        private void OnEnable() {
            so = new SerializedObject(this);
            editingDataProp = so.FindProperty(nameof(m_editingData));
        }

        private void OnDisable() {
            so.Dispose();
        }

        private void OnGUI() {
            so.Update();
            DrawToolbar();

            SerializedProperty iterator = editingDataProp.Copy();
            iterator.Next(true);
            EditorGUILayout.PropertyField(iterator);
            while (iterator.Next(false))
                EditorGUILayout.PropertyField(iterator);

            if (so.ApplyModifiedPropertiesWithoutUndo())
                hasUnsavedChanges = true;
        }

        private void DrawToolbar() {
            GUILayout.BeginHorizontal("toolbar");
            using (new EditorGUI.DisabledScope(!hasUnsavedChanges)) {
                if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                    SaveChanges();
            }
            GUILayout.EndHorizontal();
        }

        public override void SaveChanges() {
            base.SaveChanges();
            string path = AssetDatabase.GetAssetPath(m_asset);
            string json = JsonUtility.ToJson(m_editingData);
            File.WriteAllText(path, DataHandler.EncryptDecrypt(json));
            m_asset.LoadFromJson(json);
        }

        public override void DiscardChanges() {
            base.DiscardChanges();
        }
    }
}