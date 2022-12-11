using UnityEngine;

namespace Metroidvania.Animations {
    [System.Serializable]
    public class SpriteSheetAnimation : ISerializationCallbackReceiver {
        public string name;
        [HideInInspector] public int hash;
        public int frameRate = 12;
        public bool loop;
        public Sprite[] sheet;

        public float duration => sheet.Length / (float)frameRate;

        public void OnAfterDeserialize() {
            hash = Animator.StringToHash(name);
        }

        public void OnBeforeSerialize() {
        }
    }
}