﻿using System;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using UnityEngine;
using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using Assets.Scripts.Helpers;

namespace Assets.Scripts
{
    class ModeDropsPlayground : SquarePlayground
    {
        private readonly RealPoint _initialGameItemX = new RealPoint { X = -13.42F, Y = 12.82F, Z = -1 };
        private const int GameOverPoints = 65536;

        private Point _currentDroppingItem;

        //protected override float ScaleMultiplyer
        //{
        //    get { return 1f; }
        //}

        public Point CurrentDroppingItem 
        {
            get { return _currentDroppingItem; }
            private set { if (_currentDroppingItem != value) _currentDroppingItem = value; }
        }

        public override IGameSettingsHelper Preferenses
        {
            get { return GameSettingsHelper<ModeDropsPlayground>.Preferenses; }
        }

        public override String ItemPrefabName { get { return ItemsNameHelper.GetPrefabPath<ModeDropsPlayground>(); } }

        public override string ItemBackgroundTextureName { get { return ItemsNameHelper.GetBackgroundTexturePrefix<ModeDropsPlayground>(); } }

        //public override bool AreStaticItemsDroppable { get { return true; } }

        public override bool isDisabledItemActive { get { return true; } }

        public override IPlaygroundSavedata SavedataObject
        {
            get
            {
                var sd = new ModeDropsPlaygroundSavedata
                {
                    MaxInitialElementType = this.MaxInitialElementType,
                    Items = new GameItemType[FieldSize][],
                    MovesCount = GameMovesCount,
                    CurrentPlaygroundTime = CurrentTime + Time.timeSinceLevelLoad,
                    Difficulty = Game.Difficulty,
                    
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
                                sd.Items[i][j] = Items[i][j] != null && Items[i][j] != DisabledItem ? gobj.GetComponent<GameItem>().Type
                                    : (Items[i][j] == DisabledItem ? GameItemType.DisabledItem : GameItemType.NullItem);
                        }
                    }
                }

