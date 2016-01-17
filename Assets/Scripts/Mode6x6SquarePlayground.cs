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
    class Mode6x6SquarePlayground : SquarePlayground
    {
        private readonly RealPoint _initialGameItemX = new RealPoint() { X = -184/*12.8F*/, Y = 176/*12.22F*/, Z = -1 };
        private readonly Vector3 _match3ItemsScale = new Vector3(1.35f, 1.35f, 1f);
        /*private float _pbState;
        private float _pbUpper;
        private float _pbMultiplier;*/

        protected override String UserHelpPrefix()
        {
            if (Game.isExtreme && Game.Difficulty == DifficultyLevel._medium)
            {
                return "Match3";
            }
            return base.UserHelpPrefix();
        }


        protected override float HintDelayTime 
        { 
            get 
            { 
                switch(Game.Difficulty)
                {
                    case DifficultyLevel._veryhard:
                        return base.HintDelayTime;
                    default:
                        return base.HintDelayTime * 2;
                }
            } 
        }

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
                    MaxInitialElementType = this.MaxInitialElementType,
                    Items = new GameItemType[FieldSize][],
                    MovesCount = GameMovesCount,
                    MovingTypes = new GameItemMovingType[FieldSize][],
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

        public override int FieldSize { get { return 6; } }

        public override RealPoint InitialGameItemPosition { get { return _initialGameItemX; } }

        public override float GameItemSize { get { return 73.9f;/*5.14f;*/ } }

        void OnLevelWasLoaded()
        {
            if (!IsGameOver)
                DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Start, false);
        }

        void Awake()
        {
            if (Game.isExtreme)
            {
                var extImg = GameObject.Find("/Foreground/Extreme").GetComponent<Image>();
                if (extImg != null)
                    extImg.color = new Color(255f, 255f, 255f, 1f);
                InitialMoveTimerMultiple = 38;
            }

            LanguageHelper.ActivateSystemLanguage();

            MainMenuScript.UpdateTheme();

            if (Game.isExtreme)
                ProgressBar.ProgressBarOver += ProgressBarOnProgressBarOver;

            Items = new[]
            {
                new System.Object[FieldSize],
                new System.Object[FieldSize],
                new System.Object[FieldSize],
                new System.Object[FieldSize],
                new System.Object[FieldSize],
                new System.Object[FieldSize]
            };

            if(!Game.isExtreme)
            IsTimeLabelShow = false;
            Preferenses.GamesPlayed++;
            IPlaygroundSavedata sd = new Mode6x6SquarePlaygroundSavedata { Difficulty = Game.Difficulty };
            if (SavedataHelper.IsSaveDataExist(sd))
            {
                SavedataHelper.LoadData(ref sd);

                Game.Difficulty = sd.Difficulty;

                CurrentTime = sd.CurrentPlaygroundTime;

                var mit = ((SquarePlaygroundSavedata)sd).MaxInitialElementType;

                GameMovesCount = sd.MovesCount;

                MaxInitialElementType = mit;

                if (mit == MaxInitialElementType)
                    ShowMaxInitialElement();

                if (Game.isExtreme)
                {
                    ProgressBar.InnitializeBar(PlaygroundProgressBar.ProgressBarBaseSize, ProgressBar.Upper, ProgressBar.Multiplier);
                    if (!ProgressBar.Exists)
                        ProgressBar.CreateBar();
                }
                RaisePoints(sd.Score);

                /*_pbState = sd.ProgressBarStateData.State;
                _pbUpper = sd.ProgressBarStateData.Upper;
                _pbMultiplier = sd.ProgressBarStateData.Multiplier;*/

                if (sd.Items != null)
                {
                    for (var i = 0; i < FieldSize; i++)
                        for (var j = 0; j < FieldSize; j++)
                        {
                            Items[i][j] = sd.Items[i][j] != GameItemType.NullItem ? (
                                sd.Items[i][j] != GameItemType.DisabledItem ?
                                GenerateGameItem(sd.Items[i][j], i, j,null, null, false, null, null, sd.MovingTypes[i][j]) : DisabledItem)
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

            if (Game.isExtreme)
            {
                ProgressBar.InnitializeBar(PlaygroundProgressBar.ProgressBarBaseSize, ProgressBar.Upper, ProgressBar.Multiplier);
                if (!ProgressBar.Exists)
                    ProgressBar.CreateBar();
            }

            GenerateField();
            ShowMaxInitialElement();
                  
        }

        protected override void MaxInitialElementTypeRaisedActionsAdditional(object o, EventArgs e)
        {
            if (Game.isExtreme)
            {
                base.MaxInitialElementTypeRaisedActionsAdditional(o, e);
                return;
            }
            if (Game.Difficulty == DifficultyLevel._veryhard && !ProgressBar.Exists)
            {
                ProgressBar.ProgressBarOver += ProgressBarOnProgressBarOver;
                ProgressBar.CreateBar();
                ProgressBar.UpdateTexture();
                IsTimeLabelShow = true;
                base.MaxInitialElementTypeRaisedActionsAdditional(o, e);
            }
        }


        public void OnDestroy()
        {
            ProgressBar.ProgressBarOver -= ProgressBarOnProgressBarOver;
            #if UNITY_WINRT || UNITY_WP8
                WinRTDeviceHelper.FireHideAd();
            #endif
        }

        private void ProgressBarOnProgressBarOver(object sender, EventArgs eventArgs)
        {
            IsGameOver = true;
            GenerateGameOverMenu();
            ProgressBar.ProgressBarOver -= ProgressBarOnProgressBarOver;
        }

        public override GameObject GenerateGameItem(GameItemType itemType, int i, int j, Vector2? generateOn = null, Vector3? scaleTo = null, bool isItemDirectionChangable = false, float? dropSpeed = null, MovingFinishedDelegate movingCallback = null, GameItemMovingType? movingType = null)
        {
            return base.GenerateGameItem(itemType, i, j, generateOn, Game.isExtreme && itemType == GameItemType._ToMoveItem ? _match3ItemsScale : scaleTo, isItemDirectionChangable, dropSpeed, movingCallback, movingType);
        }
    }
}