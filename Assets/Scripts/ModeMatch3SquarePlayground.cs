using System.Linq;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using UnityEngine;
using Assets.Scripts.Interfaces;
using System;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class ModeMatch3SquarePlayground : SquarePlayground
    {
        private readonly RealPoint _initialGameItemX = new RealPoint { X = -13.36F, Y = 12.06F, Z = -1 };
        private const int GameOverPoints = 32768;
        private GameItemType toBlock;

        protected override String _userHelpPrefix
        {
            get { return "Match3"; }
        }

        public override int CurrentScore
        {
            get { return base.CurrentScore; }
            protected set
            {
                base.CurrentScore = value;
                switch(Game.Difficulty)
                {
                    case DifficultyLevel._easy:
                        if (CurrentScore < 8192) return;
                        Game.Difficulty = DifficultyLevel._medium;
                        MoveTimerMultiple = _initialMoveTimerMultiple + 4;
                        DifficultyRaisedGUI();
                        _raiseMaxInitialElement = true;
                        return;
                    case DifficultyLevel._medium:
                        if (CurrentScore < 16384) return;
                        Game.Difficulty = DifficultyLevel._hard;
                        MoveTimerMultiple = _initialMoveTimerMultiple + 8;
                        DifficultyRaisedGUI();
                        _raiseMaxInitialElement = true;
                        return;
                    case DifficultyLevel._hard:
                        if (CurrentScore < 24576) return;
                        Game.Difficulty = DifficultyLevel._veryhard;
                        MoveTimerMultiple = _initialMoveTimerMultiple + 12;
                        DifficultyRaisedGUI();
                        _raiseMaxInitialElement = true;
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
            /*if (_showTimeLabel)
            {
                _showTimeLabel = false;
                ShowTimeLabel();
            }
            else
                PlaygroundProgressBar.ProgressBarRun = true;*/
            switch(Game.Difficulty)
            {
                case DifficultyLevel._hard:
                    SpawnXItems();
                    break;
                case DifficultyLevel._veryhard:
                    toBlock = (GameItemType)RandomObject.Next(1, FieldSize);
                    ProgressBar.TimeBorderActivated += VeryHardLevelAction;
                    ProgressBar.TimeBorderDeActivated += VeryHardLevelActionDeactivate;
                    ProgressBar.SetSmallXsColor(GameColors.ItemsColors[toBlock]);
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
        }

        private void VeryHardLevelActionDeactivate(object sender, EventArgs e)
        {
            for (int col = 0; col < FieldSize; col++)
                for (int row = 0; row < FieldSize; row++)
                    if (Items[col][row] != null)
                    {
                        var gi = (Items[col][row] as GameObject).GetComponent<GameItem>();
                        if(gi.Type == toBlock)
                        gi.MovingType = GameItemMovingType.Standart;
                    }
           toBlock = (GameItemType)RandomObject.Next(1, FieldSize);
           ProgressBar.SetSmallXsColor(GameColors.ItemsColors[toBlock]);
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

        public override float ScaleMultiplyer
        {
            get { return 5.4f; }
        }

        public override float GameItemSize { get { return 3.805f; } }

        ModeMatch3SquarePlayground()
        {          
        }

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


            MaxType = GameItemType._7;
            _raiseMaxInitialElement = true;

           

            IPlaygroundSavedata sd = new ModeMatch3PlaygroundSavedata {Difficulty = Game.Difficulty};
            if (SavedataHelper.IsSaveDataExist(sd))
            {
                SavedataHelper.LoadData(ref sd);
               
                //var gC = GetComponent<Game>();
                //gC.Stats = sd.PlaygroundStat;

                Game.Difficulty = sd.Difficulty;

                CurrentTime = sd.CurrentPlaygroundTime;
                ProgressBar.InnitializeBar(sd.ProgressBarStateData.State, sd.ProgressBarStateData.Upper, sd.ProgressBarStateData.Multiplier);
                if (!ProgressBar.Exists)
                ProgressBar.CreateBar();
                RaisePoints(sd.Score);

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
            Preferenses.GamesPlayed++;
            ProgressBar.InnitializeBar(PlaygroundProgressBar.ProgressBarBaseSize, ProgressBar.Upper, ProgressBar.Multiplier);
            if (!ProgressBar.Exists)
            ProgressBar.CreateBar();
            GenerateField();
                //ShowMaxInitialElement();
                //var a = Items[FieldSize - 1][FieldSize-1] as GameObject;
                //DownPoint = a.transform.position.y;      

            //var a = Items[FieldSize - 1][FieldSize-1] as GameObject;
            //DownPoint = a.transform.position.y;  
            //var progressBar = ProgressBar;
            //if (progressBar != null)
            //    PlaygroundProgressBar.ProgressBarOver += ProgressBarOnProgressBarOver;
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
                if (_raiseMaxInitialElement && CallbacksCount == 0)
                {
                    _raiseMaxInitialElement = false;
                    MaxInitialElementTypeRaisedActions();
                }
                ChainCounter = 0;
                if (HintTimeCounter < 0) HintTimeCounter = 0;
                if (DropsCount == 0 && !CheckForPossibleMoves())
                {
                    LogFile.Message("No moves", true);
                    GenerateField(false, true, Game.Difficulty == DifficultyLevel._easy ? true : false);
                }
                UpdateTime();
                return 0;
            }
            HintTimeCounter = -1;

            LogFile.Message("Start clear chaines. Lines: " + lines.Count, true);
            CallbacksCount = lines.Count;
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
                        if (Items[l.X1][j] == null)
                        {
                            //LogFile.Message("Items[i][l.Y1] == null");
                            continue;
                        }
                        if (IsInAnotherLine(lines, l.X1, j))
                        {
                            //LogFile.Message("Items[" + l.X1 + "][" + j + "] = null;");
                            continue;
                        }
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
                        if (Items[i][l.Y1] == null)
                        {
                            LogFile.Message("Items[i][l.Y1] == null", true);
                            continue;
                        }
                        if (IsInAnotherLine(lines, i, l.Y1))
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

                //var currentObj = Items[l.X2][l.Y2] as GameObject;
                //if (currentObj != null)
                //{
                    //var gi = currentObj.GetComponent<GameItem>();
                    var points = pointsMultiple * (int)Math.Pow(2, (double)cellType);
                    /*var scalingLabelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                    if (scalingLabelObject != null)
                    {
                        var pointsLabel = scalingLabelObject.GetComponent<LabelShowing>();
                        pointsLabel.transform.SetParent(transform);*/
                        pointsBank += points;
                        LabelShowing.ShowScalingLabel(currentObj, "+" + points, GameColors.Match3Colors[cellType], Color.gray, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 3, null, true,
                            null, 0, cellType);

                    //}
                //}
                CallbacksCount--;
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

    }
}
