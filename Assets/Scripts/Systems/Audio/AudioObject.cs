using UnityEngine;

namespace Metroidvania.Audio {
    [CreateAssetMenu(fileName = "New Audio Object", menuName = "Scriptables/Audio/Audio Object")]
    public class AudioObject : ScriptableObject {
        public AudioClip clip;
        public GameAudioSettings.Group group = GameAudioSettings.Group.Sfx;

        [Space, RangedValue(0, 1)] public RangedFloat volume = new RangedFloat(.75f, .9f);
        [RangedValue(0, 2)] public RangedFloat pitch = new RangedFloat(.9f, 1.1f);

        [Space, Range(0, 256)] public int priority = 128;
        public bool loop;

        [Space, Range(0, 1)] public float spatialBlend = 1;

        [Space] public float minDistance = 1;
        public float maxDistance = 500;

        public void CloneToSource(AudioSource source) {
            if (!source)
                throw new System.ArgumentNullException(nameof(source));
            source.clip = clip;
            source.outputAudioMixerGroup = GameAudioSettings.instance.GetGroup(group);
            source.volume = volume.RandomRange();
            source.pitch = pitch.RandomRange();
            source.priority = priority;
            source.loop = loop;
            source.spatialBlend = spatialBlend;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
        }

#if UNITY_EDITOR
        private void OnValidate() {
            maxDistance = Mathf.Clamp(maxDistance, 0, float.MaxValue);
            minDistance = Mathf.Clamp(minDistance, 0, maxDistance);

            volume.min = Mathf.Clamp(volume.min, 0, volume.max);
            volume.max = Mathf.Clamp(volume.max, volume.min, 1);
        }
#endif
    }
}