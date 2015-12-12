using System;
using System.Linq;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using UnityEngine;
using Assets.Scripts.Interfaces;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class Mode8x8SquarePlayground : SquarePlayground
    {
        private readonly RealPoint _initialGameItemX = new RealPoint() { X = -13.35F, Y = 12.05F, Z = -1 };

        protected override String _userHelpPrefix
        {
            get { return "8x8"; }
        }

        public override IGameSettingsHelper Preferenses
        {
            get { return GameSettingsHelper<Mode8x8SquarePlayground>.Preferenses; }
        }

        public override String ItemPrefabName { get { return ItemsNameHelper.GetPrefabPath<Mode8x8SquarePlayground>(); } }

        public override string ItemBackgroundTextureName { get { return ItemsNameHelper.GetBackgroundTexturePrefix<Mode8x8SquarePlayground>(); } }

        public override IPlaygroundSavedata SavedataObject
        {
            get
            {
                var sd = new Mode8x8SquarePlaygroundSavedata
                {
                    MaxInitialElementType = this.MaxInitialElementType,//MaxType,
                    Items = new GameItemType[FieldSize][],
                    MovingTypes = new GameItemMovingType[FieldSize][],
                    //PlaygroundStat = GetComponent<Game>().Stats,
                    CurrentPlaygroundTime = CurrentTime + Time.timeSinceLevelLoad,
                    Difficulty = Game.Difficulty,
                    ProgressBarStateData = new ProgressBarState { Multiplier = ProgressBar.Multiplier, State = ProgressBar.State, Upper = ProgressBar.Upper }
                };
                if (Items == null)
                {
                    sd.Items = null;
                    sd.MovingTypes = null;
                }
                else
                {
                    for (var i = 0; i < FieldSize; i++)
                    {
                        if (sd.Items[i] == null)
                        {
                            sd.Items[i] = new GameItemType[FieldSize];
                            sd.MovingTypes[i] = new GameItemMovingType[FieldSize];
                        }
                        for (var j = 0; j < FieldSize; j++)
                        {
                            var gobj = Items[i][j] as GameObject;

                            if (gobj != null)
                            {
                                if (Items[i][j] != null && Items[i][j] != DisabledItem)
                                {
                                    var gi = gobj.GetComponent<GameItem>();
                                    sd.Items[i][j] = gi.Type;
                                    sd.MovingTypes[i][j] = gi.MovingType;
                                }
                                else
                                {
                                    sd.Items[i][j] = (Items[i][j] == DisabledItem ? GameItemType.DisabledItem : GameItemType.NullItem);
                                }

                            }
                        }
                    }
                }

                sd.Score = CurrentScore;
                return sd;
            }
        }

        public override RealPoint InitialGameItemPosition { get { return _initialGameItemX; } }

        public override int FieldSize { get { return 8; } }

        public override float GameItemSize { get { return 3.805f; } }

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
                Resources.LoadAll<Sprite>("SD/8x8Atlas")
               .SingleOrDefault(t => t.name.Contains(Game.Theme.ToString())); 

            ProgressBar.ProgressBarOver += ProgressBarOnProgressBarOver;

            Items = new[]
            {
                new System.Object[FieldSize],
                new System.Object[FieldSize],
                new System.Object[FieldSize],
                new System.Object[FieldSize],
                new System.Object[FieldSize],
                new System.Object[FieldSize],
                new System.Object[FieldSize],
                new System.Object[FieldSize]
            };

            IPlaygroundSavedata sd = new Mode8x8SquarePlaygroundSavedata { Difficulty = Game.Difficulty };
            if (SavedataHelper.IsSaveDataExist(sd))
            {
                SavedataHelper.LoadData(ref sd);

                //var gC = GetComponent<Game>();
                //gC.Stats = sd.PlaygroundStat;

                Game.Difficulty = sd.Difficulty;

                CurrentTime = sd.CurrentPlaygroundTime;

                var mit = ((SquarePlaygroundSavedata)sd).MaxInitialElementType;
                if (mit != MaxInitialElementType)
                    MaxInitialElementType = mit;
                else
                    ShowMaxInitialElement();

                ProgressBar.InnitializeBar(sd.ProgressBarStateData.State, sd.ProgressBarStateData.Upper, sd.ProgressBarStateData.Multiplier);
                ProgressBar.CreateBar();
                RaisePoints(sd.Score);

                if (sd.Items != null)
                {
                    for (var i = 0; i < FieldSize; i++)
                        for (var j = 0; j < FieldSize; j++)
                        {
                            Items[i][j] = sd.Items[i][j] != GameItemType.NullItem ? (
                                sd.Items[i][j] != GameItemType.DisabledItem ?
                                GenerateGameItem(sd.Items[i][j], i, j, null, false, null, null, sd.MovingTypes[i][j]) : DisabledItem)
                                : null;
                            switch (sd.Items[i][j])
                            {
                                case GameItemType._2x:
                                    _2xItemsCount++;
                                    break;
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

                   
                    return;
                }
            }

            //var stat = GetComponent<Game>().Stats;
            //if (stat != null)
            //{
            Preferenses.GamesPlayed++;
            if (Preferenses.CurrentItemType < MaxInitialElementType)
                Preferenses.CurrentItemType = MaxInitialElementType;
            //}

            GenerateField();
            ProgressBar.CreateBar();
            ShowMaxInitialElement();
            DifficultyRaisedGUI();
            //var a = Items[FieldSize - 1][FieldSize-1] as GameObject;
            //DownPoint = a.transform.position.y;      

            //var a = Items[FieldSize - 1][FieldSize-1] as GameObject;
            //DownPoint = a.transform.position.y;  
        }

        public void OnDestroy()
        {
            ProgressBar.ProgressBarOver -= ProgressBarOnProgressBarOver;
        }

        private void ProgressBarOnProgressBarOver(object sender, EventArgs eventArgs)
        {
            IsGameOver = true;
            GenerateGameOverMenu();
            ProgressBar.ProgressBarOver -= ProgressBarOnProgressBarOver;
        }
    }
}
