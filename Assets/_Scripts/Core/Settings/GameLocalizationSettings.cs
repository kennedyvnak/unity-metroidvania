using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Metroidvania.Events;

namespace Metroidvania.Localization
{
    public class GameLocalizationSettings : ScriptableSingleton<GameLocalizationSettings>
    {
        public string localizationPrefsKey = "Selected-Locale";

        public IntEventChannel changeLocaleChannel;

        public void LoadPrefs()
        {
            SetLocale(PlayerPrefs.GetInt(localizationPrefsKey, 0), false);
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
                changeLocaleChannel?.Raise(localeIdx);
            }
        }

        public void StepLocale() =>
            SetLocale((PlayerPrefs.GetInt(localizationPrefsKey, 0) + 1)
                      % LocalizationSettings.AvailableLocales.Locales.Count, true);
    }
}