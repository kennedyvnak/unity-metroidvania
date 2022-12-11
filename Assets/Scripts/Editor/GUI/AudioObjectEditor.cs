using Metroidvania.Audio;
using UnityEditor;
using UnityEngine;

namespace MetroidvaniaEditor {
    [CustomEditor(typeof(AudioObject))]
    public class AudioObjectEditor : Editor {
        [SerializeField] private AudioSource _previewer;

        public void OnEnable() {
            _previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
        }

        public void OnDisable() {
            DestroyImmediate(_previewer.gameObject);
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
            if (GUILayout.Button("Preview")) {
                AudioObject audio = (AudioObject)target;
                _previewer.clip = audio.clip;

                _previewer.volume = audio.volume.RandomRange();
                _previewer.pitch = audio.pitch.RandomRange();

                _previewer.priority = audio.priority;
                _previewer.loop = audio.loop;

                _previewer.spatialBlend = 0;

                _previewer.minDistance = audio.minDistance;
                _previewer.maxDistance = audio.maxDistance;
                _previewer.Play();
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
