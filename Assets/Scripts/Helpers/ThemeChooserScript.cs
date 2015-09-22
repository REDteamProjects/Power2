using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

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
            GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor =
                                GameColors.BackgroundColor;
            //var buttonsCollection = GameObject.Find("GUI").GetComponentsInChildren<Button>(); //TODO: need to load right buttons
            //foreach (var button in buttonsCollection)
            //{
            //    button.colors.normalColor.a = 2;
            //}
        }
    }
}
