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
        private readonly RealPoint _initialGameItemX = new RealPoint() { X = -192/*-12.8F*/, Y = 182/*12.1F*/, Z = -1 };
        /*private float _pbState;
        private float _pbUpper;
        private float _pbMultiplier;*/

        protected override String UserHelpPrefix
        {
            get { return "Rhombus"; }
        }

        protected override float HintDelayTime
        {
            get
            {
                switch (Game.Difficulty)
                {
                    case DifficultyLevel._veryhard:
                        return base.HintDelayTime;
                    default:
                        return base.HintDelayTime * 2;
                }
            }
        }


        public static Int32 ToOpenPoints
        {
            get { return ModeMatch3SquarePlayground.GameOverPoints; }
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
                    MaxInitialElementType = this.MaxInitialElementType,
                    Items = new GameItemType[FieldSize][],
                    MovingTypes = new GameItemMovingType[FieldSize][],
                    MovesCount = GameMovesCount,
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

        public override float GameItemSize { get { return 55.5f;/*3.7f;*/ } }

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
            MainMenuScript.UpdateTheme();

            //ProgressBar.ProgressBarOver += ProgressBarOnProgressBarOver;

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

            IsTimeLabelShow = false;

            Preferenses.GamesPlayed++;
            IPlaygroundSavedata sd = new Mode11RhombusPlaygroundSavedata { Difficulty = Game.Difficulty };
            if (SavedataHelper.IsSaveDataExist(sd))
            {
                SavedataHelper.LoadData(ref sd);

                Game.Difficulty = sd.Difficulty;

                CurrentTime = sd.CurrentPlaygroundTime;
                GameMovesCount = sd.MovesCount;

                var mit = ((RhombusPlaygroundSavedata)sd).MaxInitialElementType;
                
                MaxInitialElementType = mit;

                if (mit == MaxInitialElementType)
                    ShowMaxInitialElement();

                /*ProgressBar.InnitializeBar(sd.ProgressBarStateData.State, sd.ProgressBarStateData.Upper, sd.ProgressBarStateData.Multiplier);
                if (!ProgressBar.Exists)
                ProgressBar.CreateBar();*/
                RaisePoints(sd.Score);

                /*_pbState = sd.ProgressBarStateData.State;
                _pbUpper = sd.ProgressBarStateData.Upper;
                _pbMultiplier = sd.ProgressBarStateData.Multiplier;*/

                if (sd.Items != null)
                {
                    for (var i = 0; i < FieldSize; i++)
                        for (var j = 0; j < FieldSize; j++)
                        {
                            Items[i][j] = sd.Items[i][j] != GameItemType.NullItem
                                ? (sd.Items[i][j] == GameItemType.DisabledItem
                                ? DisabledItem
                                : GenerateGameItem(sd.Items[i][j], i, j, new Vector2(i % 2 == 1 ? -i : i, i),null, false, null, null, sd.MovingTypes[i][j]))
                            : null;
                            switch (sd.Items[i][j])
                            {
                                case GameItemType._2x:
                                    Items2XCount++;
                                    break;
                                case GameItemType._ToMoveItem:
                                    ToMoveItemsCount++;
                                    break;
                                case GameItemType._XItem:
                                    XItemsCount++;
                                    break;
                            }
                        }
                    return;
                }
            }

            /*_pbState = ProgressBar.State;
            _pbUpper = ProgressBar.Upper;
            _pbMultiplier = ProgressBar.Multiplier;*/
            

            //if (Preferenses.CurrentItemType == MaxInitialElementType)
            //{
            //    var movesRecord = Preferenses.MovesRecord;
            //    if (movesRecord == 0 || movesRecord < GameMovesCount)
            //        Preferenses.MovesRecord = GameMovesCount;
            //}
            //if (Preferenses.CurrentItemType < MaxInitialElementType)
            //    Preferenses.CurrentItemType = MaxInitialElementType;

            /*ProgressBar.InnitializeBar(PlaygroundProgressBar.ProgressBarBaseSize, ProgressBar.Upper, ProgressBar.Multiplier);
            if (!ProgressBar.Exists)
            ProgressBar.CreateBar();*/
            GenerateField();
            ShowMaxInitialElement();
        }


        protected override void MaxInitialElementTypeRaisedActionsAdditional(object o, EventArgs e)
        {
            switch(Game.Difficulty)
            {
                case DifficultyLevel._hard:
                case DifficultyLevel._veryhard:
                    if (!ProgressBar.Exists)
                    {
                        ProgressBar.ProgressBarOver += ProgressBarOnProgressBarOver;
                        ProgressBar.CreateBar();
                        ProgressBar.UpdateTexture();
                        IsTimeLabelShow = true;
                    }
                    base.MaxInitialElementTypeRaisedActionsAdditional(o, e);
                    break;
            }
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
