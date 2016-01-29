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
        private static IPlayground PlaygroundObject;
        //internal Statistics Stats;
        public static DifficultyLevel Difficulty = DifficultyLevel._easy;
        public static bool isExtreme = false;
        public static Font textFont;
        public static Font numbersFont;
        public static float _standartItemSpeed = 22;
        public static GameObject Background;
        public static GameObject Middleground;
        public static GameObject Foreground;

        public static float StandartItemSpeed
        {
            get { return _standartItemSpeed * (PlaygroundObject == null ? 1f : PlaygroundObject.ItemSpeedMultiplier); }
        }

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
            Background = GameObject.Find("/Middleground/Background");
            Middleground = GameObject.Find("/Middleground");
            Foreground = GameObject.Find("/Foreground");
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
