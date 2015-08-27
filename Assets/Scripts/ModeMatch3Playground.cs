using System.Linq;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using UnityEngine;
using Assets.Scripts.Interfaces;
using System;

namespace Assets.Scripts
{
    class ModeMatch3Playground : SquarePlayground
    {
        private readonly RealPoint _initialGameItemX = new RealPoint { X = -13.42F, Y = 12.82F, Z = -1 };
        private const int GameOverPoints = 65536;

        public override IGameSettingsHelper Preferenses
        {
            get { return GameSettingsHelper<ModeMatch3Playground>.Preferenses; }
        }

        public override String ItemPrefabName { get { return ItemPrefabNameHelper.GetPrefabPath<ModeMatch3Playground>(); } }

        public override IPlaygroundSavedata SavedataObject
        {
            get
            {
                var sd = new ModeMatch3PlaygroundSavedata
                {
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
                                sd.Items[i][j] = Items[i][j] != null
                                    ? gobj.GetComponent<GameItem>().Type
                                    : GameItemType.NullItem;
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
            get { return 0.79f; }
        }

        public override float GameItemSize { get { return 3.84f; } }

        ModeMatch3Playground()
        { }

        void OnLevelWasLoaded()
        {
            if (!IsGameOver)
                DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Start, false);
        }

        void Awake()
        {
            //var progressBar = ProgressBar;
            //if (progressBar != null)
            PlaygroundProgressBar.ProgressBarOver += ProgressBarOnProgressBarOver;

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
            MaxType = GameItemType._8;

            IPlaygroundSavedata sd = new ModeMatch3PlaygroundSavedata {Difficulty = Game.Difficulty};
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
                                ? GenerateGameItem(sd.Items[i][j], i, j)
                                : null;
                        }

                    //var score = GetComponentInChildren<Text>();
                    //if (score != null)
                    //    score.text = sd.Score.ToString(CultureInfo.InvariantCulture);

                    CurrentTime = sd.CurrentPlaygroundTime;
                    RisePoints(sd.Score);

                    ProgressBar.InnitializeBar(sd.ProgressBarStateData.State, sd.ProgressBarStateData.Upper, sd.ProgressBarStateData.Multiplier);
                    return;
                }
            }

            //var stat = GetComponent<Game>().Stats;
            //if (stat != null)
            Preferenses.GamesPlayed++;
            
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

        private void ProgressBarOnProgressBarOver(object sender, EventArgs eventArgs)
        {
            IsGameOver = true;
            GenerateGameOverMenu();
            PlaygroundProgressBar.ProgressBarOver -= ProgressBarOnProgressBarOver;
        } 

        public override int ClearChains()
        {
            if (IsGameOver) return -1;

            SelectedPoint1 = null;
            SelectedPoint2 = null;

            var lines = GetAllLines();
            if (lines.Count == 0)
            {
                ChainCounter = 0;
                if (TimeCounter < 0) TimeCounter = 0;
                if (!CheckForPossibleMoves() && DropsCount == 0)
                {
                    LogFile.Message("No moves");
                    GenerateField(false, true);
                }
                UpdateTime();
                SavedataHelper.SaveData(SavedataObject);
                return 0;
            }
            TimeCounter = -1;

            LogFile.Message("Start clear chaines. Lines: " + lines.Count);
            CallbacksCount = lines.Count;
            var linesCount = lines.Count;
            var pointsBank = 0;
            var l = lines.FirstOrDefault();
            while (l != null && !IsGameOver)
            {
                var pointsMultiple = 1;
                var toCell = (GetCellCoordinates(l.X2, l.Y2) + GetCellCoordinates(l.X1, l.Y1)) / 2;

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
                            LogFile.Message("Items[i][l.Y1] == null");
                            continue;
                        }
                        if (IsInAnotherLine(lines, i, l.Y1))
                        {
                            LogFile.Message("Items[" + i + "][" + l.Y1 + "] on another line");
                            continue;
                        }
                        var gobj = Items[i][l.Y1] as GameObject;
                        if (gobj == null) continue;
                        gobj.transform.localPosition = new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y, -0.5f);
                        RemoveGameItem(i, l.Y1);
                    }
                }

                var currentObj = Items[l.X2][l.Y2] as GameObject;
                if (currentObj != null)
                {
                    var gi = currentObj.GetComponent<GameItem>();
                    var points = pointsMultiple * (int)Math.Pow(2, (double)gi.Type);
                    var scalingLabelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                    if (scalingLabelObject != null)
                    {
                        var pointsLabel = scalingLabelObject.GetComponent<LabelShowing>();
                        pointsLabel.transform.SetParent(transform);
                        pointsBank += points;
                        pointsLabel.ShowScalingLabel(new Vector3(toCell.x,toCell.y + GameItemSize/2,toCell.z - 1), "+" + points, GameColors.ItemsColors[gi.Type], Color.gray, 60, 90, null, true, () => RemoveGameItem(l.X2, l.Y2));

                    }
                }
                IsGameOver = CurrentScore >= GameOverPoints;
                CallbacksCount--;
                lines.Remove(l);

                if (linesCount == 1)
                    DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Line, false);

                LogFile.Message("line collected");
                l = lines.FirstOrDefault();

                if (ProgressBar != null)
                    ProgressBar.AddTime(pointsMultiple * 2);

                if (l != null) continue;

                if (linesCount > 1)
                    ShowComboLabel(linesCount);

                pointsBank *= linesCount;
                ChainCounter++;
                RisePoints(pointsBank * ChainCounter);

                pointsBank = 0;

                lines = GetAllLines();
                linesCount = lines.Count;
                l = lines.FirstOrDefault();
            }
            LogFile.Message("All lines collected");

            if (!IsGameOver) return linesCount;
            GenerateGameOverMenu();

			return linesCount;
        }

    }
}
