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
        PauseMenuActive = true;

        Time.timeScale = 0F;

        var fg = GameObject.Find("/Foreground");

        _pauseMenu = Instantiate(Resources.Load("Prefabs/PauseFullMenu")) as GameObject;

        if (fg != null)
        {
            _pauseMenu.transform.SetParent(fg.transform);
        }

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

    public void OnResetButtonClick()
    {
        var middleground = GameObject.Find("Middleground");
        if (middleground == null) return;
        var pg = middleground.GetComponentInChildren<IPlayground>();
        if (pg == null) return;
        pg.ResetPlayground();
        pg.UpdateTime();
        Game.Difficulty = Assets.Scripts.Enums.DifficultyLevel.easy;
        SavedataHelper.SaveData(pg.SavedataObject);
        Application.LoadLevel(Application.loadedLevel);
        Vibration.Vibrate();
        DestroyPauseMenu();
        PauseMenuActive = false;
    }

    public void OnToMainMenuButtonClick()
    {
        Application.LoadLevel("MainScene");
        Vibration.Vibrate();
        DestroyPauseMenu();
        PauseMenuActive = false;
    }

}
