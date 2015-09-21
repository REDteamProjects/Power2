using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class ThemeChooserScript : MonoBehaviour
    {
        public void OnGameThemeChange()
        {
            switch (Game.Theme)
            {
                case GameTheme.dark:
                    Game.Theme = GameTheme.light;
                    break;
                case GameTheme.light:
                    Game.Theme = GameTheme.dark;
                    break;
            }
        }
    }
}
