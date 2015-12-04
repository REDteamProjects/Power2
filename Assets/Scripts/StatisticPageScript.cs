using System;
using System.Globalization;
using Assets.Scripts;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using Assets.Scripts.Interfaces;
using SmartLocalization;
using UnityEngine;
using UnityEngine.UI;

public class StatisticPageScript : MonoBehaviour
{
    private static GameTypes? SelectedType;
    private static GameObject SelectedItem;
    private static GameObject _resetConfirmationMenu;

    private void GenerateLevelTitle<T>(GameItemType type) where T:IPlayground
    {
        //var typeObjectName = typeof (T).Name;

        var levelTitle = GameObject.Find(/*typeObjectName.Substring(0, typeObjectName.Length - 10) + */"Body/Level");

        if (levelTitle == null) return;

        var child = levelTitle.GetComponentInChildren<GameItem>();
        if (child != null)
            Destroy(child.gameObject);

        var inststr = type <= GameItemType.DisabledItem ? "Prefabs/GameItem_LostItem" : ItemsNameHelper.GetPrefabPath<T>() + type;
        var newgobj =
            Instantiate(Resources.Load(inststr)) as GameObject;
        
        if (newgobj == null) return;

        newgobj.transform.SetParent(levelTitle.transform);
        newgobj.transform.localPosition = new Vector3(0, 0);
        newgobj.transform.localScale = type <= GameItemType.DisabledItem ? new Vector3(80, 80) :/*typeObjectName.Contains("Match3") ? new Vector3(5, 5) : */
            new Vector3(10, 10);
    }

    private void LoadDataToView<TSavedataType, TType>()
        where TSavedataType : IPlaygroundSavedata
        where TType : IPlayground
    {
        var typeObject = typeof(TType);
        var typeSaveDataObject = typeof (TSavedataType);
        var constructorInfo = typeSaveDataObject.GetConstructor(Type.EmptyTypes);
        if (constructorInfo == null) return;
        var sd = (IPlaygroundSavedata)constructorInfo.Invoke(null);
        //SavedataHelper.LoadData(ref sd);

        SelectedItem = GameObject.Find(typeObject.Name.Substring(0, typeObject.Name.Length - 10));
        bool noData = false;
        if (!SavedataHelper.IsSaveDataExist(sd))
        {
            GenerateLevelTitle<TType>(GameItemType.DisabledItem);
            noData = true;
        }

        var pref = GameSettingsHelper<TType>.Preferenses;

        var scoreText = GameObject.Find(/*typeObject.Name.Substring(0, typeObject.Name.Length - 10) +*/ "Body/Score").GetComponent<Text>();
        scoreText.text = noData ? "0" : pref.ScoreRecord.ToString(CultureInfo.InvariantCulture);
        GameObject.Find("BodyShadow/Score").GetComponent<Text>().text = scoreText.text;

        var gamesText = GameObject.Find(/*typeObject.Name.Substring(0, typeObject.Name.Length - 10) + */"Body/Game").GetComponent<Text>();
        gamesText.text = noData ? "" : pref.GamesPlayed.ToString(CultureInfo.InvariantCulture);
        GameObject.Find("BodyShadow/Game").GetComponent<Text>().text = gamesText.text;

        GenerateLevelTitle<TType>(pref.CurrentItemType);

        var timeText = GameObject.Find(/*typeObject.Name.Substring(0, typeObject.Name.Length - 10) +*/ "Body/Time").GetComponent<Text>();
        if (pref.LongestSession < 1 || noData)
        {
            timeText.text = "00:00";
            GameObject.Find("BodyShadow/Time").GetComponent<Text>().text = timeText.text;
            return;
        }

        var time = new TimeSpan(0, 0, (int)(pref.LongestSession));
        timeText.text = (time.Hours > 0 ? time.Hours.ToString("D2") + ":" : "") + time.Minutes.ToString("D2") + ":" + time.Seconds.ToString("D2");
        GameObject.Find("BodyShadow/Time").GetComponent<Text>().text = timeText.text;
    }

    void Awake()
    {
        LoadLevelData(0);
        GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = GameColors.BackgroundColor;
        var fg = GameObject.Find("/GUI");
        var btnTextShadow = MainMenuScript.GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(3, -320, 0), LanguageManager.Instance.GetTextValue("ResetAll"), 50, null, GameColors.DefaultDark).GetComponentInChildren<Text>();
        MainMenuScript.GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, -320, 0), btnTextShadow.text, btnTextShadow.fontSize,
                () => CreateResetStatConfirmationMenu());
    }

    void CreateResetStatConfirmationMenu()
    {
        Time.timeScale = 0F;

        var fg = GameObject.Find("/Foreground");

        _resetConfirmationMenu = Instantiate(Resources.Load("Prefabs/ResetStatsConfirmation")) as GameObject;

        if (fg != null)
        {
            _resetConfirmationMenu.transform.SetParent(fg.transform);
        }

        _resetConfirmationMenu.transform.localScale = Vector3.one;
        _resetConfirmationMenu.transform.localPosition = new Vector3(0, 0, -2);
        var l = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
        l.transform.SetParent(_resetConfirmationMenu.transform);
        l.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        if (l != null)
        {
            var pointsLabel = l.GetComponent<LabelShowing>();
            pointsLabel.ShowScalingLabel(new Vector3(0,50,0), LanguageManager.Instance.GetTextValue("ConfirmationQuestion"), GameColors.DefaultLight, Color.gray, Game.maxLabelFontSize, Game.maxLabelFontSize, 1, Game.textFont);
        }
    }

    public void ResetStats()
    {
        PlayerPrefs.DeleteAll();
        SelectedItem = null;
        LoadLevelData((int)SelectedType);
        DestroyResetStatConfirmationMenu();
    }

    public void DestroyResetStatConfirmationMenu()
    {
        Destroy(_resetConfirmationMenu);
    }

    public void LoadLevelData(int ttype)
    {
        var type = (GameTypes) ttype;

        if (type == SelectedType && SelectedItem != null) return;

        if (SelectedItem != null)
        {
            var lastbOject = SelectedItem.GetComponent<SpriteRenderer>();
            if (lastbOject != null)
            {
                lastbOject.color = new Color(lastbOject.color.r, lastbOject.color.g, lastbOject.color.b, 0.5f);
            }
        }
        
        switch (type)
        {
            case GameTypes._6x6:
                LoadDataToView<Mode6x6SquarePlaygroundSavedata, Mode6x6SquarePlayground>();
                break;
            case GameTypes._8x8:
                LoadDataToView<Mode8x8SquarePlaygroundSavedata, Mode8x8SquarePlayground>();
                break;
            case GameTypes._rhombus:
                LoadDataToView<Mode11RhombusPlaygroundSavedata, Mode11RhombusPlayground>();
                break;
            case GameTypes._match3:
                LoadDataToView<ModeMatch3PlaygroundSavedata, ModeMatch3Playground>();
                break;
            case GameTypes._drops:
                LoadDataToView<ModeDropsPlaygroundSavedata, ModeDropsPlayground>();
                break;
        }
        SelectedType = type;

        if (SelectedItem != null)
        {
            var lastbOject = SelectedItem.GetComponent<SpriteRenderer>();
            if (lastbOject != null)
            {
                lastbOject.color = new Color(lastbOject.color.r, lastbOject.color.g, lastbOject.color.b, 1f);
            }
        }
    }
}
