using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using Assets.Scripts.Interfaces;
using SmartLocalization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PauseButtonScript : MonoBehaviour
    {
        private static bool _pauseMenuActive;
        private static GameObject _pauseMenu;
        private static GameObject _soundButton;
        private static GameObject _resetConfirmationMenu;

        public static bool PauseMenuActive
        {
            get { return _pauseMenuActive; }
            set
            {
                _pauseMenuActive = value;
                LogFile.Message("Pause active: " + _pauseMenuActive, true);
            }
        }

        public void OnSoundButtonPressed()
        {
            Vibration.Vibrate();

            GeneralSettings.SoundEnabled++;

            _soundButton.GetComponent<Image>().sprite = GeneralSettings.SoundButtonSprite;
        }

        void CreatePauseMenu()
        {
            if (!PlaygroundProgressBar.ProgressBarRun) return;
            PauseMenuActive = true;

            Time.timeScale = 0F;

            _pauseMenu = Instantiate(Resources.Load("Prefabs/PauseFullMenu")) as GameObject;
            if (_pauseMenu != null)
            {
                _pauseMenu.transform.SetParent(Game.Foreground.transform);
                _pauseMenu.transform.localScale = Vector3.one;
                _pauseMenu.transform.localPosition = new Vector3(0, 0, -2);
            }

            var pauseButton = GameObject.Find("/Foreground/PauseButton");

            /*_soundButton = MainMenuScript.GenerateMenuButton("Prefabs/SoundButton", fg.transform, Vector3.one, new Vector3(-pauseButton.transform.localPosition.x,
                pauseButton.transform.localPosition.y, -3), null, 0, OnSoundButtonPressed);
            _soundButton.GetComponent<Image>().sprite = GeneralSettings.SoundButtonSprite;
            _soundButton.name = "SoundButton";*/
        }

        void DestroyPauseMenu()
        {
            Time.timeScale = 1F;

            Destroy(_pauseMenu);
            Destroy(_soundButton);

            PauseMenuActive = false;
        }

        public void OnResumeButtonClick()
        {
            if (PauseMenuActive)
            {
                Vibration.Vibrate();
                DestroyPauseMenu();
            }
        }

        public void OnPauseButtonClick()
        {
            if (PauseMenuActive)
                DestroyPauseMenu();
            else
                CreatePauseMenu();
        }

        public void OnResetConfirmButtonClick()
        {
            Time.timeScale = 1F;
            var pg = Game.Middleground.GetComponentInChildren<IPlayground>();
            if (pg == null) return;
            pg.ResetPlayground();
            pg.UpdateTime();
            Game.Difficulty = DifficultyLevel._easy;
            SavedataHelper.SaveData(pg.SavedataObject);
            Application.LoadLevel(Application.loadedLevel);
            Vibration.Vibrate();
            //DestroyPauseMenu();
            PauseMenuActive = false;
        }

        public void CreateResetConfirmationMenu()
        {

            _resetConfirmationMenu = Instantiate(Resources.Load("Prefabs/ResetGameConfirmation")) as GameObject;

            if (_resetConfirmationMenu == null) return;
            if (_pauseMenu != null)
            {
                _resetConfirmationMenu.transform.SetParent(_pauseMenu.transform);
            }

            _resetConfirmationMenu.transform.localScale = Vector3.one;
            _resetConfirmationMenu.transform.localPosition = new Vector3(0, 0, -2);


            var l = Instantiate(LabelShowing.LabelPrefab) as GameObject;
            if (l == null) return;

            l.transform.SetParent(_resetConfirmationMenu.transform);
            l.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            var pointsLabel = l.GetComponent<LabelShowing>();
            pointsLabel.ShowScalingLabel(new Vector3(0, 50, 0),
                LanguageManager.Instance.GetTextValue("ConfirmationQuestion"), GameColors.DefaultLabelColor,
                GameColors.DefaultDark, LabelShowing.maxLabelFontSize, LabelShowing.maxLabelFontSize, 1, Game.textFont);
        }

        public void DestroyResetConfirmationMenu()
        {
            Destroy(_resetConfirmationMenu);
        }

        public void OnToMainMenuButtonClick()
        {
            Application.LoadLevel("MainScene");
            Vibration.Vibrate();
            DestroyPauseMenu();
            PauseMenuActive = false;
        }

    }
}
