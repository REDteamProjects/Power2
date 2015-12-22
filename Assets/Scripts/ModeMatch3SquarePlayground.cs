using System.Linq;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using UnityEngine;
using Assets.Scripts.Interfaces;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Assets.Scripts
{
    class ModeMatch3SquarePlayground : SquarePlayground
    {
        private readonly RealPoint _initialGameItemX = new RealPoint { X = -13.36F, Y = 12.06F, Z = -1 };
        public static readonly int GameOverPoints = 32768;
        private GameItemType toBlock;
        private GameItemType lastMoved;

        public static Int32 ToOpenPoints
        {
            get { return 81920; }
        }

        protected override String UserHelpPrefix
        {
            get { return "Match3"; }
        }

        public override int CurrentScore
        {
            get { return base.CurrentScore; }
            protected set
            {
                base.CurrentScore = value;
                if (CurrentScore >= Preferenses.ScoreRecord && Preferenses.MovesRecord > GameMovesCount)
                    Preferenses.MovesRecord = GameMovesCount;
                switch (Game.Difficulty)
                {
                    case DifficultyLevel._easy:
                        if (CurrentScore < 4096) return;
                        Game.Difficulty = DifficultyLevel._medium;
                        MoveTimerMultiple = InitialMoveTimerMultiple - 2;
                        DifficultyRaisedGUI(true, MaxInitialElementTypeRaisedActionsAdditional);
                        RaiseMaxInitialElement = true;
                        return;
                    case DifficultyLevel._medium:
                        if (CurrentScore < 12288) return;
                        Game.Difficulty = DifficultyLevel._hard;
                        MoveTimerMultiple = InitialMoveTimerMultiple - 4;
                        DifficultyRaisedGUI(true, MaxInitialElementTypeRaisedActionsAdditional);
                        RaiseMaxInitialElement = true;
                        return;
                    case DifficultyLevel._hard:
                        if (CurrentScore < 20480) return;
                        Game.Difficulty = DifficultyLevel._veryhard;
                        MoveTimerMultiple = InitialMoveTimerMultiple - 8;
                        DifficultyRaisedGUI(true, MaxInitialElementTypeRaisedActionsAdditional);
                        RaiseMaxInitialElement = true;
                        return;
                    case DifficultyLevel._veryhard:
                        if (CurrentScore < GameOverPoints) return;
                        IsGameOver = true;
                        GenerateGameOverMenu(true);
                        return;
                }
            }
        }

        protected override void MaxInitialElementTypeRaisedActions()
        {
            switch (Game.Difficulty)
            {
                case DifficultyLevel._hard:
                    SpawnXItems();
                    break;
                case DifficultyLevel._veryhard:
                    toBlock = (GameItemType)RandomObject.Next(1, (int)MaxType + 1);
                    ProgressBar.TimeBorderActivated += VeryHardLevelAction;
                    ProgressBar.TimeBorderDeActivated += VeryHardLevelActionDeactivate;
                    ProgressBar.SetSmallXsColor(GameColors.Match3Colors[toBlock]);
                    break;
            }
        }

        protected override void VeryHardLevelAction(object sender, EventArgs e)
        {
            /*for (int diag1 = 0,row = FieldSize - 1; diag1 < FieldSize; diag1++,row--)
                {
                    var gobj = Items[diag1][diag1] as GameObject;
                    if (gobj == null) continue;
                    var gi = gobj.GetComponent<GameItem>();
                    if (gi.MovingType != GameItemMovingType.Static)
                        RemoveGameItem(diag1, diag1);
                    gobj = Items[diag1][row] as GameObject;
                    if (gobj == null) continue;
                    gi = gobj.GetComponent<GameItem>();
                    if (gi.MovingType != GameItemMovingType.Static)
                        RemoveGameItem(diag1, row);
                }*/
            for (int col = 0; col < FieldSize; col++)
                for (int row = 0; row < FieldSize; row++)
                    if (Items[col][row] != null)
                    {
                        var gi = (Items[col][row] as GameObject).GetComponent<GameItem>();
                        if (gi.Type == toBlock)
                            gi.MovingType = GameItemMovingType.Static;
                    }
            MoveTimerMultiple = MoveTimerMultiple - 16;
        }

        private void VeryHardLevelActionDeactivate(object sender, EventArgs e)
        {
            for (int col = 0; col < FieldSize; col++)
                for (int row = 0; row < FieldSize; row++)
                    if (Items[col][row] != null)
                    {
                        var gi = (Items[col][row] as GameObject).GetComponent<GameItem>();
                        if (gi.Type == toBlock)
                            gi.MovingType = GameItemMovingType.Standart;
                    }
            toBlock = (GameItemType)RandomObject.Next(1, FieldSize);
            ProgressBar.SetSmallXsColor(GameColors.Match3Colors[toBlock]);
            MoveTimerMultiple = MoveTimerMultiple + 16;
        }

        public override IGameSettingsHelper Preferenses
        {
            get { return GameSettingsHelper<ModeMatch3SquarePlayground>.Preferenses; }
        }

        public override String ItemPrefabName { get { return ItemsNameHelper.GetPrefabPath<ModeMatch3SquarePlayground>(); } }

        public override string ItemBackgroundTextureName { get { return ItemsNameHelper.GetBackgroundTexturePrefix<ModeMatch3SquarePlayground>(); } }

        public override IPlaygroundSavedata SavedataObject
        {
            get
            {
                var sd = new ModeMatch3PlaygroundSavedata
                {
                    Items = new GameItemType[FieldSize][],
                    MovingTypes = new GameItemMovingType[FieldSize][],
                    MovesCount = GameMovesCount,
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

            MaxType = GameItemType._7;
            Preferenses.GamesPlayed++;
            IPlaygroundSavedata sd = new ModeMatch3PlaygroundSavedata { Difficulty = Game.Difficulty };
            if (SavedataHelper.IsSaveDataExist(sd))
            {
                SavedataHelper.LoadData(ref sd);

                Game.Difficulty = sd.Difficulty;

                CurrentTime = sd.CurrentPlaygroundTime;
                ProgressBar.InnitializeBar(sd.ProgressBarStateData.State, sd.ProgressBarStateData.Upper, sd.ProgressBarStateData.Multiplier);
                if (!ProgressBar.Exists)
                    ProgressBar.CreateBar();
                RaisePoints(sd.Score);
                GameMovesCount = sd.MovesCount;

                if (sd.Items != null)
                {
                    for (var i = 0; i < FieldSize; i++)
                        for (var j = 0; j < FieldSize; j++)
                        {
                            Items[i][j] = sd.Items[i][j] != GameItemType.NullItem
                                ? GenerateGameItem(sd.Items[i][j], i, j, null, false, null, null, sd.MovingTypes[i][j])
                                : null;
                            switch (sd.Items[i][j])
                            {
                                case GameItemType._ToMoveItem:
                                    ToMoveItemsCount++;
                                    break;
                                case GameItemType._XItem:
                                    XItemsCount++;
                                    break;
                            }
                        }

                    DifficultyRaisedGUI(true, MaxInitialElementTypeRaisedActionsAdditional);
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
            DifficultyRaisedGUI(true, MaxInitialElementTypeRaisedActionsAdditional);
        }

        public override GameObject GenerateGameItem(int i, int j, IList<GameItemType> deniedTypes = null, Vector2? generateOn = null, bool isItemDirectionChangable = false, float? dropSpeed = null, MovingFinishedDelegate movingCallback = null, GameItemMovingType? movingType = null)
        {
            var newType = RandomObject.Next((int)(/*MaxType > (GameItemType)FieldSize ? MinType + 1 :*/ GameItemType._1), (int)MaxInitialElementType + 1);
            if (deniedTypes == null || deniedTypes.Count == 0)
                return GenerateGameItem((GameItemType)newType, i, j, generateOn, isItemDirectionChangable, dropSpeed, movingCallback, movingType);
            while (deniedTypes.Contains((GameItemType)newType))
                newType = RandomObject.Next((int)GameItemType._1, (int)MaxInitialElementType + 1);
            return GenerateGameItem((GameItemType)newType, i, j, generateOn, isItemDirectionChangable, dropSpeed, movingCallback);
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

        public override int ClearChains()
        {
            if (IsGameOver) return -1;

            SelectedPoint1 = null;
            SelectedPoint2 = null;

            var lines = GetAllLines();
            if (lines.Count == 0)
            {
                if (RaiseMaxInitialElement && CallbacksCount == 0)
                {
                    RaiseMaxInitialElement = false;
                    MaxInitialElementTypeRaisedActions();
                }
                ChainCounter = 0;
                if (HintTimeCounter < 0) HintTimeCounter = 0;

                if (CallbacksCount == 0 && !CheckForPossibleMoves())
                {
                    LogFile.Message("No moves", true);

                    if (lastMoved != GameItemType._ToMoveItem)
                    {
                        GenerateField(false, true, Game.Difficulty != DifficultyLevel._easy, () => CreateInGameHelpModule(UserHelpPrefix + "NoMoves"));
                        if (Game.Difficulty > DifficultyLevel._easy)
                            lastMoved = GameItemType._ToMoveItem;
                    }

                }
                UpdateTime();
                return 0;
            }
            HintTimeCounter = -1;
            _isDropDone = false;
            LogFile.Message("Start clear chaines. Lines: " + lines.Count, true);
            var linesCount = lines.Count;
            var pointsBank = 0;
            var l = lines.FirstOrDefault();
            while (l != null && !IsGameOver)
            {
                var pointsMultiple = 1;

                var currentObj = Items[l.X2][l.Y2] as GameObject;
                var gi = currentObj.GetComponent<GameItem>();
                var cellType = gi.Type;

                if (l.Orientation == LineOrientation.Vertical)
                {
                    pointsMultiple += l.Y2 - l.Y1 - 2;
                    for (var j = l.Y2; j >= l.Y1; j--)
                    {
                        if (Items[l.X1][j] == null || Items[l.X1][j] == DisabledItem)
                            continue;
                        if (LinesWithItem(lines, l.X1, j).Count() > 1)
                            continue;

                        var gobj = Items[l.X1][j] as GameObject;
                        if (gobj == null) continue;
                        gobj.transform.localPosition = new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y, -0.5f);
                        RemoveGameItem(l.X1, j);
                    }
                }
                else
                {
                    pointsMultiple += l.X2 - l.X1 - 2;
                    for (var i = l.X2; i >= l.X1; i--)
                    {
                        if (Items[i][l.Y1] == null || Items[i][l.Y1] == DisabledItem)
                        {
                            LogFile.Message("Items[i][l.Y1] == null", true);
                            continue;
                        }
                        if (LinesWithItem(lines, i, l.Y1).Count() > 1)
                        {
                            LogFile.Message("Items[" + i + "][" + l.Y1 + "] on another line", true);
                            continue;
                        }
                        var gobj = Items[i][l.Y1] as GameObject;
                        if (gobj == null) continue;
                        gobj.transform.localPosition = new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y, -0.5f);
                        RemoveGameItem(i, l.Y1);
                    }
                }

                var points = pointsMultiple * (int)Math.Pow(2, (double)cellType);
                pointsBank += points;
                LabelShowing.ShowScalingLabel(currentObj, "+" + points, GameColors.Match3Colors[cellType], GameColors.DefaultDark, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 3, null, true,
                    null, 0, cellType);

                lines.Remove(l);

                if (linesCount == 1)
                    DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Line, false);

                LogFile.Message("line collected", true);
                l = lines.FirstOrDefault();

                if (ProgressBar != null)
                    ProgressBar.AddTime(pointsMultiple * 2);

                if (l != null) continue;

                if (linesCount > 1)
                    ShowComboLabel(linesCount);

                pointsBank *= linesCount;
                ChainCounter++;
                RaisePoints(pointsBank * ChainCounter * (int)Game.Difficulty);

                pointsBank = 0;

                lines = GetAllLines();
                linesCount = lines.Count;
                l = lines.FirstOrDefault();
            }
            LogFile.Message("All lines collected", true);

            return linesCount;
        }

        public override bool GameItemsExchange(int x1, int y1, int x2, int y2, float speed, bool isReverse, MovingFinishedDelegate exchangeCallback = null)
        {
            var gobj = Items[x1][y1] as GameObject;
            if (gobj != null && Items[x1][y1] != DisabledItem)
            {
                lastMoved = gobj.GetComponent<GameItem>().Type;
            }
            return base.GameItemsExchange(x1, y1, x2, y2, speed, isReverse, exchangeCallback);
        }

    }
}
