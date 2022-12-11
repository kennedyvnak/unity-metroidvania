using Metroidvania.Events;
using Metroidvania.Settings;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Metroidvania.Audio {
    public class GameAudioSettings : ScriptableSingleton<GameAudioSettings>, IInitializableSingleton {
        public enum Group {
            Master,
            Musics,
            Sfx,
            None,
        }

        public string masterFieldName = "Master Volume",
            musicsFieldName = "Musics Volume",
            sfxFieldName = "Sfx Volume";

        [Header("Mixers")]
        public AudioMixer audioMixer;

        [Header("Groups")]
        public AudioMixerGroup masterGroup;
        public AudioMixerGroup musicsGroup;
        public AudioMixerGroup sfxGroup;

        [Header("Snapshots")]
        public AudioMixerSnapshot pausedSnapshot;
        public AudioMixerSnapshot unpausedSnapshot;

        [Header("Events")]
        public FloatEventChannel masterVolumeChangedChannel;
        public FloatEventChannel musicsVolumeChangedChannel;
        public FloatEventChannel sfxVolumeChangedChannel;

        public void Initialize() {
            SetAudioField(masterFieldName, PlayerPrefs.GetFloat(masterFieldName, 1), masterVolumeChangedChannel, false);
            SetAudioField(musicsFieldName, PlayerPrefs.GetFloat(musicsFieldName, 1), musicsVolumeChangedChannel, false);
            SetAudioField(sfxFieldName, PlayerPrefs.GetFloat(sfxFieldName, 1), sfxVolumeChangedChannel, false);
        }

        public void SetAudioField(string fieldName, float source, FloatEventChannel eventChannel, bool setPrefs) {
            audioMixer.SetFloat(fieldName, CalculateAudio(source));
            if (setPrefs)
                PlayerPrefs.SetFloat(fieldName, source);
            eventChannel?.Raise(source);
        }

        public AudioMixerGroup GetGroup(Group group) => group switch {
            Group.Master => masterGroup,
            Group.Musics => musicsGroup,
            Group.Sfx => sfxGroup,
            Group.None => null,
            _ => throw new ArgumentOutOfRangeException(nameof(group), group, null)
        };

        public void SetMasterField(float source) => SetAudioField(masterFieldName, source, masterVolumeChangedChannel, true);

        public void SetMusicsField(float source) => SetAudioField(musicsFieldName, source, musicsVolumeChangedChannel, true);

        public void SetSfxField(float source) => SetAudioField(sfxFieldName, source, sfxVolumeChangedChannel, true);

        public static float CalculateAudio(float source) => Mathf.Log10(source) * 20;
    }
}