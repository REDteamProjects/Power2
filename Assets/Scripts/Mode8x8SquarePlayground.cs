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
        private readonly RealPoint _initialGameItemX = new RealPoint() { X = -192/*-13.35F*/, Y = 172/*12.05F*/, Z = -1 };
        private readonly Vector3 _gameItemScale = new Vector3(0.74f, 0.74f, 1f);

        protected override String UserHelpPrefix
        {
            get { return "8x8"; }
        }

        protected override Vector3 GameItemScale
        {
            get { return _gameItemScale; }
        }

        public override float ItemSpeedMultiplier
        {
            get { return _gameItemScale.x; }
        }

        public static Int32 ToOpenPoints
        {
            get { return 65536; }
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

        public override RealPoint InitialGameItemPosition { get { return _initialGameItemX; } }

        public override int FieldSize { get { return 8; } }

        public override float GameItemSize { get { return 55f;/*3.805f;*/ } }

        void OnLevelWasLoaded()
        {
            if (!IsGameOver)
                DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Start, false);
        }

        void Awake()
        {
            LanguageHelper.ActivateSystemLanguage();

            MainMenuScript.UpdateTheme();

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
            Preferenses.GamesPlayed++;
            IPlaygroundSavedata sd = new Mode8x8SquarePlaygroundSavedata { Difficulty = Game.Difficulty };
            if (SavedataHelper.IsSaveDataExist(sd))
            {
                SavedataHelper.LoadData(ref sd);

                Game.Difficulty = sd.Difficulty;

                CurrentTime = sd.CurrentPlaygroundTime;

                GameMovesCount = sd.MovesCount;

                var mit = ((SquarePlaygroundSavedata)sd).MaxInitialElementType;
                if (mit != MaxInitialElementType)
                    MaxInitialElementType = mit;
                else
                    ShowMaxInitialElement();

                ProgressBar.InnitializeBar(sd.ProgressBarStateData.State, sd.ProgressBarStateData.Upper, sd.ProgressBarStateData.Multiplier);
                if(!ProgressBar.Exists)
                ProgressBar.CreateBar();
                RaisePoints(sd.Score);

                if (sd.Items != null)
                {
                    for (var i = 0; i < FieldSize; i++)
                        for (var j = 0; j < FieldSize; j++)
                        {
                            Items[i][j] = sd.Items[i][j] != GameItemType.NullItem ? (
                                sd.Items[i][j] != GameItemType.DisabledItem ?
                                GenerateGameItem(sd.Items[i][j], i, j, null, GameItemScale, false, null, null, sd.MovingTypes[i][j]) : DisabledItem)
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

            //if (Preferenses.CurrentItemType == MaxInitialElementType)
            //{
            //    var movesRecord = Preferenses.MovesRecord;
            //    if (movesRecord == 0 || movesRecord < GameMovesCount)
            //        Preferenses.MovesRecord = GameMovesCount;
            //}
            //if (Preferenses.CurrentItemType < MaxInitialElementType)
            //    Preferenses.CurrentItemType = MaxInitialElementType;

            ProgressBar.InnitializeBar(PlaygroundProgressBar.ProgressBarBaseSize, ProgressBar.Upper, ProgressBar.Multiplier);
            
            if (!ProgressBar.Exists)
                ProgressBar.CreateBar();
            
            GenerateField();
            ShowMaxInitialElement();
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
