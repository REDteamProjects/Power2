using UnityEngine;
using System.Collections;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Helpers;

public class PauseButtonScript : MonoBehaviour
{
    private static GameObject _backButton;
    private static GameObject _resetButton;
    private static GameObject _continueButton;
    private static GameObject _pausebackground;
    private static bool _pauseMenuActive;

    public static bool PauseMenuActive
    {
        get { return _pauseMenuActive; }
        set
        {
            _pauseMenuActive = value;
            LogFile.Message("Pause active: " + _pauseMenuActive);
        }
    }

    void Awake()
    {
    }

    void Update()
    {
        if (Application.platform != RuntimePlatform.Android) return;
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (PauseMenuActive)
            DestroyPauseMenu();
        else
            CreatePauseMenu();
    }

    void CreatePauseMenu()
    {
        Time.timeScale = 0F;

        var fg = GameObject.Find("/Foreground");
        
        _pausebackground = Instantiate(Resources.Load("Prefabs/PauseBackground")) as GameObject;
        _backButton = Instantiate(Resources.Load("Prefabs/ToMainMenuButton")) as GameObject;
        _resetButton = Instantiate(Resources.Load("Prefabs/ResetButton")) as GameObject;
        _continueButton = Instantiate(Resources.Load("Prefabs/ResumeButton")) as GameObject;

        if (fg != null)
        {
            _pausebackground.transform.SetParent(fg.transform);
            _backButton.transform.SetParent(fg.transform);
            _resetButton.transform.SetParent(fg.transform);
            _continueButton.transform.SetParent(fg.transform); 
        }

        _pausebackground.transform.localPosition= Vector3.zero;
        (_pausebackground.transform as RectTransform).sizeDelta = Vector2.zero;

        _resetButton.transform.localScale = Vector3.one;
        _resetButton.transform.localPosition = new Vector3(100, 0, 0);

        _continueButton.transform.localScale = Vector3.one;
        _continueButton.transform.localPosition = new Vector3(0, 0, 0);
        
        _backButton.transform.localScale = Vector3.one;
        _backButton.transform.localPosition = new Vector3(-100, 0, 0);

        PauseMenuActive = true;
    }

    void DestroyPauseMenu()
    {
        Destroy(_continueButton);
        Destroy(_backButton);
        Destroy(_resetButton);
        Destroy(_pausebackground);

        PauseMenuActive = false;
        Time.timeScale = 1F;
    }

    public void OnResumeButtonClick()
    {
        if (PauseMenuActive)
            DestroyPauseMenu();
    }

    public void OnPauseButtonClick()
    {
        if (!PauseMenuActive)
            CreatePauseMenu();
    }

    public void OnResetButtonClick()
    {
        //if (!PauseMenuActive) return;
        var middleground = GameObject.Find("Middleground");
        if (middleground == null) return;
        var pg = middleground.GetComponentInChildren<IPlayground>();
        if (pg == null) return;
        pg.ResetPlayground();
        pg.UpdateTime();
        SavedataHelper.SaveData(pg.SavedataObject);
        Application.LoadLevel(Application.loadedLevel);

        //Time.timeScale = 1F;
        DestroyPauseMenu();
        PauseMenuActive = false;
    }

    public void OnToMainMenuButtonClick()
    {
        //if (!PauseMenuActive) return;

        Application.LoadLevel("MainScene");

        //Time.timeScale = 1F;
        DestroyPauseMenu();
        PauseMenuActive = false;
    }

    //public void OnApplicationPause(bool pause)
    //{
    //    if (!PauseMenuActive)
    //        CreatePauseMenu();
    //}
}
