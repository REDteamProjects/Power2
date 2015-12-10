using System;
using System.Linq;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using SmartLocalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class MainMenuScript : MonoBehaviour
    {
        public MenuState CurrentState;
        private static GameObject _soundButton;
        private static GameObject _mainCamera;
        private const int _toOpenLevelStep = 65536;
        private List<string> _availableScenes = new List<string>();

        public static void UpdateTheme()
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor =
                                GameColors.BackgroundColor;
            var bg = GameObject.Find("BackgroundGrid");
            if (bg != null)
            {
                bg.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("SD/" + bg.GetComponent<Image>().sprite.name.Split('_')[0]/*"SD/6x6Atlas"*/)
                  .SingleOrDefault(t => t.name.Contains(Game.Theme.ToString()));
            }
            /*var changeObject = GameObject.Find("StatsButton");
            if (changeObject != null)
            {
                changeObject.GetComponent<Image>().color = GameColors.ForegroundButtonsColor;
            }

            changeObject = GameObject.Find("HelpButton");
            if (changeObject != null)
            {
                changeObject.GetComponent<Image>().color = GameColors.ForegroundButtonsColor;
            }

            changeObject = GameObject.Find("SoundButton");
            if (changeObject != null)
            {
                changeObject.GetComponent<Image>().color = GameColors.ForegroundButtonsColor;
            }

            changeObject = GameObject.Find("AboutButton");
            if (changeObject != null)
            {
                changeObject.GetComponent<Image>().color = GameColors.ForegroundButtonsColor;
            }*/
        }

        void Awake()
        {
            Game.textFont = Resources.Load<Font>("Fonts/SEGUIBL");
            Game.numbersFont = Game.textFont;
            _mainCamera = GameObject.Find("Main Camera");
            var fg = GameObject.Find("/GUI");
            var statsButton = GameObject.Find("/GUI/StatsButton");
            _soundButton = GenerateMenuButton("Prefabs/SoundButton", fg.transform, Vector3.one, new Vector3(-statsButton.transform.localPosition.x,
                statsButton.transform.localPosition.y, statsButton.transform.localPosition.z), null, 0, OnSoundButtonPressed);
            _soundButton.GetComponent<Image>().sprite = GeneralSettings.SoundButtonSprite;
            _soundButton.name = "SoundButton";
            if (SoundEnabled == SoundState.on)      
                _mainCamera.GetComponent<AudioSource>().Play();

            UpdateTheme();
            _availableScenes.Add("6x6");
            if (GameSettingsHelper<Mode6x6SquarePlayground>.Preferenses.ScoreRecord < _toOpenLevelStep)
            {
                CloseLevelGUI("8x8", "6x6", _toOpenLevelStep - GameSettingsHelper<Mode6x6SquarePlayground>.Preferenses.ScoreRecord);
            }
            else
                _availableScenes.Add("8x8");
            if(GameSettingsHelper<Mode8x8SquarePlayground>.Preferenses.ScoreRecord < _toOpenLevelStep)
            {
                CloseLevelGUI("Match3", "8x8", _toOpenLevelStep - GameSettingsHelper<Mode8x8SquarePlayground>.Preferenses.ScoreRecord);
            }
            else
                _availableScenes.Add("Match3");
            if (GameSettingsHelper<ModeMatch3SquarePlayground>.Preferenses.ScoreRecord < _toOpenLevelStep)
            {
                CloseLevelGUI("Rhombus", "Match3", _toOpenLevelStep - GameSettingsHelper<ModeMatch3SquarePlayground>.Preferenses.ScoreRecord);
            }
            else
                _availableScenes.Add("11Rhombus");
        }


        private void CloseLevelGUI(String level, String previos, Int32 pointsLeft)
        {
            var obj = GameObject.Find("/GUI/MainBlock/" + level);
            var img = obj.GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
            var label = Instantiate(Resources.Load("Prefabs/ShortLabel")) as GameObject;
            label.transform.SetParent(obj.transform);
            label.transform.localPosition = new Vector3(15, 0, -4);
            var labelText = label.GetComponent<Text>();
            labelText.font = Game.textFont;
            labelText.color = GameColors.ModesColors[previos];
            labelText.fontSize = LabelShowing.minLabelFontSize;
            labelText.text = pointsLeft.ToString();
            labelText.alignment = TextAnchor.MiddleRight;
            var modeLogo = Instantiate(Resources.Load("Prefabs/ModeLogo")) as GameObject;
            var newsprite = Resources.LoadAll<Sprite>("SD/ModesButtonsAtlas")
            .SingleOrDefault(t => t.name.Contains(previos));
            modeLogo.GetComponent<SpriteRenderer>().sprite = newsprite;
            modeLogo.transform.SetParent(obj.transform);
            modeLogo.transform.localPosition = new Vector3(60, -2, -4);
        }

        public SoundState SoundEnabled
        {
            get
            {
                return GeneralSettings.SoundEnabled;
            }
            set
            {
                GeneralSettings.SoundEnabled = value;
                switch (GeneralSettings.SoundEnabled)
                {
                    case SoundState.on:
                        _mainCamera.GetComponent<AudioSource>().Play();
                        return;
                }
                _mainCamera.GetComponent<AudioSource>().Pause();
            }
        }

        public void OnSoundButtonPressed()
        {
            SoundEnabled+=1;
            Vibration.Vibrate();
            _soundButton.GetComponent<Image>().sprite = GeneralSettings.SoundButtonSprite;
        }

        public void OnNavigationButtonClick(String scene)
        {
            if (scene != "Statistics" && scene != "Help" && scene != "About" && !_availableScenes.Contains(scene)) return;
            Vibration.Vibrate();

            var gui = GameObject.Find("/GUI");
            var collection = gui.GetComponentsInChildren<Transform>();
            foreach (var component in collection)
            {
                if (gui != component.gameObject)
                    Destroy(component.gameObject);
            }
            var label = Instantiate(Resources.Load("Prefabs/LoadingLabel")) as GameObject;
            label.transform.SetParent(gui.transform);
            label.transform.localPosition = Vector3.zero;
            label.transform.localScale = Vector3.one;
            var txt = label.GetComponent<Text>();
            txt.text = LanguageManager.Instance.GetTextValue("LoadingTitle");
            txt.color = GameColors.ForegroundButtonsColor;
            Game.Difficulty = DifficultyLevel._easy;
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

            var buttonComponent = button.GetComponent<Button>();
            if (buttonComponent != null)
            {
                if (onClickCall != null)
                    buttonComponent.onClick.AddListener(onClickCall);
                else
                    buttonComponent.interactable = false;
            }


            if (String.IsNullOrEmpty(buttonText)) return button;

            var textComponent = button.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                textComponent.text = buttonText;
                textComponent.fontSize = textSize;
                textComponent.font = Game.textFont;
                if (buttonColor.HasValue) textComponent.color = buttonColor.Value;
            }

            return button;
        }
    }
}
