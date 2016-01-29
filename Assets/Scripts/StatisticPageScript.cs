using System;
using System.Globalization;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using Assets.Scripts.Interfaces;
using SmartLocalization;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Collections.Generic;

public class StatisticPageScript : MonoBehaviour
{
    private static GameTypes? SelectedType;
    private static GameObject SelectedItem;
    private static GameObject _resetConfirmationMenu;
    private bool isExtremeSwitched = false;

    private void GenerateLevelTitle<T>(GameItemType type) where T:IPlayground
    {
        //var typeObjectName = typeof (T).Name;

        var levelTitle = GameObject.Find(/*typeObjectName.Substring(0, typeObjectName.Length - 10) + */"Body/Level");

        if (levelTitle == null) return;

        var child = levelTitle.GetComponentInChildren<GameItem>();
        if (child != null)
            Destroy(child.gameObject);

        var inststr = type <= GameItemType.DisabledItem ? "Prefabs/GameItem_LostItem" : ItemsNameHelper.GetPrefabPath<T>();
        var newgobj =
            Instantiate(Resources.Load(inststr)) as GameObject;
        if (newgobj == null) return;
        if (type > GameItemType.DisabledItem)
        newgobj.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>(inststr.Split('/')[1] + "Tiles").FirstOrDefault(t => t.name.Contains(GameItem.GetStandartTextureIDByType(type)));
        newgobj.transform.SetParent(levelTitle.transform);
        newgobj.transform.localPosition = new Vector3(0, 0);
        //newgobj.transform.localScale = type <= GameItemType.DisabledItem ? new Vector3(80, 80) : new Vector3(10, 10);
        newgobj.transform.localScale = type <= GameItemType.DisabledItem ? new Vector3(10, 10) : Vector3.one;//new Vector3(10, 10);
    }

    private void LoadDataToView<TSavedataType, TType>()
        where TSavedataType : IPlaygroundSavedata
        where TType : IPlayground
    {
        var typeObject = typeof(TType);

        SelectedItem = GameObject.Find(typeObject.Name.Substring(0, typeObject.Name.Length - 10));

        var pref = GameSettingsHelper<TType>.Preferenses;

        var scoreText = GameObject.Find("Body/Score").GetComponent<Text>();
        scoreText.text = pref.ScoreRecord.ToString(CultureInfo.InvariantCulture);
        GameObject.Find("BodyShadow/Score").GetComponent<Text>().text = scoreText.text;

        var gamesText = GameObject.Find("Body/Game").GetComponent<Text>();
        gamesText.text = pref.GamesPlayed.ToString(CultureInfo.InvariantCulture);
        GameObject.Find("BodyShadow/Game").GetComponent<Text>().text = gamesText.text;

        var movesText = GameObject.Find("Body/Moves").GetComponent<Text>();
        movesText.text = pref.MovesRecord.ToString(CultureInfo.InvariantCulture);
        GameObject.Find("BodyShadow/Moves").GetComponent<Text>().text = movesText.text;

        GenerateLevelTitle<TType>(pref.CurrentItemType <= GameItemType.NullItem ? GameItemType.DisabledItem : pref.CurrentItemType);

        var timeText = GameObject.Find("Body/Time").GetComponent<Text>();
        
        if (pref.LongestSession < 1 )
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
        LanguageHelper.ActivateSystemLanguage();

        LoadLevelData(0);
        GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor = GameColors.BackgroundColor;
        //var fg = GameObject.Find("/GUI");
        GameObject.Find("/GUI/ResetProgressButtonShadow").GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("ResetAll");//MainMenuScript.GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(3, 80, 0), LanguageManager.Instance.GetTextValue("ResetAll"), 60, null, GameColors.DefaultDark).GetComponentInChildren<Text>();
        var rpb = GameObject.Find("/GUI/ResetProgressButton");
        rpb.GetComponentInChildren<Text>().text = LanguageManager.Instance.GetTextValue("ResetAll");
        rpb.GetComponent<Button>().onClick.AddListener(() => CreateResetStatConfirmationMenu());
        //MainMenuScript.GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, 80, 0), btnTextShadow.text, btnTextShadow.fontSize,
                //CreateResetStatConfirmationMenu);

    }

    void CreateResetStatConfirmationMenu()
    {
        Time.timeScale = 0F;

        var fg = GameObject.Find("/Foreground");

        _resetConfirmationMenu = Instantiate(Resources.Load("Prefabs/ResetStatsConfirmation")) as GameObject;

        if (_resetConfirmationMenu == null) return;

        if (fg != null)
             _resetConfirmationMenu.transform.SetParent(fg.transform);

        _resetConfirmationMenu.transform.localScale = Vector3.one;
        _resetConfirmationMenu.transform.localPosition = new Vector3(0, 0, -2);
        
        var l = Instantiate(LabelShowing.LabelPrefab) as GameObject;
        if (l == null) return;

        l.transform.SetParent(_resetConfirmationMenu.transform);
        l.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        var pointsLabel = l.GetComponent<LabelShowing>();
        pointsLabel.ShowScalingLabel(new Vector3(0, 50, 0), LanguageManager.Instance.GetTextValue("ConfirmationQuestion"), GameColors.DefaultLabelColor, GameColors.DefaultDark, 
            LabelShowing.maxLabelFontSize, LabelShowing.maxLabelFontSize, 1, Game.textFont);
    }

    public void SwitchFromToExtreme()
    {
        if(Game.isExtreme)
        {
            Game.isExtreme = false;
            GameObject.Find("/GUI/Extreme").GetComponent<Image>().color = new Color(255f, 255f, 255f, 0.7f);
        }
        else
        {
            Game.isExtreme = true;
            GameObject.Find("/GUI/Extreme").GetComponent<Image>().color = new Color(255f, 255f, 255f, 1f);
        }
        if (SelectedType.HasValue)
        {
            isExtremeSwitched = true;
            LoadLevelData((int)SelectedType.Value);
        }
    }


    public void ResetProgress()
    {
        GeneralSettings.RemoveAllPrefsExceptGeneral();
        DestroyResetStatConfirmationMenu();
        SelectedItem = null;
        LoadLevelData((int)SelectedType.GetValueOrDefault());
        SavedataHelper.RemoveAllData();
    }

    public void DestroyResetStatConfirmationMenu()
    {
        Destroy(_resetConfirmationMenu);
    }

    public void LoadLevelData(int ttype)
    {
        var type = (GameTypes) ttype;

        if (type == SelectedType && SelectedItem != null && !isExtremeSwitched) return;

        isExtremeSwitched = false;

        SpriteRenderer lastbOject;
        if (SelectedItem != null)
        {
            lastbOject = SelectedItem.GetComponent<SpriteRenderer>();
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
            case GameTypes._match3:
                LoadDataToView<ModeMatch3PlaygroundSavedata, ModeMatch3SquarePlayground>();
                break;
            case GameTypes._rhombus:
                LoadDataToView<Mode11RhombusPlaygroundSavedata, Mode11RhombusPlayground>();
                break;
            case GameTypes._drops:
                LoadDataToView<ModeDropsPlaygroundSavedata, ModeDropsPlayground>();
                break;
        }
        SelectedType = type;

        if (SelectedItem == null) return;
        lastbOject = SelectedItem.GetComponent<SpriteRenderer>();
        if (lastbOject != null)
        {
            lastbOject.color = new Color(lastbOject.color.r, lastbOject.color.g, lastbOject.color.b, 1f);
        }
    }
}
