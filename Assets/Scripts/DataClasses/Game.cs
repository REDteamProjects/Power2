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
        public static DifficultyLevel Difficulty = DifficultyLevel._easy;
        public static GameTheme _theme = GameTheme.dark;
        public static Font textFont;
        public static Font numbersFont;
        public static float standartItemSpeed = 24;

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
                    PlaygroundObject = gameObject.AddComponent<ModeMatch3SquarePlayground>();
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
