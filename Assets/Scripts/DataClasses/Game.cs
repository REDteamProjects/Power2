using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using Assets.Scripts.Interfaces;
using SmartLocalization;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.DataClasses
{
    public class Game : MonoBehaviour {

        public GameTypes Type;
        //public Texture2D emptyProgressBar;
        public Texture2D fullProgressBar;
        private IPlayground PlaygroundObject;
        //internal Statistics Stats;
        public static DifficultyLevel Difficulty = DifficultyLevel.easy;
        public static GameTheme _theme = GameTheme.dark;
        public static Font textFont;
        public static Font numbersFont;
        public static Int32 minLabelFontSize = 60;
        public static Int32 maxLabelFontSize = 90;
        public static float standartItemSpeed = 14;

        public static GameTheme Theme
        {
            get { return GeneralSettings.ActiveTheme; }
            set
            {
                if (GeneralSettings.ActiveTheme == value) return;
                GeneralSettings.ActiveTheme = value;
            }
        }

        void Awake()
        {
            textFont = Resources.Load<Font>("Fonts/" + LanguageManager.Instance.GetTextValue("LabelsFont"));
            numbersFont = Resources.Load<Font>("Fonts/BITALIC");
			//Stats = new Statistics(Type);
            switch(Type)
            {
                case GameTypes._6x6:
                    PlaygroundObject = gameObject.AddComponent<Mode6x6SquarePlayground>();
                    break;
                case GameTypes._8x8:
                    PlaygroundObject = gameObject.AddComponent<Mode8x8SquarePlayground>();
                    break;
                case GameTypes._rhombus:
                    PlaygroundObject = gameObject.AddComponent<Mode11RhombusPlayground>();
                    break;
                case GameTypes._match3:
                    PlaygroundObject = gameObject.AddComponent<ModeMatch3Playground>();
                    break;
                case GameTypes._drops:
                    PlaygroundObject = gameObject.AddComponent<ModeDropsPlayground>();
                    break;
            }
        }

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }
    }
}
