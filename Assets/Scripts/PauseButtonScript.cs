using System;
using System.Linq;
using System.Threading;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using SmartLocalization;
using UnityEngine;
using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Scripts;

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

    void Awake() { }

    void Update() { }

    public void OnSoundButtonPressed()
    {
        Vibration.Vibrate();

        GeneralSettings.SoundEnabled = !GeneralSettings.SoundEnabled;

        _soundButton.GetComponent<Image>().sprite = GeneralSettings.SoundEnabled
            ? Resources.LoadAll<Sprite>("SD/SignsAtlas").SingleOrDefault(s => s.name.Contains("sound_on"))
            : Resources.LoadAll<Sprite>("SD/SignsAtlas").SingleOrDefault(s => s.name.Contains("sound_off"));
    }

    void CreatePauseMenu()
    {
        if (!PlaygroundProgressBar.ProgressBarRun) return;
        PauseMenuActive = true;

        Time.timeScale = 0F;

        var fg = GameObject.Find("/Foreground");
        if (fg == null) return;
        _pauseMenu = Instantiate(Resources.Load("Prefabs/PauseFullMenu")) as GameObject;
        _pauseMenu.transform.SetParent(fg.transform);
        _pauseMenu.transform.localScale = Vector3.one;
        _pauseMenu.transform.localPosition = new Vector3(0, 0, -2);

        var pauseButton = GameObject.Find("/Foreground/PauseButton");

        _soundButton = MainMenuScript.GenerateMenuButton("Prefabs/SoundButton", fg.transform, Vector3.one, new Vector3(-pauseButton.transform.localPosition.x,
            pauseButton.transform.localPosition.y, -3), null, 0, OnSoundButtonPressed);
        var rectTransform = _soundButton.transform as RectTransform;
        _soundButton.GetComponent<Image>().sprite = GeneralSettings.SoundEnabled
            ? Resources.LoadAll<Sprite>("SD/SignsAtlas").SingleOrDefault(s => s.name.Contains("sound_on"))
            : Resources.LoadAll<Sprite>("SD/SignsAtlas").SingleOrDefault(s => s.name.Contains("sound_off"));
        _soundButton.name = "SoundButton";
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
        var middleground = GameObject.Find("Middleground");
        if (middleground == null) return;
        var pg = middleground.GetComponentInChildren<IPlayground>();
        if (pg == null) return;
        pg.ResetPlayground();
        pg.UpdateTime();
        Game.Difficulty = Assets.Scripts.Enums.DifficultyLevel._easy;
        SavedataHelper.SaveData(pg.SavedataObject);
        Application.LoadLevel(Application.loadedLevel);
        Vibration.Vibrate();
        DestroyPauseMenu();
        PauseMenuActive = false;
    }

    public void CreateResetConfirmationMenu()
    {
        Time.timeScale = 0F;

        //var fg = GameObject.Find("/Foreground");

        _resetConfirmationMenu = Instantiate(Resources.Load("Prefabs/ResetGameConfirmation")) as GameObject;

        if (_pauseMenu != null)
        {
            _resetConfirmationMenu.transform.SetParent(_pauseMenu.transform);
        }

        _resetConfirmationMenu.transform.localScale = Vector3.one;
        _resetConfirmationMenu.transform.localPosition = new Vector3(0, 0, -2);
        var l = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
        l.transform.SetParent(_resetConfirmationMenu.transform);
        l.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        if (l != null)
        {
            var pointsLabel = l.GetComponent<LabelShowing>();
            pointsLabel.ShowScalingLabel(new Vector3(0, 50, 0), LanguageManager.Instance.GetTextValue("ConfirmationQuestion"), GameColors.DefaultLight, Color.gray, Game.maxLabelFontSize, Game.maxLabelFontSize, 1, Game.textFont);
        }
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
