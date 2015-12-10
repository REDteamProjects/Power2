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
    class Mode11RhombusPlayground : RhombusPlayground
    {
        private readonly RealPoint _initialGameItemX = new RealPoint() { X = -12.8F, Y = 12.1F, Z = -1 };

        protected override String _userHelpPrefix
        {
            get { return "Rhombus"; }
        }


        public override IGameSettingsHelper Preferenses
        {
            get { return GameSettingsHelper<Mode11RhombusPlayground>.Preferenses; }
        }

        public override IPlaygroundSavedata SavedataObject
        {
            get
            {
                var sd = new Mode11RhombusPlaygroundSavedata
                {
                    MaxInitialElementType = this.MaxInitialElementType,//MaxType,
                    Items = new GameItemType[FieldSize][],
                    MovingTypes = new GameItemMovingType[FieldSize][],
                    //PlaygroundStat = GetComponent<Game>().Stats,
                    CurrentPlaygroundTime = CurrentTime + Time.timeSinceLevelLoad,
                    Difficulty = Game.Difficulty,
                    ProgressBarStateData = new ProgressBarState { Multiplier = ProgressBar.Multiplier, State = ProgressBar.State, Upper = ProgressBar.Upper}
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
                            sd.Items[i] = new GameItemType[Items[i].Length];
                            sd.MovingTypes[i] = new GameItemMovingType[FieldSize];
                        }
                        for (var j = 0; j < Items[i].Length; j++)
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

        public override int FieldSize { get { return 11; } }

        public override float GameItemSize { get { return 3.7f; } }

        //public override GameItemType MaxInitialElementType
        //{
        //    get { return MaxType; }
        //    set
        //    {
        //        if (MaxType == value) return;
        //        MaxType = value;
        //        ShowMaxInitialElement();
        //    }
        //}

        void OnLevelWasLoaded()
        {
            if (!IsGameOver)
                DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Start, false);
        }

        void Awake()
        {
            
            GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor =
                                GameColors.BackgroundColor;

           /* GameObject.Find("PauseButton").GetComponent<Image>().color =
                GameColors.ForegroundButtonsColor;*/

            GameObject.Find("BackgroundGrid").GetComponent<Image>().sprite =
                Resources.LoadAll<Sprite>("SD/RhombusAtlas")
               .SingleOrDefault(t => t.name.Contains(Game.Theme.ToString()));

            PlaygroundProgressBar.ProgressBarOver += ProgressBarOnProgressBarOver;

            Items = new[]
            {
                new []{ DisabledItem, DisabledItem, DisabledItem, DisabledItem, DisabledItem, null,  DisabledItem, DisabledItem, DisabledItem, DisabledItem, DisabledItem },
                new []{ DisabledItem, DisabledItem, DisabledItem, DisabledItem, null, DisabledItem, null, DisabledItem, DisabledItem, DisabledItem, DisabledItem },
                new []{ DisabledItem, DisabledItem, DisabledItem, null, DisabledItem, null,  DisabledItem, null, DisabledItem, DisabledItem, DisabledItem },
                new []{ DisabledItem, DisabledItem, null, DisabledItem, null, DisabledItem, null, DisabledItem, null, DisabledItem, DisabledItem },
                new []{ DisabledItem, null, DisabledItem, null, DisabledItem, null,  DisabledItem, null, DisabledItem, null, DisabledItem },
                new []{ null, DisabledItem, null, DisabledItem, null, DisabledItem, null, DisabledItem, null, DisabledItem, null },
                new []{ DisabledItem, null, DisabledItem, null, DisabledItem, null,  DisabledItem, null, DisabledItem, null, DisabledItem },
                new []{ DisabledItem, DisabledItem, null, DisabledItem, null, DisabledItem, null, DisabledItem, null, DisabledItem, DisabledItem },
                new []{ DisabledItem, DisabledItem, DisabledItem, null, DisabledItem, null,  DisabledItem, null, DisabledItem, DisabledItem, DisabledItem },
                new []{ DisabledItem, DisabledItem, DisabledItem, DisabledItem, null, DisabledItem, null, DisabledItem, DisabledItem, DisabledItem, DisabledItem },
                new []{ DisabledItem, DisabledItem, DisabledItem, DisabledItem, DisabledItem, null,  DisabledItem, DisabledItem, DisabledItem, DisabledItem, DisabledItem },
            };

            

            IPlaygroundSavedata sd = new Mode11RhombusPlaygroundSavedata { Difficulty = Game.Difficulty };
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
                            Items[i][j] = sd.Items[i][j] != GameItemType.NullItem
                                ? (sd.Items[i][j] == GameItemType.DisabledItem
                                ? DisabledItem
                                : GenerateGameItem(sd.Items[i][j], i, j, new Vector2(i % 2 == 1 ? -i : i, i), false, null, null, sd.MovingTypes[i][j]))
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

                    Game.Difficulty = sd.Difficulty;

                    CurrentTime = sd.CurrentPlaygroundTime;

                    var mit = ((RhombusPlaygroundSavedata)sd).MaxInitialElementType;
                    if (mit != MaxInitialElementType)
                        MaxInitialElementType = mit;
                    else
                        ShowMaxInitialElement();

                    ProgressBar.InnitializeBar(sd.ProgressBarStateData.State, sd.ProgressBarStateData.Upper, sd.ProgressBarStateData.Multiplier);
                    ProgressBar.CreateBar();
                    RaisePoints(sd.Score);
                    DifficultyRaisedGUI();
                    return;
                }
            }


            Preferenses.GamesPlayed++;
            if (Preferenses.CurrentItemType < MaxInitialElementType)
                Preferenses.CurrentItemType = MaxInitialElementType;


            GenerateField();
            ProgressBar.CreateBar();
            ShowMaxInitialElement();
            DifficultyRaisedGUI();
        }

        public void OnDestroy()
        {
            PlaygroundProgressBar.ProgressBarOver -= ProgressBarOnProgressBarOver;
        }

        private void ProgressBarOnProgressBarOver(object sender, EventArgs eventArgs)
        {
            IsGameOver = true;
            GenerateGameOverMenu();
            PlaygroundProgressBar.ProgressBarOver -= ProgressBarOnProgressBarOver;
        }

    }
}
