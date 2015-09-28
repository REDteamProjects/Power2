using System;
using System.Linq;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using UnityEngine;
using Assets.Scripts.Interfaces;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class Mode6x6SquarePlayground : SquarePlayground
    {
        private readonly RealPoint _initialGameItemX = new RealPoint() { X = -12.8F, Y = 12.22F, Z = -1 };

        public override IGameSettingsHelper Preferenses
        {
            get { return GameSettingsHelper<Mode6x6SquarePlayground>.Preferenses; }
        }

        public override String ItemPrefabName { get { return ItemsNameHelper.GetPrefabPath<Mode6x6SquarePlayground>(); } }

        public override string ItemBackgroundTextureName { get { return ItemsNameHelper.GetBackgroundTexturePrefix<Mode6x6SquarePlayground>(); } }

        public override IPlaygroundSavedata SavedataObject
        {
            get
            {
                var sd = new Mode6x6SquarePlaygroundSavedata
                {
                    MaxInitialElementType = MaxType,
                    Items = new GameItemType[FieldSize][],
                    //PlaygroundStat = GetComponent<Game>().Stats,
                    CurrentPlaygroundTime = CurrentTime + Time.timeSinceLevelLoad,
                    Difficulty = Game.Difficulty,
                    ProgressBarStateData = new ProgressBarState { Multiplier = ProgressBar.Multiplier, State = ProgressBar.State, Upper = ProgressBar.Upper }
                };
                if (Items == null)
                    sd.Items = null;
                else
                {
                    for (var i = 0; i < FieldSize; i++)
                    {
                        if (sd.Items[i] == null)
                            sd.Items[i] = new GameItemType[FieldSize];
                        for (var j = 0; j < FieldSize; j++)
                        {
                            var gobj = Items[i][j] as GameObject;

                            if (gobj != null)
                                sd.Items[i][j] = Items[i][j] != null && Items[i][j] != DisabledItem
                                    ? gobj.GetComponent<GameItem>().Type
                                    : (Items[i][j] == DisabledItem ? GameItemType.DisabledItem : GameItemType.NullItem);
                        }
                    }
                }

                sd.Score = CurrentScore;
                return sd;
            }
        }

        public override int FieldSize { get { return 6; } }

        public override RealPoint InitialGameItemPosition { get { return _initialGameItemX; } }

        public override float GameItemSize { get { return 5.14f; } }

        void OnLevelWasLoaded()
        {
            if (!IsGameOver)
                DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Start, false);
        }

        void Awake()
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor =
                                GameColors.BackgroundColor;

            /*GameObject.Find("PauseButton").GetComponent<Image>().color =
                GameColors.ForegroundButtonsColor;*/

            GameObject.Find("BackgroundGrid").GetComponent<Image>().sprite =
                Resources.LoadAll<Sprite>("SD/6x6Atlas")
               .SingleOrDefault(t => t.name.Contains(Game.Theme.ToString()));

            PlaygroundProgressBar.ProgressBarOver += ProgressBarOnProgressBarOver;

            #if UNITY_WINRT || UNITY_WP8
                WinRTDeviceHelper.FireShowAd();
            #endif          

            Items = new[]
            {
                new System.Object[FieldSize],
                new System.Object[FieldSize],
                new System.Object[FieldSize],
                new System.Object[FieldSize],
                new System.Object[FieldSize],
                new System.Object[FieldSize]
            };

            IPlaygroundSavedata sd = new Mode6x6SquarePlaygroundSavedata { Difficulty = Game.Difficulty };
            if (SavedataHelper.IsSaveDataExist(sd))
            {
                SavedataHelper.LoadData(ref sd);

                //var gC = GetComponent<Game>();
                //gC.Stats = sd.PlaygroundStat;

                if (sd.Items != null)
                {
                    for (var i = 0; i < FieldSize; i++)
                        for (var j = 0; j < FieldSize; j++)
                        {
                            Items[i][j] = sd.Items[i][j] != GameItemType.NullItem ? (
                                sd.Items[i][j] != GameItemType.DisabledItem ?
                                GenerateGameItem(sd.Items[i][j], i, j) : DisabledItem)
                                : null;
                            switch (sd.Items[i][j])
                            {
                                case GameItemType._DropDownItem:
                                    DropDownItemsCount++;
                                    break;
                                case GameItemType._XItem:
                                    XItemsCount++;
                                    break;
                            }
                        }

                    //var score = GetComponentInChildren<Text>();
                    //if (score != null)
                    //    score.text = sd.Score.ToString(CultureInfo.InvariantCulture);

                    Game.Difficulty = sd.Difficulty;

                    CurrentTime = sd.CurrentPlaygroundTime;

                    var mit = ((SquarePlaygroundSavedata)sd).MaxInitialElementType;
                    if (mit != MaxInitialElementType)
                        MaxInitialElementType = mit;
                    else
                        ShowMaxInitialElement();
                    RisePoints(sd.Score);

                    ProgressBar.InnitializeBar(sd.ProgressBarStateData.State, sd.ProgressBarStateData.Upper, sd.ProgressBarStateData.Multiplier);
                    return;
                }
            }

            //var stat = GetComponent<Game>().Stats;
            //if (stat != null)
            //{
            Preferenses.GamesPlayed++;
            if (Preferenses.CurrentItemType < MaxType)
                Preferenses.CurrentItemType = MaxType;
            //}
            GenerateField();
            ShowMaxInitialElement();
            
            //var a = Items[FieldSize - 1][FieldSize-1] as GameObject;
            //DownPoint = a.transform.position.y;      
        }

        public void OnDestroy()
        {
            PlaygroundProgressBar.ProgressBarOver -= ProgressBarOnProgressBarOver;
            #if UNITY_WINRT || UNITY_WP8
                WinRTDeviceHelper.FireHideAd();
            #endif
        }

        private void ProgressBarOnProgressBarOver(object sender, EventArgs eventArgs)
        {
            IsGameOver = true;
            GenerateGameOverMenu();
            PlaygroundProgressBar.ProgressBarOver -= ProgressBarOnProgressBarOver;
        }
    }
}