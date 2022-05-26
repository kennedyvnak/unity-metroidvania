using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Metroidvania.Localization
{
    [ResourceObjectPath("Settings/Localization Settings")]
    public class GameLocalizationSettings : ScriptableSingleton<GameLocalizationSettings>
    {
        public string localizationPrefsKey = "Selected-Locale";

        public static event System.Action<Locale> SelectedLocaleChange;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void LoadPrefs()
        {
            var i = instance;
            i.SetLocale(PlayerPrefs.GetInt(i.localizationPrefsKey, 0), false);
        }

        public void SetLocale(int localeIdx, bool setPrefs)
        {
            if (!LocalizationSettings.InitializationOperation.IsDone)
                LocalizationSettings.InitializationOperation.Completed += _ => DoStep();
            else DoStep();

            void DoStep()
            {
                localeIdx = Mathf.Clamp(localeIdx, 0, LocalizationSettings.AvailableLocales.Locales.Count - 1);
                Locale locale = LocalizationSettings.AvailableLocales.Locales[localeIdx];
                if (locale == LocalizationSettings.SelectedLocale) return;

                LocalizationSettings.SelectedLocale = locale;
                if (setPrefs)
                    PlayerPrefs.SetInt(localizationPrefsKey, localeIdx);
                SelectedLocaleChange?.Invoke(locale);
            }
        }

        public void StepLocale() =>
            SetLocale((PlayerPrefs.GetInt(localizationPrefsKey, 0) + 1)
                      % LocalizationSettings.AvailableLocales.Locales.Count, true);
    }
}