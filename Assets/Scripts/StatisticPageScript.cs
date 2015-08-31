﻿using System;
using System.Globalization;
using Assets.Scripts;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class StatisticPageScript : MonoBehaviour
{
    private static GameTypes SelectedType = GameTypes._6x6;
    private static GameObject SelectedItem;

    private void GenerateLevelTitle<T>(GameItemType type) where T:IPlayground
    {
        var typeObjectName = typeof (T).Name;

        var levelTitle = GameObject.Find(/*typeObjectName.Substring(0, typeObjectName.Length - 10) + */"Body/Level");

        if (levelTitle == null) return;

        var child = levelTitle.GetComponentInChildren<GameItem>();
        if (child != null)
            Destroy(child.gameObject);

        var inststr = type <= GameItemType.DisabledItem ? "Prefabs/GameItem_LostItem" : ItemPrefabNameHelper.GetPrefabPath<T>() + type;
        var newgobj =
            Instantiate(Resources.Load(inststr)) as GameObject;
        
        if (newgobj == null) return;

        newgobj.transform.SetParent(levelTitle.transform);
        newgobj.transform.localPosition = new Vector3(0, 0);
        newgobj.transform.localScale = /*type <= GameItemType.DisabledItem ? new Vector3(4, 4) :*/ 
            new Vector3(5, 5);
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

        if (!SavedataHelper.IsSaveDataExist(sd))
        {
            GenerateLevelTitle<TType>(GameItemType.DisabledItem);
            return;
        }
        //SavedataHelper.LoadData(ref sd);

        SelectedItem = GameObject.Find(typeObject.Name.Substring(0, typeObject.Name.Length - 10));

        var pref = GameSettingsHelper<TType>.Preferenses;

        var scoreText = GameObject.Find(/*typeObject.Name.Substring(0, typeObject.Name.Length - 10) +*/ "Body/Score").GetComponent<Text>();
        scoreText.text = pref.ScoreRecord.ToString(CultureInfo.InvariantCulture);

        var gamesText = GameObject.Find(/*typeObject.Name.Substring(0, typeObject.Name.Length - 10) + */"Body/Game").GetComponent<Text>();
        gamesText.text = pref.GamesPlayed.ToString(CultureInfo.InvariantCulture);

        GenerateLevelTitle<TType>(pref.CurrentItemType);

        if (pref.LongestSession < 1) return;

        var timeText = GameObject.Find(/*typeObject.Name.Substring(0, typeObject.Name.Length - 10) +*/ "Body/Time").GetComponent<Text>();
        var time = new TimeSpan(0, 0, (int)(pref.LongestSession));
        timeText.text = (time.Hours > 0 ? time.Hours.ToString("D2") + ":" : "") + time.Minutes.ToString("D2") + ":" + time.Seconds.ToString("D2");
    }

    void Awake()
    {
        LoadLevelData(0);

        var fg = GameObject.Find("/GUI");
        MainMenuScript.GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, -300, 0), "Reset all", 50,
                () => { PlayerPrefs.DeleteAll(); LoadLevelData((int)SelectedType); });
    }

    public void LoadLevelData(int ttype)
    {
        if (SelectedItem != null)
        {
            var lastbOject = gameObject.GetComponent<SpriteRenderer>();
            if (lastbOject != null)
            {
                lastbOject.color = new Color(lastbOject.color.r, lastbOject.color.g, lastbOject.color.b, 0.5f);
            }
        }
        var type = (GameTypes) ttype;
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
            var lastbOject = gameObject.GetComponent<SpriteRenderer>();
            if (lastbOject != null)
            {
                lastbOject.color = new Color(lastbOject.color.r, lastbOject.color.g, lastbOject.color.b, 0.5f);
            }
        }
    }
}
