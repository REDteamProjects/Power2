using UnityEngine;
using System.Collections;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Helpers;
using Assets.Scripts.DataClasses;

public class PauseButtonScript : MonoBehaviour
{
    private static bool _pauseMenuActive;
    private static GameObject _pauseMenu;

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
    }

    void DestroyPauseMenu()
    {
        Time.timeScale = 1F;

        Destroy(_pauseMenu);

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
