using SmartLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Helpers
{
    class LanguageHelper
    {
        public static void ActivateSystemLanguage()
        {
            var languageManager = LanguageManager.Instance;

            var deviceCulture = languageManager.GetDeviceCultureIfSupported();
            if (deviceCulture != null)
            {
                languageManager.ChangeLanguage(deviceCulture);
            }
        }
    }
}