                sd.Score = CurrentScore;
                return sd;
            }
        }

        public override RealPoint InitialGameItemPosition { get { return _initialGameItemX; } }

        public override int FieldSize { get { return 8; } }

        public override float GameItemSize { get { return 3.84f; } }

        public override void GenerateField(bool completeCurrent = false, bool mixCurrent = false, bool onlyNoMovesLabel = false, LabelAnimationFinishedDelegate callback = null)
        {
            //if (CallbacksCount > 1) return;

            while (true)
            {
                if (!completeCurrent && !mixCurrent)
                {
                    for (var i = 0; i < FieldSize; i++)
                        for (var j = 0; j < FieldSize; j++)
                        {
                            Items[i][j] = DisabledItem;
                        }
                    completeCurrent = true;
                    continue;
                }
                if (completeCurrent)
                {
                    var availibleColumns = new Dictionary<int, int>();
                    for (var i = 0; i < FieldSize; i++)
                        for (var j = FieldSize - 1; j >= 0; j--)
                        {
                            if (Items[i][j] == null)
                                Items[i][j] = DisabledItem;

                            if (Items[i][j] == DisabledItem && !availibleColumns.ContainsKey(i))
                                availibleColumns.Add(i, j);
                        }
                    int respCol;
                    if (availibleColumns.Count == 0)
                    {
                        completeCurrent = false;
                        mixCurrent = true;
                        continue;
                    }
                    while (!availibleColumns.ContainsKey(respCol = RandomObject.Next(0, FieldSize))) { }
                    //var res1 = RandomObject.Next(0, FieldSize);
                    switch (Game.Difficulty)
                    {
                        case DifficultyLevel._medium:
                        case DifficultyLevel._hard:
                        case DifficultyLevel._veryhard:
                            if (XItemsCount < MaxAdditionalItemsCount)
                            {
                                if (RandomObject.Next(0, FieldSize) % 3 == 0)
                                {
                                    GenerateDropsModeItem(respCol, availibleColumns[respCol], GameItemType._XItem);
                                    XItemsCount++;
                                    return;
                                }
                            }
                            if (ToMoveItemsCount < MaxAdditionalItemsCount)
                            {
                                if (RandomObject.Next(0, FieldSize) % 3 == 0)
                                {
                                    if (Items[respCol][availibleColumns[respCol]] == DisabledItem)
                                        break;

                                    GenerateDropsModeItem(respCol, availibleColumns[respCol], GameItemType._ToMoveItem);
                                    ToMoveItemsCount++;
                                    return;
                                }
                            }
                            break;
                    }
                    GenerateDropsModeItem(respCol, availibleColumns[respCol]);
                }
                if (mixCurrent && !IsGameOver)
                {
                    IsGameOver = true;
                    //var labelObject = Instantiate(LabelShowing.LabelPrefab) as GameObject;
                    //if (labelObject != null)
                    //{
                    //    var gameOverLabel = labelObject.GetComponent<LabelShowing>();
                    //    gameOverLabel.transform.SetParent(transform);
                    //    gameOverLabel.ShowScalingLabel(new Vector3(0, 0, -3),
                    //        "Game over", Color.white, Color.gray, 60, 90);
                    //}
                    ////TODO: stop the game
                    GenerateGameOverMenu();
                }
                break;
            }
        }

        public override bool GameItemsExchange(int x1, int y1, int x2, int y2, float speed, bool isReverse, MovingFinishedDelegate exchangeCallback = null)
        {
            var gobj = Items[x1][y1] as GameObject;
            if (gobj == null) return false;
            var gims = gobj.GetComponent<GameItemMovingScript>();
            if (!gims.IsMoving)
            {
                var res = base.GameItemsExchange(x1, y1, x2, y2, speed, isReverse, exchangeCallback ?? ((go, r) =>
                {
                    if (CallbacksCount != 1 || isReverse) return;
                    while (ClearChains() > 0)
                    {
                        RemoveAdditionalItems(); //TODO: ?
                    }
                }));

                return res;
            }
            if (isReverse) return true;

            y2 = -1;
            //TODO: add check on localPositionCoordinates
            for (var i = FieldSize - 1; i >= 0; i--)
            {

                if (Items[x2][i] != DisabledItem) continue;
                y2 = i;
                break;
            }
            if (y2 < 0)
                return false;
            
            var item1 = Items[x1][y1] as GameObject;
            var item = item1.GetComponent<GameItemMovingScript>();
            if (!item.IsMoving) return false;
            Items[x1][y1] = Items[x2][y2];
            Items[x2][y2] = item1;
            var toPoint = GetCellCoordinates(x2, y2);
            var cdc = item.CurrentDestination;
            item.ChangeDirection(toPoint.x, item.transform.localPosition.y - GameItemSize / 5,
                /* cdc.Speed.y + 4*/ speed,
                (mItem, result) =>
                   item.MoveTo(toPoint.x, toPoint.y, cdc.Speed, cdc.MovingCallback, null, null, true));
            CurrentDroppingItem = new Point{X = x2, Y = y2};

            //item.ChangeDirection(toPoint.x, toPoint.y >= (item.transform.localPosition.y - GameItemSize / 5) ? toPoint.y : item.transform.localPosition.y - GameItemSize / 5,
            //    /* cdc.Speed.y + 4*/ speed, 
            //    (mItem, result) =>
            //       item.MoveTo(toPoint.x, toPoint.y, cdc.Speed.y, cdc.MovingCallback, null, null, true));



            //item.MoveTo(toPoint.x,
            //    toPoint.y >= (item.transform.localPosition.y - GameItemSize/5)
            //        ? toPoint.y
            //        : item.transform.localPosition.y - GameItemSize/5,
            //    speed, null, null, null, true);
            //item.MoveTo(toPoint.x, toPoint.y, cdc.Speed.y, cdc.MovingCallback);
            //item.CancelMoving();

            return true;
        }

        public override void RevertMovedItem(int col, int row, MovingFinishedDelegate callback = null)
        {
            if (Items[col][row] == null || Items[col][row] == DisabledItem) return;
            var item1 = Items[col][row] as GameObject;
            if (item1 == null) return;
            var item = item1.GetComponent<GameItemMovingScript>();
            if (item.IsMoving) return;
            base.RevertMovedItem(col, row, callback);
            //return;
            //var toPoint = GetCellCoordinates(col, row);
            //var cdc = item.CurrentDestination;
            //item.ChangeDirection(toPoint.x, toPoint.y >= (item.transform.localPosition.y - GameItemSize) ? toPoint.y : item.transform.localPosition.y - GameItemSize,
            //    cdc.Speed.y,
            //    (mItem, result) =>
            //        item.MoveTo(toPoint.x, toPoint.y, cdc.Speed.y, cdc.MovingCallback, null, null, true));
            
            //item.MoveTo(toPoint.x, toPoint.y >= (item.transform.localPosition.y - GameItemSize) ? toPoint.y : item.transform.localPosition.y - GameItemSize,
            //    cdc.Speed.y, null, null, null, true);
            //item.MoveTo(toPoint.x, toPoint.y, cdc.Speed.y, cdc.MovingCallback);
            //item.CancelMoving();
        }

        public override bool CheckForPossibleMoves()
        {
            for (var col = 0; col < FieldSize; col++)
                for (var row = 0; row < FieldSize; row++)
                {
                    if (Items[col][row] == DisabledItem)
                         return true;
                }
            return false;
        }

        public override bool TryMakeMove(int x1, int y1, int x2, int y2)
        {
            var gobj = Items[x1][y1] as GameObject;
            if (gobj == null) return false;
            var gims = gobj.GetComponent<GameItemMovingScript>();
            return gims.IsMoving || base.TryMakeMove(x1, y1, x2, y2);
        }

        protected override void Update()
        {
            if (Items == null || CallbacksCount > 1 || DropsCount > 1) return;
            ////ClearChains();

            if (CallbacksCount > 1 || DropsCount != 0) return;
            Drop();

            //if (CallbacksCount != 0) return;
            //RemoveAdditionalItems();
        }

        public override bool IsItemMovingAvailable(int col, int row, MoveDirections mdir)
        {
            var gobj = Items[col][row] as GameObject;
            if (gobj == null) return false;
            var gims = gobj.GetComponent<GameItemMovingScript>();
            if (gims == null) return false;
            if (!gims.IsMoving) return base.IsItemMovingAvailable(col, row, mdir);
            var target = GetCellCoordinates(col, row);
            var fourthPart = GameItemSize / 4;
            if (Math.Abs(target.y - gobj.transform.localPosition.y) < fourthPart)
                return false;
            switch (mdir)
            {
                case MoveDirections.Left:
                    if (col == 0 || Items[col - 1][row] == null)
                        return false;
                    for (var i = FieldSize - 1; i >= 0; i--)
                        if (Items[col - 1][i] == DisabledItem)
                        {
                            return GetCellCoordinates(col - 1, i).y < gobj.transform.localPosition.y - fourthPart;
                        }
                    break;
                case MoveDirections.Right:
                    if (col == (FieldSize - 1) || Items[col + 1][row] == null)
                        return false;
                    for (var i = FieldSize - 1; i >= 0; i--)
                        if (Items[col + 1][i] == DisabledItem)
                        {
                            return GetCellCoordinates(col + 1, i).y < gobj.transform.localPosition.y - fourthPart;
                        }
                    break;
            }
            return false;
        }

        public override void Drop()
        {
            var generateAfterDrop = true;

            var counter = 0;

            for (var col = 0; col < FieldSize; col++)
            {
                for (var row = 0; row < FieldSize - 1; row++)
                {
                    if (Items[col][row] == null || Items[col][row] == DisabledItem)
                        continue;

                    var o = Items[col][row] as GameObject;
                    if (o != null &&
                        (!AreStaticItemsDroppable && o.GetComponent<GameItem>().MovingType == GameItemMovingType.Static))
                        continue;

                    if (Items[col][row + 1] != null)
                    {
                        var gameObject1 = Items[col][row + 1] as GameObject;
                        if (gameObject1 != null && gameObject1.GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                        {
                            var rowStaticCounter = 1;
                            var o1 = Items[col][row + rowStaticCounter] as GameObject;
                            while (o1 != null &&
                                   ((row + rowStaticCounter) < FieldSize && Items[col][row + rowStaticCounter] != null
                                    && o1.GetComponent<GameItem>().MovingType == GameItemMovingType.Static))
                                rowStaticCounter++;
                            if ((row + rowStaticCounter) >= FieldSize || Items[col][row + rowStaticCounter] != null)
                                continue;
                            var gobjS = Items[col][row] as GameObject;
                            if (gobjS == null) continue;
                            var cS = gobjS.GetComponent<GameItemMovingScript>();
                            if (cS.IsMoving) continue;
                            counter++;
                            Items[col][row + rowStaticCounter] = Items[col][row];
                            Items[col][row] = null;
                            if (!cS.IsMoving) DropsCount++;
                            var colS = col;
                            var rowS = row;
                            cS.MoveTo(null, GetCellCoordinates(col, row + rowStaticCounter).y, Game.StandartItemSpeed, (item, result) =>
                            {
                                if (!cS.IsMoving)
                                    DropsCount--;
                                if (!result) return;
                                LogFile.Message("New item droped Items[" + colS + "][" + rowS + "] DC: " + DropsCount, true);
                            });
                        }
                        if (row + 2 < FieldSize && Items[col][row + 2] == null) generateAfterDrop = false;
                        continue;
                    }

                    var gobj = Items[col][row] as GameObject;
                    if (gobj == null) continue;
                    var c = gobj.GetComponent<GameItemMovingScript>();
                    if (c.IsMoving) continue;
                    counter++;

                    Items[col][row + 1] = Items[col][row];
                    Items[col][row] = null;
                    
                    if (row + 2 < FieldSize && Items[col][row + 2] == null) generateAfterDrop = false;
                    //if (!c.isMoving)
                        DropsCount++;
                    
                    var col1 = col;
                    var row1 = row;
                    c.MoveTo(null, GetCellCoordinates(col, row + 1).y, Game.StandartItemSpeed, (item, result) =>
                    {
                        //if (!c.isMoving)
                            DropsCount--;
                        if (!result) return;
                        if (DropsCount == 0)
                            while (ClearChains() > 0)
                                RemoveAdditionalItems();

                        LogFile.Message("New item droped Items[" + col1 + "][" + row1 + "] DC: " + DropsCount, true);
                    });
                }
            }
            if (generateAfterDrop)
            {
                _isDropDone = true;
                GenerateField(true);
            }
        }

        void OnLevelWasLoaded()
        {
            if (!IsGameOver)
                DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Start, false);
        }

        public void OnDestroy()
        {
            GameItemMovingScript.MovingItemFinished -= GameItemMovingScriptMovingItemFinished;
        }

        void Awake()
        {
            LanguageHelper.ActivateSystemLanguage();


            MainMenuScript.UpdateTheme();

            GameItemMovingScript.MovingItemFinished += GameItemMovingScriptMovingItemFinished;

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
            IPlaygroundSavedata sd = new ModeDropsPlaygroundSavedata { Difficulty = Game.Difficulty };
            if (SavedataHelper.IsSaveDataExist(sd))
            {
                SavedataHelper.LoadData(ref sd);

                if (sd.Items != null)
                {
                    for (var i = 0; i < FieldSize; i++)
                        for (var j = 0; j < FieldSize; j++)
                        {
                            Items[i][j] = sd.Items[i][j] != GameItemType.NullItem
                                ? (sd.Items[i][j] == GameItemType.DisabledItem
                                    ? DisabledItem
                                    : GenerateGameItem(sd.Items[i][j], i, j))
                                : null;


                            var newGameItem = Items[i][j] as GameObject;
                            switch (sd.Items[i][j])
                            {
                                case GameItemType._ToMoveItem:
                                    if (newGameItem != null)
                                        newGameItem.GetComponent<GameItem>().IsDraggableWhileMoving = true;
                                    ToMoveItemsCount++;
                                    break;
                                case GameItemType._XItem:
                                    if (newGameItem != null)
                                        newGameItem.GetComponent<GameItem>().IsDraggableWhileMoving = true;
                                    XItemsCount++;
                                    break;
                                case GameItemType.DisabledItem:
                                case GameItemType.NullItem:
                                case GameItemType._2x:
                                    break;
                                default:
                                    if (newGameItem != null)
                                        newGameItem.GetComponent<GameItem>().IsDraggableWhileMoving = true;
                                    break;

                            }
                        }

                    Game.Difficulty = sd.Difficulty;

                    CurrentTime = sd.CurrentPlaygroundTime;
                    GameMovesCount = sd.MovesCount;
                    var mit = ((SquarePlaygroundSavedata)sd).MaxInitialElementType;
                    if (mit != MaxInitialElementType)
                        MaxInitialElementType = mit;
                    else
                        ShowMaxInitialElement();
                    GenerateField(true);
                    RaisePoints(sd.Score);
                    

                    DifficultyRaisedGUI();
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


            GenerateField();
            ShowMaxInitialElement();

            DifficultyRaisedGUI();
        }

        void GameItemMovingScriptMovingItemFinished(object sender, int count)
        {
            //if (Items == null || CallbacksCount > 1 || DropsCount > 1) return;
            //while (ClearChains() > 0)
            //{
            //    RemoveAdditionalItems();
            //}
        }

        private void GenerateDropsModeItem(int col, int row, GameItemType type = GameItemType.NullItem)
        {
            var callback = (MovingFinishedDelegate)((sender, result) =>
            {
                CurrentDroppingItem = null;
                while (ClearChains() > 0)
                {
                    RemoveAdditionalItems();
                }
            })
                ;
            var gameItem = (type != GameItemType.NullItem) ?
                GenerateGameItem(type, col, row, new Vector2(0, 1),null, true, 2, callback)
                : GenerateGameItem(col, row, null, new Vector2(0, 1), true, 2, callback);

            if (gameItem != null)
                gameItem.GetComponent<GameItem>().IsDraggableWhileMoving = true;
            Items[col][row] = gameItem;

            CurrentDroppingItem = new Point{X = col, Y = row};
        }
    }
}
