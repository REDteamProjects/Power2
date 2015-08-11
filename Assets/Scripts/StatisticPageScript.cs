using System;
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
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{ }

    private void GenerateLevelTitle<T>(GameItemType type) where T:IPlayground
    {
        var typeObjectName = typeof (T).Name;

        var levelTitle = GameObject.Find(typeObjectName.Substring(0, typeObjectName.Length - 10) + "/Level");

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
        newgobj.transform.localScale = /*type <= GameItemType.DisabledItem ? new Vector3(4, 4) :*/ new Vector3(10, 10);
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

        var pref = GameSettingsHelper<TType>.Preferenses;

        var scoreText = GameObject.Find(typeObject.Name.Substring(0, typeObject.Name.Length - 10) + "/Score").GetComponent<Text>();
        scoreText.text = pref.ScoreRecord.ToString(CultureInfo.InvariantCulture);

        var gamesText = GameObject.Find(typeObject.Name.Substring(0, typeObject.Name.Length - 10) + "/Game").GetComponent<Text>();
        gamesText.text = pref.GamesPlayed.ToString(CultureInfo.InvariantCulture);

        GenerateLevelTitle<TType>(pref.CurrentItemType);

        if (pref.LongestSession < 1) return;

        var timeText = GameObject.Find(typeObject.Name.Substring(0, typeObject.Name.Length - 10) + "/Time").GetComponent<Text>();
        var time = new TimeSpan(0, 0, (int)(pref.LongestSession));
        timeText.text = (time.Hours > 0 ? time.Hours.ToString("D2") + ":" : "") + time.Minutes.ToString("D2") + ":" + time.Seconds.ToString("D2");
    }

    void LoadData()
    {
        LoadDataToView<Mode6x6SquarePlaygroundSavedata, Mode6x6SquarePlayground>();
        LoadDataToView<Mode8x8SquarePlaygroundSavedata, Mode8x8SquarePlayground>();
        LoadDataToView<ModeMatch3PlaygroundSavedata, ModeMatch3Playground>();
        LoadDataToView<ModeDropsPlaygroundSavedata, ModeDropsPlayground>();
        LoadDataToView<Mode11RhombusPlaygroundSavedata, Mode11RhombusPlayground>();
    }

    void Awake()
    {
        LoadData();

        var fg = GameObject.Find("/GUI");
        MainMenuScript.GenerateMenuButton("Prefabs/MainMenuButton", fg.transform, Vector3.one, new Vector3(0, -300, 0), "Reset all", 50,
                () => { PlayerPrefs.DeleteAll(); LoadData(); });
    }
}
