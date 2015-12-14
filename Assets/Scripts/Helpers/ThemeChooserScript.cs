﻿using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Helpers
{
    public class ThemeChooserScript : MonoBehaviour
    {
        public void OnGameThemeChange()
        {
            GameThemeChange();
        }

        public static void OnScriptGameThemeChange()
        {
            GameThemeChange();
        }

        private static void GameThemeChange()
        {
            switch (Game.Theme)
            {
                case GameTheme.light:
                    Game.Theme = GameTheme.dark;
                    break;
                case GameTheme.dark:
                    Game.Theme = GameTheme.additional_1;
                    break;
                case GameTheme.additional_1:
                    Game.Theme = GameTheme.light;
                    break;
            }

            MainMenuScript.UpdateTheme();
        }
    }
}
