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
        private List<string> _availableScenes = new List<string>();
        private static GameObject _pressLogoLabel;

        public static void UpdateTheme()
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor =
                                GameColors.BackgroundColor;
            var bg = GameObject.Find("BackgroundGrid");

            if (_pressLogoLabel != null)
            {
                var ls = _pressLogoLabel.GetComponent<LabelShowing>();
                if (ls.Shadow != null)
                    Destroy(ls.Shadow.gameObject);
                Destroy(_pressLogoLabel);
                PlayerPrefs.SetInt("PressLogoLabel", 1);
            }
        }

        void Awake()
        {
            Game.textFont = Resources.Load<Font>("Fonts/SEGUIBL");
            Game.numbersFont = Game.textFont;
            _mainCamera = GameObject.Find("Main Camera");
            var gui = GameObject.Find("/GUI");
            _soundButton = GameObject.Find("/GUI/SoundButton");
            _soundButton.GetComponent<Image>().sprite = GeneralSettings.SoundButtonSprite;
            if (SoundEnabled == SoundState.on)
                _mainCamera.GetComponent<AudioSource>().Play();

            UpdateTheme();
            _availableScenes.Add("6x6");
//#if !DEBUG
            if (GameSettingsHelper<Mode6x6SquarePlayground>.Preferenses.ScoreRecord < Mode8x8SquarePlayground.ToOpenPoints)
            {
                CloseLevelGUI("8x8", "6x6", Mode8x8SquarePlayground.ToOpenPoints);
            }
            else
//#endif
            _availableScenes.Add("8x8");
//#if !DEBUG
            if (GameSettingsHelper<Mode8x8SquarePlayground>.Preferenses.ScoreRecord < ModeMatch3SquarePlayground.ToOpenPoints)
            {
                CloseLevelGUI("Match3", "8x8", ModeMatch3SquarePlayground.ToOpenPoints);
            }
            else
//#endif
            _availableScenes.Add("Match3");
//#if !DEBUG
            if (GameSettingsHelper<ModeMatch3SquarePlayground>.Preferenses.ScoreRecord < Mode11RhombusPlayground.ToOpenPoints)
            {
                CloseLevelGUI("Rhombus", "Match3", Mode11RhombusPlayground.ToOpenPoints);
            }
            else
//#endif
            _availableScenes.Add("11Rhombus");

            if (!PlayerPrefs.HasKey("PressLogoLabel"))
            {
                _pressLogoLabel = (Instantiate(Resources.Load("Prefabs/Label")) as GameObject);
                if (_pressLogoLabel != null)
                {
                    _pressLogoLabel.transform.SetParent(GameObject.Find("/GUI").transform);
                    var pressLogoLabelShowing = _pressLogoLabel.GetComponent<LabelShowing>();
                    pressLogoLabelShowing.ShowScalingLabel(new Vector3(125, 350, -4), LanguageManager.Instance.GetTextValue("PressLogo"),
                        GameColors.DefaultLabelColor, GameColors.DefaultDark, LabelShowing.minLabelFontSize+10, LabelShowing.minLabelFontSize+40, 1, null, false, null, false, 0, new Vector3(0.3f,0.3f,1));
                }
            }



            if (PlayerPrefs.HasKey("_hard") && !PlayerPrefs.HasKey("RateUsUserMessage"))
            {
                Application.CancelQuit();
                var manualPrefab = LanguageManager.Instance.GetPrefab("UserHelp_RateUs");
                if (manualPrefab == null)
                    return;
                var manual = Instantiate(manualPrefab);
                var resource = Resources.Load("Prefabs/RateUsUserMessage");
                PauseButtonScript.PauseMenuActive = true;
                RateUsHelper.RateUsModule = Instantiate(resource) as GameObject;
                if (RateUsHelper.RateUsModule != null)
                {
                    RateUsHelper.RateUsModule.transform.SetParent(gui.transform);
                    RateUsHelper.RateUsModule.transform.localScale = Vector3.one;
                    RateUsHelper.RateUsModule.transform.localPosition = new Vector3(0, 0, -10);
                    manual.transform.SetParent(RateUsHelper.RateUsModule.transform);
                }
                manual.transform.localScale = new Vector3(25, 25, 0);
                manual.transform.localPosition = new Vector3(0, 30, -1);
                var rateNowButton = GameObject.Find("/GUI/RateUsUserMessage(Clone)/RateNowButton");
                if (rateNowButton != null)
                {
                    var text = rateNowButton.GetComponentInChildren<Text>();
                    text.font = Game.textFont;
                    text.fontSize = LabelShowing.maxLabelFontSize;
                    text.text = LanguageManager.Instance.GetTextValue("RateUs");
                    text.color = GameColors.DifficultyLevelsColors[DifficultyLevel._hard];
                }
                var rateLaterButton = GameObject.Find("/GUI/RateUsUserMessage(Clone)/RateLaterButton");
                if (rateLaterButton != null)
                {
                    var text = rateLaterButton.GetComponentInChildren<Text>();
                    text.font = Game.textFont;
                    text.fontSize = 20;
                    text.text = "X";
                    text.color = new Color(GameColors.DefaultDark.r,GameColors.DefaultDark.g,GameColors.DefaultDark.b,0.7f);
                }
            }

        }


        private void CloseLevelGUI(String level, String previos, Int32 pointsLeft)
        {
            var obj = GameObject.Find("/GUI/MainBlock/" + level);
            var img = obj.GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
            var lockLogo = Instantiate(Resources.Load("Prefabs/LockLogo")) as GameObject;
            lockLogo.transform.SetParent(obj.transform);
            lockLogo.transform.localPosition = new Vector3(0, -2, -4);
            var label = Instantiate(Resources.Load("Prefabs/ShortLabel")) as GameObject;
            label.transform.SetParent(obj.transform);
            label.transform.localPosition = new Vector3(0, -((float)Screen.height/8.8f), -4);
            var labelText = label.GetComponent<Text>();
            labelText.font = Game.textFont;
            labelText.color = GameColors.ModesColors[previos];
            labelText.fontSize = LabelShowing.maxLabelFontSize;
            labelText.text = pointsLeft.ToString();
            var modeLogo = Instantiate(Resources.Load("Prefabs/ModeLogo")) as GameObject;
            var modeSprite = Resources.LoadAll<Sprite>("ModesButtonsAtlas")
            .SingleOrDefault(t => t.name.Contains(previos));
            modeLogo.GetComponent<SpriteRenderer>().sprite = modeSprite;
            modeLogo.transform.SetParent(obj.transform);
            modeLogo.transform.localPosition = new Vector3(0, -((float)Screen.height / 17.6f), -5);
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
            SoundEnabled += 1;
            Vibration.Vibrate();
            _soundButton.GetComponent<Image>().sprite = GeneralSettings.SoundButtonSprite;
        }

        public void OnNavigationButtonClick(String scene)
        {
#if !DEBUG
            if (scene != "Statistics" && scene != "Help" && scene != "About" && !_availableScenes.Contains(scene)) return;
#endif
            Vibration.Vibrate();

            var gui = GameObject.Find("/GUI");
            var collection = gui.GetComponentsInChildren<Transform>();
            foreach (var component in collection)
            {
                if (gui != component.gameObject)
                    Destroy(component.gameObject);
            }
            var label = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
            label.transform.SetParent(gui.transform);
            label.transform.localPosition = Vector3.zero;
            label.transform.localScale = Vector3.one;
            var txt = label.GetComponent<Text>();
            txt.text = LanguageManager.Instance.GetTextValue("LoadingTitle");
            txt.fontSize = LabelShowing.maxLabelFontSize-10;
            txt.color = GameColors.DifficultyLevelsColors[DifficultyLevel._hard];
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
