using System;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MainMenuScript : MonoBehaviour
    {

        //public static bool IsSoundOn { get; set; }
        public MenuState CurrentState;
        //private GUISkin skin;
        private GameObject _rhombusbutton;
        private GameObject _dropsbutton;
        private GameObject _match3button;
        private GameObject _8x8button;
        private GameObject _6x6button;
        private GameObject _backButton;
        private GameObject _newgamebutton;
        //private GameObject _statbutton;
        //private GameObject _aboutbutton;

        private GameObject _easyButton;
        private GameObject _mediumButton;
        private GameObject _hardButton;
        private GameObject _veryhardButton;

        // Use this for initialization
        void Start()
        {
            //skin = Resources.Load("MenuButtons") as GUISkin;
            //var currentResolution = Screen.currentResolution;
            //Camera.main.aspect = (float)currentResolution.width / currentResolution.height;
        }

        // Update is called once per frame
        void Update()
        {
            if (Application.platform != RuntimePlatform.Android) return;
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            OnBackButtonPress();
        }

        void Awake()
        {
            CreateMainButtons();
        }

        void CreateMainButtons()
        {
            var fg = GameObject.Find("/GUI");
            _newgamebutton = GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, -280, 0), "Start Game", 50,
                OnNewGameButtonClick);
            //_statbutton = GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, -180, 0), "STATISTICS",
            //    () => OnNavigationButtonClick("Statistics"));
            //_aboutbutton = GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, -260, 0), "ABOUT",
            //    () => OnNavigationButtonClick("About"));
        }

        public void OnBackButtonPress()
        {
            switch (CurrentState)
            {
                case MenuState.ChooseMode:
                    CurrentState = MenuState.MainMenu;

                    Destroy(_6x6button);
                    Destroy(_8x8button);
                    Destroy(_rhombusbutton);
                    Destroy(_dropsbutton);
                    Destroy(_match3button);
                    Destroy(_backButton);

                    var logo = GameObject.Find("/GUI/Image");
                    var imageRectTransform = logo.GetComponent<Image>().rectTransform;

                    imageRectTransform.localScale = new Vector3(0.7f, 0.7f, 0);
                    imageRectTransform.localPosition = new Vector3(imageRectTransform.localPosition.x, imageRectTransform.localPosition.y - 150, 
                        imageRectTransform.localPosition.z);

                    CreateMainButtons();

                    break;
                case MenuState.ChooseLevel:
                    OnNewGameButtonClick();
                    break;
                case MenuState.MainMenu:
                    Application.Quit();
                    break;
            }
        }

        public void OnModeSelect(String scene, DifficultyLevel maximumLevel)
        {
            switch (CurrentState)
            {
                case MenuState.ChooseMode:
                    Destroy(_6x6button);
                    Destroy(_8x8button);
                    Destroy(_rhombusbutton);
                    Destroy(_dropsbutton);
                    Destroy(_match3button);
                    break;
            }
            CurrentState = MenuState.ChooseLevel;

            var fg = GameObject.Find("/GUI");

            _easyButton = GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, 0, 0), "NEWBIE", 50,
                ((maximumLevel >= DifficultyLevel.easy) ? () => { Game.Difficulty = DifficultyLevel.easy; OnNavigationButtonClick(scene); } 
                : (UnityAction)null), (maximumLevel >= DifficultyLevel.easy ? (Color?)null : Color.gray));
            _mediumButton = GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, -80, 0), "PLAYER", 50,
                ((maximumLevel >= DifficultyLevel.medium) ? () => { Game.Difficulty = DifficultyLevel.medium; OnNavigationButtonClick(scene); } 
                : (UnityAction)null), maximumLevel >= DifficultyLevel.easy ? (Color?)null : Color.gray);
            _hardButton = GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, -160, 0), "FIGHTER", 50,
                ((maximumLevel >= DifficultyLevel.hard) ? () => { Game.Difficulty = DifficultyLevel.hard; OnNavigationButtonClick(scene); } 
                : (UnityAction)null), maximumLevel >= DifficultyLevel.easy ? (Color?)null : Color.gray);
            _veryhardButton = GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, -240, 0), "PSYCHO", 50,
                ((maximumLevel >= DifficultyLevel.veryhard) ? () => { Game.Difficulty = DifficultyLevel.veryhard; OnNavigationButtonClick(scene); } 
                : (UnityAction)null), maximumLevel >= DifficultyLevel.easy ? (Color?)null : Color.gray);
        }

        public void OnNewGameButtonClick()
        {
            switch (CurrentState)
            {
               case MenuState.MainMenu:
                    Destroy(_newgamebutton);
                    //Destroy(_statbutton);
                    //Destroy(_aboutbutton);
                    
                    var logo = GameObject.Find("/GUI/Image");
                    var imageRectTransform = logo.GetComponent<Image>().rectTransform;
                    imageRectTransform.localScale = new Vector3(0.5f, 0.5f, 0);
                    imageRectTransform.localPosition = new Vector3(imageRectTransform.localPosition.x, imageRectTransform.localPosition.y + 150, imageRectTransform.localPosition.z);
                    break;
                case MenuState.ChooseLevel:
                    Destroy(_easyButton);
                    Destroy(_mediumButton);
                    Destroy(_hardButton);
                    Destroy(_veryhardButton);
                    break;
            }
            CurrentState = MenuState.ChooseMode;

            var statsButton = GameObject.Find("/GUI/StatsButton");
            var fg = GameObject.Find("/GUI");

            _6x6button = GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, 0, 0), "6 X 6", 50,
                () => OnModeSelect("6x6", GameSettingsHelper<Mode6x6SquarePlayground>.Preferenses.MaximumOpenedLevel));
            _8x8button = GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, -80, 0), "8 x 8", 50,
                () => OnModeSelect("8x8", GameSettingsHelper<Mode8x8SquarePlayground>.Preferenses.MaximumOpenedLevel));
            _rhombusbutton = GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, -160, 0), "RHOMBUS", 50,
                () => OnModeSelect("11Rhombus", GameSettingsHelper<Mode11RhombusPlayground>.Preferenses.MaximumOpenedLevel));
            _dropsbutton = GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, -240, 0), "DROPS", 50,
                () => OnModeSelect("Drops", GameSettingsHelper<ModeDropsPlayground>.Preferenses.MaximumOpenedLevel));
            _match3button = GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, -320, 0), "MATCH3", 50,
                () => OnModeSelect("Match3", GameSettingsHelper<ModeMatch3Playground>.Preferenses.MaximumOpenedLevel));
            _backButton = GenerateMenuButton("Prefabs/BackButton", fg.transform, Vector3.one, new Vector3(-statsButton.transform.localPosition.x, 
                statsButton.transform.localPosition.y, statsButton.transform.localPosition.z), null, 0,
                OnBackButtonPress);

        }

        public void OnNavigationButtonClick(String scene)
        {
            Application.LoadLevel(scene);
        }

        public static GameObject GenerateMenuButton(String prefabName, Transform parentTransform, Vector3 localScale, Vector3 localPosition, String buttonText
            , int textSize, UnityAction onClickCall, Color? buttonColor = null)
        {
            var button = Instantiate(Resources.Load(prefabName)) as GameObject;
            var rectTransform = button.transform as RectTransform;
            rectTransform.SetParent(parentTransform);
            rectTransform.localScale = localScale;
            rectTransform.localPosition = localPosition;

            if (onClickCall != null)
            {
                var buttonComponent = button.GetComponent<Button>();
                if (buttonComponent != null)
                    buttonComponent.onClick.AddListener(onClickCall);
            }

            if (String.IsNullOrEmpty(buttonText)) return button;

            var textComponent = button.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                textComponent.text = buttonText;
                textComponent.fontSize = textSize;
                if (buttonColor.HasValue) textComponent.color = buttonColor.Value;
            }

            return button;
        }

        //void OnGUI()
        //{
            //GUILayout.Label("Power2");
            //http://habrahabr.ru/post/246605/
            // Set the skin to use
            //GUI.skin = skin;
        //}
    }
}
