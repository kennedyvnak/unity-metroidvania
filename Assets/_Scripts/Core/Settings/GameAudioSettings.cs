using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Metroidvania.Audio
{
    [ResourceObjectPath("Settings/Audio Settings")]
    public class GameAudioSettings : ScriptableSingleton<GameAudioSettings>
    {
        public enum Group
        {
            Master,
            Musics,
            Sfx,
            None,
        }

        public string masterFieldName = "Master Volume",
            musicsFieldName = "Musics Volume",
            sfxFieldName = "Sfx Volume";

        public static event Action<string, float> AudioChanged;

        [Header("Mixers")] public AudioMixer audioMixer;

        [Header("Groups")] public AudioMixerGroup masterGroup;
        public AudioMixerGroup musicsGroup;
        public AudioMixerGroup sfxGroup;

        [Header("Snapshots")] public AudioMixerSnapshot pausedSnapshot;
        public AudioMixerSnapshot unpausedSnapshot;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void LoadPrefs()
        {
            var i = instance;
            i.SetAudioField(i.masterFieldName, PlayerPrefs.GetFloat(i.masterFieldName, 1), false);
            i.SetAudioField(i.musicsFieldName, PlayerPrefs.GetFloat(i.musicsFieldName, 1), false);
            i.SetAudioField(i.sfxFieldName, PlayerPrefs.GetFloat(i.sfxFieldName, 1), false);
        }

        public void SetAudioField(string fieldName, float source, bool setPrefs)
        {
            audioMixer.SetFloat(fieldName, CalculateAudio(source));
            if (setPrefs)
                PlayerPrefs.SetFloat(fieldName, source);
            AudioChanged?.Invoke(fieldName, source);
        }

        public AudioMixerGroup GetGroup(Group group) => group switch
        {
            Group.Master => masterGroup,
            Group.Musics => musicsGroup,
            Group.Sfx => sfxGroup,
            Group.None => null,
            _ => throw new ArgumentOutOfRangeException(nameof(group), group, null)
        };

        public void SetMasterField(float source) => SetAudioField(masterFieldName, source, true);

        public void SetMusicsField(float source) => SetAudioField(musicsFieldName, source, true);

        public void SetSfxField(float source) => SetAudioField(sfxFieldName, source, true);

        public static float CalculateAudio(float source) => Mathf.Log10(source) * 20;
    }
}