﻿using System;
using System.Linq;
using System.Threading;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Enums;
using Assets.Scripts.Helpers;
using SmartLocalization;
using UnityEngine;
using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class SquarePlayground : MonoBehaviour, IPlayground
    {
        public static readonly System.Object _disabledItem = new object();
        protected static readonly System.Random RandomObject = new System.Random();
        protected virtual int _spawnItemTypesInterval { get { return 5; } }

        protected const int AdditionalItemCost = 222;
        protected virtual float HintDelayTime { get { return 4; } }
        protected int MaxAdditionalItemsCount = 2;

        private readonly AutoResetEvent _callbackReady = new AutoResetEvent(false);
        private volatile int _callbacksCount;
        private int _dropsCount;
        private float _timeCounter = -1;

        private GameObject _selectedPoint1;
        private GameObject _selectedPoint2;
        protected Point _selectedPoint1Coordinate;
        protected Point _selectedPoint2Coordinate;
        private int _chainCounter;
        private GameItemType _nextUpperLevelGameItemType = GameItemType.NullItem;
        private float _currentTime;
        private int _minTypePlus;
        private GameObject _leftComboLabel;
        private GameObject _rightComboLabel;

        protected GameItemType lastMoved;
        protected int ToMoveItemsCount;
        protected int XItemsCount;
        protected int Items2XCount;
        protected int GameMovesCount;
        private bool _isGameOver;
        private bool _isMixing;
        protected bool _isDropDone = false;
        protected bool _clearAfterDropDone = false;

        protected GameItemType MaxType = GameItemType._3;
        protected bool CallClearChainsAfterExchange;
        protected bool isExchanging = false;
        protected int CurrentExchangeItemsCount;
        protected float InitialMoveTimerMultiple = 34;
        private readonly Vector3 _selectionScale = new Vector3(1.1f, 1.1f, 1f);
        protected List<GameItem> TempGameItems = new List<GameItem>();
        protected Dictionary<GameItemType, Sprite> GameItemsSprites = new Dictionary<GameItemType, Sprite>();

        #region Scenes Arguments

        public bool IsTimeLabelShow = true;

        public float ScaleMultiplyer = 5.4f;

        public bool AreStaticItemsDroppable = false;

        public bool RaiseMaxInitialElement = true;

        public RealPoint InitialGameItem;

        #endregion


        protected virtual String UserHelpPrefix()
        {
            return "";
        }

        protected virtual Vector3 GameItemScale
        {
            get { return Vector3.one; }
        }

        protected virtual Vector3 SelectionScale
        {
            get { return _selectionScale; }
        }

        public virtual float ItemSpeedMultiplier 
        {
            get { return SelectionScale.x; }
        }

        protected GameItemType MinType
        {
            get { return MaxType - _spawnItemTypesInterval/*FieldSize*/ > GameItemType._1 ? MaxType - _spawnItemTypesInterval/*FieldSize + 1*/ : GameItemType._1; }
        }

        public float MoveTimerMultiple
        {
            get
            {
                return ProgressBar.MoveTimerMultiple;
            }
            protected set
            {
                ProgressBar.MoveTimerMultiple = value;
            }
        }

        public bool IsGameOver
        {
            get { return _isGameOver; }
            set
            {
                if (value == _isGameOver) return;
                _isGameOver = value;
                if (value)
                    SavedataHelper.SaveData(SavedataObject);
            }
        }

        public bool IsMixing
        {
            get { return _isMixing; }

            protected set
            {
                _isMixing = value;
                if (_isMixing)
                    PlaygroundProgressBar.ProgressBarRun = false;
                else
                    PlaygroundProgressBar.ProgressBarRun = true;
            }
        }

        public virtual IGameSettingsHelper Preferenses
        {
            get { return GameSettingsHelper<SquarePlayground>.Preferenses; }
        }

        public int CallbacksCount
        {
            get
            {
                return _callbacksCount;
            }
            protected set
            {
                if (value == _callbacksCount)
                {
                    LogFile.Message("value == _callbacksCount: " + value, true);
                    return;
                }
                _callbacksCount = value;
                LogFile.Message("CallbacksCount = " + value, true);
                if (value > 0)
                {
                    CallbackReady.Reset();
                    //LogFile.Message("_callbackReady.Reset()");
                }
                else
                {
                    CallbackReady.Set();
                    LogFile.Message("_callbackReady.Set()", true);
                }
            }
        }

        public virtual String ItemPrefabName { get { return ItemsNameHelper.GetPrefabPath<SquarePlayground>(); } }

        protected virtual String ItemsTextureName(GameItemType type)
        {
            switch(type)
            {
                case GameItemType._ToMoveItem:
                    if (Game.isExtreme)
                        return ItemsNameHelper.GetPrefabPath<ModeMatch3SquarePlayground>();
                    else
                        return ItemPrefabName;
                default:
                    return ItemPrefabName;
            }
        }

        public virtual string ItemBackgroundTextureName { get { return ItemsNameHelper.GetBackgroundTexturePrefix<SquarePlayground>(); } }

        public virtual bool isBackgroundInSprite { get { return false; } }

        public virtual bool isDisabledItemActive { get { return false; } }

        public System.Object DisabledItem { get { return _disabledItem; } }

        public Point SelectedPoint1
        {
            set
            {
                if (_selectedPoint1 == null && value == null)
                    return;
                if (_selectedPoint1 != null)
                {
                    Destroy(_selectedPoint1);
                    _selectedPoint1 = null;
                }
                if (value != null) LogFile.Message("SelectedPoint1 X = " + value.X + " Y= " + value.Y, true);
                if (/*value == null || */value != null && Items[value.X][value.Y] == null) return;
                _selectedPoint1Coordinate = value;
            }
        }

        public Point SelectedPoint2
        {
            set
            {
                if (_selectedPoint2 == null && value == null)
                    return;
                if (_selectedPoint2 != null)
                {
                    Destroy(_selectedPoint2);
                    _selectedPoint2 = null;
                }
                if (value != null) LogFile.Message("SelectedPoint2 X = " + value.X + " Y= " + value.Y, true);
                if (/*value == null || */value != null && Items[value.X][value.Y] == null) return;
                _selectedPoint2Coordinate = value;
            }
        }

        public GameItemType MaxInitialElementType
        {
            get { return MaxType; }
            set
            {
                if (MaxType == value) return;

                MaxType = value;

                if (Preferenses.CurrentItemType == MaxType && Preferenses.MovesRecord > GameMovesCount)
                    Preferenses.MovesRecord = GameMovesCount;

                if (Preferenses.CurrentItemType < MaxType)
                {
                    Preferenses.CurrentItemType = MaxType;
                    Preferenses.MovesRecord = GameMovesCount;
                }

                RaiseMaxInitialElement = true;
                ShowMaxInitialElement();
            }
        }

        protected virtual String GetTextureIDByType(GameItemType type)
        {
            return GameItem.GetStandartTextureIDByType(type);
        }

        protected virtual GameItemMovingType GetMovingTypeByItemType(GameItemType type)
        {
            switch (type)
            {
                case GameItemType._XItem:
                    return GameItemMovingType.Static;
                case GameItemType._ToMoveItem:
                    if (Game.isExtreme)
                        return GameItemMovingType.StandartExchangable;
                    else
                        return GameItemMovingType.Standart;
                default:
                    return GameItemMovingType.Standart;
            }
        }


        protected virtual void MaxInitialElementTypeRaisedActions()
        {
            switch (MaxType)
            {
                case GameItemType._3:
                case GameItemType._4:
                case GameItemType._5:
                case GameItemType._6:
                    DifficultyRaisedGUI(false, MaxInitialElementTypeRaisedActionsAdditional);
                    _nextUpperLevelGameItemType = GameItemType._7;
                    break;
                case GameItemType._7:
                    Game.Difficulty = DifficultyLevel._medium;
                    _minTypePlus = 1;
                    MoveTimerMultiple = InitialMoveTimerMultiple + 2;
                    DifficultyRaisedGUI(_nextUpperLevelGameItemType != GameItemType.NullItem, MaxInitialElementTypeRaisedActionsAdditional);
                    DeviceButtonsHelpers.OnSoundAction(Power2Sounds.NextLevel, false);
                    _nextUpperLevelGameItemType = GameItemType._10;
                    break;
                case GameItemType._8:
                case GameItemType._9:
                    _minTypePlus = 0;
                    _nextUpperLevelGameItemType = GameItemType._10;
                    MoveTimerMultiple = InitialMoveTimerMultiple + 2;
                    DifficultyRaisedGUI(false, MaxInitialElementTypeRaisedActionsAdditional);
                    break;
                case GameItemType._10:
                    Game.Difficulty = DifficultyLevel._hard;
                    _minTypePlus = 1;
                    MoveTimerMultiple = InitialMoveTimerMultiple + 4;
                    DifficultyRaisedGUI(_nextUpperLevelGameItemType != GameItemType.NullItem, MaxInitialElementTypeRaisedActionsAdditional);
                    if (Items2XCount < 1)
                        SpawnItemOnRandomPlace(GameItemType._2x);
                    DeviceButtonsHelpers.OnSoundAction(Power2Sounds.NextLevel, false);
                    SpawnXItems();
                    _nextUpperLevelGameItemType = GameItemType._13;
                    break;
                case GameItemType._11:
                case GameItemType._12:
                    _minTypePlus = 0;
                    _nextUpperLevelGameItemType = GameItemType._13;
                    MoveTimerMultiple = InitialMoveTimerMultiple + 4;
                    DifficultyRaisedGUI(false, MaxInitialElementTypeRaisedActionsAdditional);
                    break;
                case GameItemType._13:
                    Game.Difficulty = DifficultyLevel._veryhard;
                    _minTypePlus = 1;
                    MoveTimerMultiple = InitialMoveTimerMultiple + 2;
                    DifficultyRaisedGUI(_nextUpperLevelGameItemType != GameItemType.NullItem, MaxInitialElementTypeRaisedActionsAdditional);
                    if (Items2XCount < 2)
                        SpawnItemOnRandomPlace(GameItemType._2x);
                    DeviceButtonsHelpers.OnSoundAction(Power2Sounds.NextLevel, false);
                    _nextUpperLevelGameItemType = GameItemType._2x;
                    break;
                case GameItemType._14:
                case GameItemType._15:
                case GameItemType._16:
                    _minTypePlus = 0;
                    _nextUpperLevelGameItemType = GameItemType._2x;
                    MoveTimerMultiple = InitialMoveTimerMultiple + 2;
                    DifficultyRaisedGUI(false, MaxInitialElementTypeRaisedActionsAdditional);
                    break;
                case GameItemType._2x:
                    IsGameOver = true;
                    GenerateGameOverMenu(true);
                    return;
            }
            if (MaxType - _spawnItemTypesInterval > GameItemType._1)
            {
                DestroyElements(new List<GameItemType> { MinType - 1, MinType - 1 + _minTypePlus });
            }

        }

        protected virtual void MaxInitialElementTypeRaisedActionsAdditional(object o, EventArgs e)
        {
            if (Game.Difficulty == DifficultyLevel._veryhard)
                ProgressBar.TimeBorderActivated += VeryHardLevelAction;
        }

        protected void DifficultyRaisedGUI(bool withLabel = true, EventHandler callback = null)
        {
            var oits = Game.Background.GetComponent<ObjectImageTransparencyScript>();

            oits.SetTransparency(0.1f, (obj, res) =>
            {
                if(isBackgroundInSprite)
                    Game.Background.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>(ItemBackgroundTextureName).SingleOrDefault(t => t.name.Contains(Game.Difficulty.ToString()));
                else
                    Game.Background.GetComponent<Image>().sprite = Resources.Load<Sprite>(ItemBackgroundTextureName + Game.Difficulty.ToString());

                GetComponent<PlaygroundProgressBar>().UpdateTexture();

                var points = GameObject.Find("Points");
                points.GetComponent<Text>().color = GameColors.DifficultyLevelsColors[Game.Difficulty];
                oits.SetTransparency(0.7f, null);


                if (!withLabel)
                {
                    CreateInGameHelpModule(UserHelpPrefix() + Game.Difficulty.ToString(), () =>
                    {
                        if (callback != null)
                            callback(null, EventArgs.Empty);
                        if (IsTimeLabelShow)
                        {
                            IsTimeLabelShow = false;
                            ShowTimeLabel();
                        }
                        else
                            PlaygroundProgressBar.ProgressBarRun = true;

                    });
                    return;
                }

                var o = Instantiate(LabelShowing.LabelPrefab) as GameObject;
                if (o == null) return;
                var difficultyRaisedLabel = o.GetComponent<LabelShowing>();

                difficultyRaisedLabel.ShowScalingLabel(new Vector3(0, -2, -4), LanguageManager.Instance.GetTextValue(Game.Difficulty.ToString()),
                    GameColors.DifficultyLevelsColors[Game.Difficulty], GameColors.DefaultDark, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 2, null, true, () => CreateInGameHelpModule(UserHelpPrefix() + Game.Difficulty.ToString(), () =>
                    {
                        if (callback != null)
                            callback(null, EventArgs.Empty);
                        if (IsTimeLabelShow)
                        {
                            IsTimeLabelShow = false;
                            ShowTimeLabel();
                        }
                        else
                            PlaygroundProgressBar.ProgressBarRun = true;

                    }), true);
            });


        }

        protected virtual void VeryHardLevelAction(object sender, EventArgs e)
        {
            var actionIndex = RandomObject.Next(0, 2);
            switch (actionIndex)
            {
                case 0:
                    int col = RandomObject.Next(0, FieldSize);
                    for (int row = 0; row < FieldSize; row++)
                        if (Items[col][row] != DisabledItem)
                        {
                            var gobj = Items[col][row] as GameObject;
                            if (gobj == null) continue;
                            var gi = gobj.GetComponent<GameItem>();
                            if (gi.MovingType != GameItemMovingType.Static)
                                RemoveGameItem(col, row);
                        }
                    return;
                case 1:
                    int row1 = RandomObject.Next(0, FieldSize);
                    for (int col1 = 0; col1 < FieldSize; col1++)
                        if (Items[col1][row1] != DisabledItem)
                        {
                            var gobj = Items[col1][row1] as GameObject;
                            if (gobj == null) continue;
                            var gi = gobj.GetComponent<GameItem>();
                            if (gi.MovingType != GameItemMovingType.Static)
                                RemoveGameItem(col1, row1);
                        }
                    return;
            }
        }

        protected void ShowTimeLabel()
        {
            var o = Instantiate(LabelShowing.LabelPrefab) as GameObject;
            if (o == null) return;
            var showTimeLabel = o.GetComponent<LabelShowing>();
            var fg = GameObject.Find("/Foreground");
            showTimeLabel.transform.SetParent(fg.transform);
            showTimeLabel.ShowScalingLabel(new Vector3(ProgressBar.Coordinate.x, ProgressBar.Coordinate.y, -4), LanguageManager.Instance.GetTextValue("TimeStart"),
                GameColors.BackgroundColor, GameColors.BackgroundColor, LabelShowing.minLabelFontSize - 10, LabelShowing.maxLabelFontSize - 30, 1, null, true, () =>
                {
                    PlaygroundProgressBar.ProgressBarRun = true;
                });
        }

        protected virtual void SpawnXItems()
        {
            while (XItemsCount < MaxAdditionalItemsCount)
            {
                SpawnItemOnRandomPlace(GameItemType._XItem, XItemsCount % 2 == 0 ? MoveDirections.Left : MoveDirections.Right, null);
                XItemsCount++;
            }
        }

        protected void SpawnItemOnRandomPlace(GameItemType type, MoveDirections? area = null, Vector3? scaleTo = null)
        {
            int col;
            int row;
            int fromCol = 1;
            int toCol = FieldSize - 1;
            int fromRow = fromCol;
            int toRow = toCol;
            if(area != null)
            switch (area.Value)
            {
                case MoveDirections.Left:
                    fromCol = 1;
                    toCol = FieldSize/2;
                    fromRow = 1;
                    toRow = FieldSize - 1;
                    break;
                case MoveDirections.Right:
                    fromCol = FieldSize / 2 + FieldSize%2;
                    toCol = FieldSize - 1;
                    fromRow = 1;
                    toRow = FieldSize - 1;
                    break;
            }
            while (true)
            {
                col = RandomObject.Next(fromCol, toCol);
                row = RandomObject.Next(fromRow, toRow);
                if (Items[col][row] == null || Items[col][row] == DisabledItem) continue;
                var o = Items[col][row] as GameObject;
                if (o == null) continue;
                var gi = o.GetComponent<GameItem>();
                if (Items[col][row] != null && Items[col][row] != DisabledItem &&
                    gi.Type < GameItemType._2x && gi.Type != (MaxType + 1) &&
                    !o.GetComponent<GameItemScalingScript>().isScaling)
                    break;
            }
            RemoveGameItem(col, row, (item, r) =>
            {
                Items[col][row] = GenerateGameItem(type, col, row, Vector2.zero, scaleTo == null ? GameItemScale : scaleTo, false, null, null);
            });
        }

        public void ShowMaxInitialElement()
        {
            var currentMIE = GameObject.Find("/Foreground/MaximumItem");
            if (currentMIE != null)
            {
                var miess = currentMIE.GetComponent<GameItemScalingScript>();
                miess.ScaleTo(new Vector3(0.5f, 0.5f, 0), 8, (item, r) =>
                {
                    currentMIE.transform.localPosition = new Vector3(0, 400f, -1);
                    currentMIE.transform.localScale = Vector3.one;
                    if (!GameItemsSprites.Keys.Contains(MaxType))
                        GameItemsSprites.Add(MaxType, Resources.LoadAll<Sprite>(ItemPrefabName.Split('/')[1] + "Tiles").FirstOrDefault(t => t.name.Contains(GetTextureIDByType(MaxType))));
                    currentMIE.GetComponent<SpriteRenderer>().sprite = GameItemsSprites[MaxType];
                    var c = currentMIE.GetComponent<GameItemMovingScript>();
                    c.MoveTo(null, 320f, Game.StandartItemSpeed / 4);
                });
            }
            else
            {
                var fg = GameObject.Find("/Foreground");
                var gobj = Instantiate(Resources.Load(ItemPrefabName)) as GameObject;
                if (gobj == null) return;
                gobj.transform.SetParent(fg.transform);
                gobj.transform.localPosition = new Vector3(0, 400f, -1);
                gobj.transform.localScale = Vector3.one;//new Vector3(16, 16);
                if (!GameItemsSprites.Keys.Contains(MaxType))
                    GameItemsSprites.Add(MaxType, Resources.LoadAll<Sprite>(ItemPrefabName.Split('/')[1] + "Tiles").FirstOrDefault(t => t.name.Contains(GetTextureIDByType(MaxType))));
                gobj.GetComponent<SpriteRenderer>().sprite = GameItemsSprites[MaxType];
                gobj.name = "MaximumItem";
                gobj.AddComponent<Button>();
                var image = gobj.AddComponent<Image>();
                image.color = new Color(0, 0, 0, 0);
                var buttonComponent = gobj.GetComponent<Button>();
                if (buttonComponent != null)
                    buttonComponent.onClick.AddListener(ThemeChooserScript.OnScriptGameThemeChange);
                var c = gobj.GetComponent<GameItemMovingScript>();
                LogFile.Message("GameItem generated to X:" + gobj.transform.localPosition.x + " Y:" + (gobj.transform.localPosition.y), true);
                c.MoveTo(null, 320f, Game.StandartItemSpeed / 4);
            }
        }

        public void DestroyElements(List<GameItemType> withTypes)
        {
            foreach (var t in withTypes)
                LogFile.Message("Destroy elements above " + t);
            var pointsBank = 0;

            for (var i = FieldSize - 1; i >= 0; i--)
            {
                for (var j = 0; j < FieldSize; j++)
                    if (Items[i][j] != null && Items[i][j] != DisabledItem)
                    {
                        var gobj = Items[i][j] as GameObject;
                        if (gobj == null) continue;
                        var item = gobj.GetComponent<GameItem>();
                        if (item.Type == GameItemType.DisabledItem || item.Type == GameItemType.NullItem || !withTypes.Contains(item.Type)) continue;
                        LogFile.Message("Item destroied: " + item.Type, true);
                        pointsBank += (int)Math.Pow(2, (double)item.Type);
                        RemoveGameItem(i, j);
                    }
            }

            RaisePoints(pointsBank * (int)Game.Difficulty);
            if (ProgressBar != null)
                ProgressBar.AddTime(pointsBank * 2);
        }

        public virtual IPlaygroundSavedata SavedataObject
        {
            get { return null; }
        }

        public virtual int FieldSize { get { return 0; } }

        public float DeltaToExchange { get { return GameItemSize / 2f; } }

        public float DeltaToMove { get { return GameItemSize / 6f; } }

        public object[][] Items { get; set; }

        public virtual float GameItemSize { get { return 0f; } }

        public virtual RealPoint InitialGameItemPosition { get { return null; } }

        public Dictionary<MoveDirections, Vector2> AvailableMoveDirections { get; protected set; }

        public virtual int CurrentScore
        {
            get { return GetComponent<PointsUpdater>().CurrentScore; }
            protected set
            {
                var c = GetComponent<PointsUpdater>();
                c.RisePoints(value);

                if (Preferenses.ScoreRecord < c.CurrentScore)
                    Preferenses.ScoreRecord = c.CurrentScore;
            }
        }

        public void UpdateTime()
        {
            if (Preferenses.LongestSession < CurrentTime + Time.timeSinceLevelLoad)
                Preferenses.LongestSession = CurrentTime + Time.timeSinceLevelLoad;
        }

        public float CurrentTime
        {
            get { return _currentTime; }
            set
            {
                if (_currentTime != value)
                    _currentTime = value;
            }
        }

        public int DropsCount
        {
            get
            {
                return _dropsCount;
            }
            set
            {
                if (value == _dropsCount)
                {
                    LogFile.Message("value == _dropsCount: " + value, true);
                    return;
                }
                _dropsCount = value;
                LogFile.Message("DropsCount = " + value, true);
                //if (value > 0)
                //{
                //    DropsReady.Reset();
                //    //LogFile.Message("_callbackReady.Reset()");
                //}
                //else
                //{
                //    DropsReady.Set();
                //    //LogFile.Message("DropsReady.Set()");
                //}
            }
        }

        public int ChainCounter
        {
            get { return _chainCounter; }
            set
            {
                if (_chainCounter == value) return;
                _chainCounter = value;
                //var stat = GetComponent<Game>().Stats;
                //if (stat != null && stat.MaxMultiplier < _chainCounter)
                if (Preferenses.MaxMultiplier < _chainCounter)
                    Preferenses.MaxMultiplier = _chainCounter;
            }
        }

        public float HintTimeCounter
        {
            get { return _timeCounter; }
            set { _timeCounter = value; }
        }

        protected AutoResetEvent CallbackReady
        {
            get { return _callbackReady; }
        }

        protected Point SelectedPoint1Coordinate
        {
            get { return _selectedPoint1Coordinate; }
        }

        protected Point SelectedPoint2Coordinate
        {
            get { return _selectedPoint2Coordinate; }
        }

        public PlaygroundProgressBar ProgressBar
        {
            get
            {
                PlaygroundProgressBar pb = null;
                try
                {
                    pb = GetComponent<PlaygroundProgressBar>();
                }
                catch (Exception ex)
                {
                    LogFile.Message(ex.Message + " " + ex.StackTrace);
                }
                return pb;
            }
        }

        public SquarePlayground(Dictionary<MoveDirections, Vector2> dictionary)
        {
            AvailableMoveDirections = dictionary;
        }

        public SquarePlayground()
            : this(new Dictionary<MoveDirections, Vector2>
            {
                {MoveDirections.Up, new Vector2(0, 1)},
                {MoveDirections.Down, new Vector2(0, -1)},
                {MoveDirections.Left, new Vector2(-1, 0)},
                {MoveDirections.Right, new Vector2(1, 0)},
                {MoveDirections.UL, new Vector2(-1, 1)},
                {MoveDirections.UR, new Vector2(1, 1)},
                {MoveDirections.DL, new Vector2(-1, -1)},
                {MoveDirections.DR, new Vector2(1, -1)},
            }) { }

        public void Start()
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().backgroundColor =
                                GameColors.BackgroundColor;
        }

        protected virtual bool RemoveAdditionalItems()
        {
            if (Game.isExtreme) return false;
            bool result = false;
            for (var i = 0; i < FieldSize; i++)
                if (Items[i][FieldSize - 1] != null && Items[i][FieldSize - 1] != DisabledItem)
                {
                    var gobj = Items[i][FieldSize - 1] as GameObject;
                    if (gobj == null ||
                        (gobj.GetComponent<GameItem>().Type != GameItemType._ToMoveItem ||
                         gobj.GetComponent<GameItemMovingScript>().IsMoving)) continue;
                    /* var o = Instantiate(LabelShowing.LabelPrefab) as GameObject;
                     if (o != null)
                     {
                         var pointsLabel = o.GetComponent<LabelShowing>();
                         pointsLabel.transform.SetParent(transform);*/
                    LabelShowing.ShowScalingLabel(gobj, false, (i >= FieldSize/2)/*new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y + GameItemSize / 2, gobj.transform.localPosition.z - 1)*/,
                        "+" + 222, Color.white, Color.white, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 2, Game.numbersFont, true, null, 0, GameItemType._ToMoveItem);
                    //}
                    RaisePoints(AdditionalItemCost);
                    if (ProgressBar != null)
                        ProgressBar.AddTime(AdditionalItemCost * 2);

                    //Destroy(gobj);
                    RemoveGameItem(i, FieldSize - 1); /*(s, e) =>
                        {
                            ToMoveItemsCount--;
                        });*/
                    result = true;
                }
            return result;
        }

        protected virtual void Update()
        {
            if (IsGameOver || PauseButtonScript.PauseMenuActive) return;

            if (!_isDropDone && CallbacksCount == 0)
                Drop();
            if (Game.isExtreme || !(HintTimeCounter >= 0)) return;
            if (HintTimeCounter > HintDelayTime && _selectedPoint1 == null && _selectedPoint2 == null)
            {
                if (SelectedPoint1Coordinate == null || SelectedPoint2Coordinate == null) return;
                var parentGobj = Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] as GameObject;
                if (parentGobj == null || Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] == DisabledItem || parentGobj.GetComponent<GameItem>().MovingType == GameItemMovingType.Static) return;
                var parentGobj2 = Items[SelectedPoint2Coordinate.X][SelectedPoint2Coordinate.Y] as GameObject;
                if (parentGobj2 == null || Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] == DisabledItem || parentGobj2.GetComponent<GameItem>().MovingType == GameItemMovingType.Static) return;
                _selectedPoint1 = Instantiate(Resources.Load(ItemPrefabName + "_SelectedItem")) as GameObject;
                if (_selectedPoint1 == null)
                {
                    LogFile.Message("SelectedPoint1 initialization failed", true);
                    return;
                }
                _selectedPoint1.transform.SetParent(parentGobj.transform);
                _selectedPoint1.transform.localScale = SelectionScale;
                _selectedPoint1.transform.localPosition = new Vector3(0, -0.03f, -1);

                _selectedPoint2 = Instantiate(Resources.Load(ItemPrefabName + "_SelectedItem")) as GameObject;
                if (_selectedPoint2 == null)
                {
                    LogFile.Message("SelectedPoint2 initialization failed", true);
                    return;
                }
                _selectedPoint2.transform.SetParent(parentGobj2.transform);
                _selectedPoint2.transform.localScale = SelectionScale;
                _selectedPoint2.transform.localPosition = new Vector3(0, -0.03f, -1);
            }
            HintTimeCounter += Time.deltaTime;
        }

        public GameObject InstantiateGameItem(GameItemType itemType, Vector3 localPosition, Vector3 localScale, GameItemMovingType? movingType = null)
        {
            GameItem newgi = TempGameItems.Where(ti=>ti.Type == GameItemType.NullItem).FirstOrDefault();
            if (newgi == null)
            {
                var newgobj = Instantiate(Resources.Load(ItemPrefabName)) as GameObject;
                if (newgobj == null) return null;
                var rectTransform = newgobj.transform as RectTransform;
                if (rectTransform != null)
                    rectTransform.sizeDelta = new Vector2(GameItemSize * 0.9375f, GameItemSize * 0.9375f);
                newgobj.transform.SetParent(transform);
                newgi = newgobj.GetComponent<GameItem>();
                /*newgobj.transform.localPosition = localPosition;
                newgobj.transform.localScale = localScale;
                newgi = newgobj.GetComponent<GameItem>();
                newgi.Type = itemType;
                if (movingType.HasValue)
                    newgi.MovingType = movingType.Value;*/
                TempGameItems.Add(newgi);
            }
            newgi.transform.localPosition = localPosition;
            newgi.transform.localScale = localScale;
            if(!GameItemsSprites.Keys.Contains(itemType))
            GameItemsSprites.Add(itemType,Resources.LoadAll<Sprite>(ItemsTextureName(itemType).Split('/')[1] + "Tiles").FirstOrDefault(t => t.name.Contains(GetTextureIDByType(itemType))));
            newgi.GetComponent<SpriteRenderer>().sprite = GameItemsSprites[itemType];
            newgi.Type = itemType;
            newgi.MovingType = movingType != null? movingType.Value  : GetMovingTypeByItemType(itemType);
            return newgi.gameObject;
        }

        protected void DestroyGameItem(GameObject gobj)
        {
            if (gobj == null) return;
            var gi = gobj.GetComponent<GameItem>();
            if(gi == null)
            {
                Destroy(gobj);
                return;
            }
            gi.Reset();
            gi.transform.localPosition = new Vector3(Screen.width, Screen.height, gi.transform.localPosition.z);
        }

        public virtual GameObject GenerateGameItem(GameItemType itemType, int i, int j, Vector2? generateOn = null, Vector3? scaleTo = null, bool isItemDirectionChangable = false, float? dropSpeed = null, MovingFinishedDelegate movingCallback = null, GameItemMovingType? movingType = null)
        {
            if (!generateOn.HasValue)
                generateOn = new Vector2(0, FieldSize - j);
            if (InitialGameItem == null)
                InitialGameItem = InitialGameItemPosition;
            var cell = GetCellCoordinates(i, j);
            var gobj = InstantiateGameItem(itemType, new Vector3(
                (float)Math.Round(cell.x + generateOn.Value.x * GameItemSize, 2),
                    InitialGameItem.Y + generateOn.Value.y * GameItemSize,
                InitialGameItem.Z), Vector3.zero, movingType);

            var c = gobj.GetComponent<GameItemMovingScript>();
            LogFile.Message("GameItem generated to X:" + gobj.transform.localPosition.x + " Y:" + (gobj.transform.localPosition.y - 6 * GameItemSize), true);
            CallbacksCount++;
            //var toS = GameItemSize / ScaleMultiplyer;
            c.MoveTo(cell.x, cell.y, dropSpeed.HasValue ? dropSpeed.Value : 10 - i % 2 + j * 1.5f, (item, result) =>
            {
                CallbacksCount--;
                if (movingCallback != null)
                    movingCallback(item, result);
                else
                {
                    //if (!result) return;
                    if (CallbacksCount == 0)
                        ClearChains();
                    else
                        if (isExchanging && CallbacksCount == CurrentExchangeItemsCount)
                            CallClearChainsAfterExchange = true;
                }
            }, new Vector2(InitialGameItem.X, InitialGameItem.Y + GameItemSize / 2), scaleTo != null ? scaleTo : Vector3.one/*new Vector3(toS, toS, 1f)*/, isItemDirectionChangable,
            null, Power2Sounds.Drop);

            return gobj;
        }

        public virtual GameObject GenerateGameItem(int i, int j, IList<GameItemType> deniedTypes = null, Vector2? generateOn = null, bool isItemDirectionChangable = false, float? dropSpeed = null,
            MovingFinishedDelegate movingCallback = null, GameItemMovingType? movingType = null)
        {
            int newType;
            var possibility = RandomObject.Next(1, 101);
            var minItem = (int)MinType + _minTypePlus;//(int)MaxType > FieldSize ? (int)MinType + 1 + _minTypePlus : (int)GameItemType._1;
            var isEven = (i + j) % 2;
            if (possibility <= 50)
                possibility = isEven == 0 ? 50 : 20;
            else
                if (possibility <= 70)
                    possibility = isEven == 0 ? 20 : 50;
            switch (possibility)
            {
                case 50:
                    newType = minItem;
                    break;
                case 20:
                    newType = RandomObject.Next(minItem + 2, (int)MaxInitialElementType + 1);
                    break;
                default:
                    newType = minItem + 1;
                    break;
            }
            if (deniedTypes == null || deniedTypes.Count == 0)
                return GenerateGameItem((GameItemType)newType, i, j, generateOn, GameItemScale, isItemDirectionChangable, dropSpeed, movingCallback, movingType);
            while (deniedTypes.Contains((GameItemType)newType))
                newType = RandomObject.Next((int)GameItemType._1, (int)MaxInitialElementType + 1);
            return GenerateGameItem((GameItemType)newType, i, j, generateOn, GameItemScale, isItemDirectionChangable, dropSpeed, movingCallback);
        }

        public virtual IEnumerable<Line> LinesWithItem(IEnumerable<Line> lines, int currentX, int currentY)
        {
            return lines.Where(l => (l.X1 <= currentX && l.X2 >= currentX && l.Y1 <= currentY && l.Y2 >= currentY));
        }

        public bool MatchType(int x, int y, GameItemType itemType)
        {
            // убедимся, что фишка не выходит за пределы поля    
            if ((x < 0) || (x >= FieldSize) || (y < 0) || (y >= FieldSize) || Items[x][y] == null || Items[x][y] == DisabledItem || itemType > GameItemType._2x) return false;
            var gobj = Items[x][y] as GameObject;
            if (gobj == null) return false;
            var gi = gobj.GetComponent<GameItem>();
            return gi != null && gi.Type == itemType && gi.MovingType != GameItemMovingType.Static;
        }

        public virtual Vector3 GetCellCoordinates(int col, int row)
        {
            var roundedX = Math.Round(InitialGameItem.X + Convert.ToSingle(col) * GameItemSize, 2);
            var roundedY = Math.Round(InitialGameItem.Y - GameItemSize * Convert.ToSingle(row), 2);
            return new Vector3((float)roundedX, (float)roundedY, InitialGameItem.Z);
        }

        public virtual bool MatchPattern(GameItemType itemType, Point firstItem, Point secondItem, IEnumerable<Point> pointForCheck)
        {
            var mainType = MatchType(firstItem.X + secondItem.X, firstItem.Y + secondItem.Y, itemType);
            if (!mainType) return false;

            var lineGenerationPoints = pointForCheck.Where(point => MatchType(firstItem.X + point.X, firstItem.Y + point.Y, itemType)).ToList();
            if (!lineGenerationPoints.Any()) return false;

            foreach (var lineGenerationPoint in lineGenerationPoints)
            {
                if (secondItem.X == 0)
                {
                    LogFile.Message("secondItem.X == 0 and firstItem.X: " + firstItem.X + " firstItem.Y: " + firstItem.Y +
                                    " secondItem.Y: " + secondItem.Y + "lineGenerationPoint.Y: " + lineGenerationPoint.Y, true);
                    if (Math.Abs(secondItem.Y) > 1)
                        SelectedPoint1 = new Point
                        {
                            X = firstItem.X,
                            Y = firstItem.Y + (secondItem.Y > 0 ? 1 : -1)
                        };
                    else
                    {
                        SelectedPoint1 = new Point
                        {
                            X = firstItem.X,
                            Y = firstItem.Y + (lineGenerationPoint.Y > 0 ? 2 : -1)
                        };
                    }
                }
                else if (secondItem.Y == 0)
                {
                    LogFile.Message("secondItem.Y == 0 and firstItem.X: " + firstItem.X + " firstItem.Y: " + firstItem.Y +
                                    " secondItem.Y: " + secondItem.Y + "lineGenerationPoint.Y: " + lineGenerationPoint.Y, true);
                    if (Math.Abs(secondItem.X) > 1)
                        SelectedPoint1 = new Point
                        {
                            X = firstItem.X + (secondItem.X > 0 ? 1 : -1),
                            Y = firstItem.Y
                        };
                    else
                    {
                        SelectedPoint1 = new Point
                        {
                            X = firstItem.X + (lineGenerationPoint.X > 0 ? 2 : -1),
                            Y = firstItem.Y
                        };
                    }
                }
                var diffX = Math.Abs(SelectedPoint1Coordinate.X - (firstItem.X + lineGenerationPoint.X));
                var diffY = Math.Abs(SelectedPoint1Coordinate.Y - (firstItem.Y + lineGenerationPoint.Y));
                if (!((diffX == 0 && diffY == 1) || (diffX == 1 && diffY == 0)))
                    continue;
                if (Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] != null && Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] != DisabledItem)
                {
                    var selectedObject = Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] as GameObject;
                    if (selectedObject != null)
                    {
                        var gi = selectedObject.GetComponent<GameItem>();
                        if (gi.MovingType == GameItemMovingType.Static || gi.Type == GameItemType.NullItem)
                            continue;
                    }
                }
                else continue;
                SelectedPoint2 = new Point { X = firstItem.X + lineGenerationPoint.X, Y = firstItem.Y + lineGenerationPoint.Y };
                return true;
            }
            return false;
        }

        public virtual IList<Line> GetAllLines()
        {
            var list = new List<Line>();
            for (var row = 0; row < FieldSize; row++)
                for (var col = 0; col < FieldSize - 2; col++)
                {
                    var o = Items[col][row] as GameObject;
                    if (o == null ||
                        (Items[col][row] == null || Items[col][row] == DisabledItem ||
                         (o.GetComponent<GameItemMovingScript>().IsMoving &&
                          !o.GetComponent<GameItem>().IsDraggableWhileMoving))) continue;
                    var match = CheckForLine(col, row, LineOrientation.Horizontal);
                    if (match <= 2) continue;
                    list.Add(new Line { X1 = col, Y1 = row, X2 = col + match - 1, Y2 = row, Orientation = LineOrientation.Horizontal });
                    col += match - 1;
                }
            for (var col = 0; col < FieldSize; col++)
                for (var row = 0; row < FieldSize - 2; row++)
                {
                    var o = Items[col][row] as GameObject;
                    if (o != null && (Items[col][row] != null && Items[col][row] != DisabledItem && (!o.GetComponent<GameItemMovingScript>().IsMoving
                                                                                                     || o.GetComponent<GameItem>().IsDraggableWhileMoving)))
                    {
                        var match = CheckForLine(col, row, LineOrientation.Vertical);
                        if (match <= 2) continue;
                        list.Add(new Line { X1 = col, Y1 = row, X2 = col, Y2 = row + match - 1, Orientation = LineOrientation.Vertical });
                        row += match - 1;
                    }
                }
            if (list.Count > 0)
                LogFile.Message("Find " + list.Count + " lines", true);
            return list;
        }

        public virtual int CheckForLine(int x, int y, LineOrientation orientation, bool includeMovingItemsInLine = true)
        {
            var count = 1;
            if (Items[x][y] == null || Items[x][y] == DisabledItem) return count;
            switch (orientation)
            {
                case LineOrientation.Horizontal:
                    for (var i = 1; x + i < FieldSize; i++)
                    {
                        if (Items[x + i][y] == null || Items[x + i][y] == DisabledItem) break;
                        if (includeMovingItemsInLine)
                        {
                            var goItem = (Items[x + i][y] as GameObject);
                            if (goItem != null && ((goItem.GetComponent<GameItemMovingScript>().IsMoving && !goItem.GetComponent<GameItem>().IsDraggableWhileMoving)
                                || goItem.GetComponent<GameItemScalingScript>().isScaling)) return 1;
                        }
                        var gobj1 = Items[x][y] as GameObject;
                        if (gobj1 != null)
                        {
                            var gi1 = gobj1.GetComponent<GameItem>();
                            var gobj2 = Items[x + i][y] as GameObject;
                            if (gobj2 != null)
                            {
                                var gi2 = gobj2.GetComponent<GameItem>();
                                if (gi1.Type != gi2.Type)
                                    break;
                            }
                        }
                        count++;
                    }
                    break;
                case LineOrientation.Vertical:
                    for (var i = 1; y + i < FieldSize; i++)
                    {

                        if (Items[x][y + i] == null || Items[x][y + i] == DisabledItem) break;
                        if (includeMovingItemsInLine)
                        {
                            var goItem = (Items[x][y + i] as GameObject);
                            if (goItem != null && ((goItem.GetComponent<GameItemMovingScript>().IsMoving && !goItem.GetComponent<GameItem>().IsDraggableWhileMoving)
                                || goItem.GetComponent<GameItemScalingScript>().isScaling)) return 1;
                        }
                        var gobj1 = Items[x][y] as GameObject;
                        if (gobj1 != null)
                        {
                            var gi1 = gobj1.GetComponent<GameItem>();
                            var gobj2 = Items[x][y + i] as GameObject;
                            if (gobj2 != null)
                            {
                                var gi2 = gobj2.GetComponent<GameItem>();
                                if (gi1.Type != gi2.Type)
                                    break;
                            }
                        }
                        count++;
                    }
                    break;
            }
            return count;
        }

        public virtual int ClearChains()
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

                if (!RemoveAdditionalItems() && CallbacksCount == 0 && !CheckForPossibleMoves())
                {
                    LogFile.Message("No moves", true);
                    if (Game.isExtreme)
                    {
                        if (lastMoved != GameItemType._ToMoveItem)
                        {
                            GenerateField(false, true, Game.Difficulty != DifficultyLevel._easy, () => CreateInGameHelpModule("Match3NoMoves"));
                            if (Game.Difficulty > DifficultyLevel._easy)
                            {
                                lastMoved = GameItemType._ToMoveItem;
                                _selectedPoint1Coordinate = null;
                                _selectedPoint2Coordinate = null;
                            }
                        }
                    }
                    else
                    {
                        GenerateField(false, true);
                    }
                }
                UpdateTime();
                return 0;
            }
            if (Game.isExtreme && lastMoved == GameItemType._ToMoveItem)
                lastMoved = GameItemType.NullItem;
            HintTimeCounter = -1;
            _isDropDone = false;
            LogFile.Message("Start clear chaines. Lines: " + lines.Count, true);
            var linesCount = lines.Count;
            var pointsBank = 0;
            var l = lines.FirstOrDefault();
            int repeatForLine = -1;
            bool raiseMaxIEtype = true;
            int toObjX = 0, toObjY = 0;
            Vector3 toCell = Vector3.zero;
            while (l != null && !IsGameOver)
            {
                var pointsMultiple = 1;
                if (l.Orientation == LineOrientation.Vertical)
                {
                    pointsMultiple += l.Y2 - l.Y1 - 2;
                    if (repeatForLine == -1)
                    {
                        toObjX = l.X2;
                        toObjY = l.Y2 - (l.Y2 - l.Y1)/2;
                        for (var j = l.Y2; j >= l.Y1; j--)
                        {
                            var lwi = LinesWithItem(lines, l.X1, j);
                            var enumerable = lwi as IList<Line> ?? lwi.ToList();
                            if (enumerable.Count() <= 1) continue;

                            toObjX = l.X1;
                            toObjY = j;
                            repeatForLine = lines.IndexOf(enumerable.FirstOrDefault(line => line != l));
                        }
                        toCell = GetCellCoordinates(toObjX, toObjY);
                    }
                    else
                    {
                        raiseMaxIEtype = false;
                        repeatForLine = -1;
                    }
                    for (var j = l.Y2; j >= l.Y1; j--)
                    {
                        if (Items[l.X1][j] == null || Items[l.X1][j] == DisabledItem || j == toObjY)
                            continue;

                        var gobj = Items[l.X1][j] as GameObject;
                        if (gobj == null) continue;

                        gobj.transform.localPosition = new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y, -0.5f);
                        var c = gobj.GetComponent<GameItemMovingScript>();

                        var cX = l.X1;
                        var cY = j;
                        if (c.GetComponent<GameItem>().IsTouched)
                        {
                            var cell = toCell;
                            GetComponent<DragItemScript>().CancelDragging((s, e) =>
                            {
                                CallbacksCount++;
                                Items[cX][cY] = null;
                                c.MoveTo(null, cell.y, Game.StandartItemSpeed, (item, result) =>
                                {
                                    LogFile.Message(cX + " " + cY, true);
                                    CallbacksCount--;
                                    if (!result) return;
                                    DestroyGameItem((GameObject)item);
                                });
                            });
                        }
                        else
                        {
                            CallbacksCount++;
                            Items[cX][cY] = null;
                            c.MoveTo(null, toCell.y, Game.StandartItemSpeed, (item, result) =>
                            {
                                LogFile.Message(cX + " " + cY, true);
                                CallbacksCount--;
                                if (!result) return;
                                DestroyGameItem((GameObject)item);
                            });
                        }


                    }
                }
                else
                {
                    pointsMultiple += l.X2 - l.X1 - 2;
                    if (repeatForLine == -1)
                    {
                        toObjX = l.X2 - (l.X2 - l.X1) / 2;
                        toObjY = l.Y2;
                        for (var i = l.X2; i >= l.X1; i--)
                        {
                            var lwi = LinesWithItem(lines, i, l.Y1);
                            var enumerable = lwi as IList<Line> ?? lwi.ToList();
                            if (enumerable.Count() <= 1) continue;
                            toObjX = i;
                            toObjY = l.Y1;
                            repeatForLine = lines.IndexOf(enumerable.FirstOrDefault(line => line != l));
                        }
                        toCell = GetCellCoordinates(toObjX, toObjY);
                    }
                    else
                    {
                        raiseMaxIEtype = false;
                        repeatForLine = -1;
                    }
                    for (var i = l.X2; i >= l.X1; i--)
                    {
                        if (Items[i][l.Y1] == null || Items[i][l.Y1] == DisabledItem || i == toObjX)
                        {
                            continue;
                        }

                        var gobj = Items[i][l.Y1] as GameObject;
                        if (gobj == null) continue;

                        gobj.transform.localPosition = new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y, -0.5f);
                        var c = gobj.GetComponent<GameItemMovingScript>();

                        var cX = i;
                        var cY = l.Y1;

                        if (c.GetComponent<GameItem>().IsTouched)
                        {
                            var cell = toCell;
                            GetComponent<DragItemScript>().CancelDragging((s, e) =>
                            {
                                CallbacksCount++;
                                Items[cX][cY] = null;
                                c.MoveTo(cell.x, null, Game.StandartItemSpeed, (item, result) =>
                                {
                                    LogFile.Message(cX + " " + cY, true);
                                    CallbacksCount--;
                                    if (!result) return;
                                    DestroyGameItem((GameObject)item);
                                });
                            });
                        }
                        else
                        {
                            CallbacksCount++;
                            Items[cX][cY] = null;
                            c.MoveTo(toCell.x, null, Game.StandartItemSpeed, (item, result) =>
                            {
                                LogFile.Message(cX + " " + cY, true);
                                CallbacksCount--;
                                if (!result) return;
                                DestroyGameItem((GameObject)item);
                            });
                        }
                    }
                }
                var toGobj = Items[toObjX][toObjY] as GameObject;
                if (toGobj != null)
                {
                    var toGi = toGobj.GetComponent<GameItem>();
                    if (raiseMaxIEtype && toGi.Type > MaxInitialElementType)
                        MaxInitialElementType = toGi.Type;
                    var newgobjtype = toGi.Type != GameItemType._2x ? toGi.Type + 1 : GameItemType._2x;
                    var newgobj = InstantiateGameItem(newgobjtype, toCell, GameItemScale);
                                //new Vector3(GameItemSize / ScaleMultiplyer, GameItemSize / ScaleMultiplyer, 0f));
                    if (toGobj.GetComponent<GameItemMovingScript>().IsMoving)
                        CallbacksCount--;
                    DestroyGameItem(toGobj);
                    Items[toObjX][toObjY] = newgobj;
                    var points = pointsMultiple * (int)Math.Pow(2, (double)newgobjtype);
                    if (newgobjtype <= MaxInitialElementType)
                    {
                        pointsBank += points;
                        LabelShowing.ShowScalingLabel(newgobj, (l.Orientation == LineOrientation.Horizontal), (toObjX >= FieldSize/2),
                            "+" + points, GameColors.ItemsColors[newgobjtype], GameColors.DefaultDark, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 2, Game.numbersFont, true, null, 0, newgobjtype);
                    }
                    else
                    {
                        pointsBank += 2 * points;
                        LabelShowing.ShowScalingLabel(newgobj, (l.Orientation == LineOrientation.Horizontal), (toObjX >= FieldSize / 2),
                            "+" + points + "x2", GameColors.ItemsColors[newgobjtype], GameColors.DefaultDark, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 2, Game.numbersFont, true, null, 0, newgobjtype);
                    }
                }
                lines.Remove(l);
                if (linesCount == 1)
                    DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Line, false);

                LogFile.Message("line collected", true);
                if (repeatForLine == -1)
                    l = lines.FirstOrDefault();
                else
                    l = lines[repeatForLine - 1];

                if (ProgressBar != null)
                    ProgressBar.AddTime(pointsMultiple * 2);

                if (l != null) continue;

                if (linesCount > 1)
                    ShowComboLabel(linesCount);

                pointsBank *= linesCount;
                ChainCounter++;
                RaisePoints(pointsBank * ChainCounter);

                pointsBank = 0;
                lines = GetAllLines();
                linesCount = lines.Count;
                l = lines.FirstOrDefault();
            }
            LogFile.Message("All lines collected", true);
            //RemoveAdditionalItems();

            return linesCount;
        }

        public void GenerateGameOverMenu(bool isWinning = false)
        {

            SavedataHelper.RemoveData(SavedataObject);
            var gameOverMenu = Instantiate(Resources.Load("Prefabs/GameOverMenu")) as GameObject;
            if (gameOverMenu == null) return;
            //var pausebackground = Instantiate(Resources.Load("Prefabs/PauseBackground")) as GameObject;
            var gameOverLabelObject = Instantiate(LabelShowing.LabelPrefab) as GameObject;
            if (/*pausebackground == null || */gameOverLabelObject == null) return;
            PauseButtonScript.PauseMenuActive = true;
            Time.timeScale = 0F;
            if (Game.Foreground != null)
            {
                //pausebackground.transform.SetParent(fg.transform);
                gameOverMenu.transform.SetParent(Game.Foreground.transform);
                gameOverLabelObject.transform.SetParent(Game.Foreground.transform);
            }

            /*pausebackground.transform.localPosition = Vector3.zero;
            var rectTransform = pausebackground.transform as RectTransform;
            if (rectTransform != null)
                rectTransform.sizeDelta = Vector2.zero;*/

            var gameOverLabel = gameOverLabelObject.GetComponent<LabelShowing>();
            gameOverLabel.ShowScalingLabel(new Vector3(0, 10, -3),
                isWinning ? LanguageManager.Instance.GetTextValue("YouWinTitle") : LanguageManager.Instance.GetTextValue("GameOverTitle"), isWinning ? GameColors.DifficultyLevelsColors[DifficultyLevel._hard] :
                GameColors.DifficultyLevelsColors[Game.Difficulty], GameColors.DefaultDark, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 1, null, false, () =>
                {
                    gameOverMenu.transform.localScale = Vector3.one;
                    gameOverMenu.transform.localPosition = new Vector3(0, -70, 0);
                });
            if (!isWinning)
                DeviceButtonsHelpers.OnSoundAction(Power2Sounds.GameOver, false, true);
            else
                DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Winning, false, true);
        }

        public virtual void Drop()
        {
            if (Items == null) return;

            var generateAfterDrop = false;
            for (var col = 0; col < FieldSize; col++)
            {
                bool nextrow = false;
                for (var row = FieldSize - 1; row >= 0; row--)
                {
                    if (Items[col][row] != null || Items[col][row] == DisabledItem)
                        continue;
                    if (row == 0)
                    {
                        generateAfterDrop = true;
                        break;
                    }
                    var fromrow = row - 1;
                    while (fromrow >= 0)
                    {
                        var gobj = Items[col][fromrow] as GameObject;
                        if (Items[col][fromrow] == null || Items[col][row] == DisabledItem || (!AreStaticItemsDroppable && gobj.GetComponent<GameItem>().MovingType == GameItemMovingType.Static))
                        {
                            fromrow--;
                            _clearAfterDropDone = true;
                        }
                        else
                        {
                            generateAfterDrop = true;
                            var gims = gobj.GetComponent<GameItemMovingScript>();
                            //var dis = GetComponent<DragItemScript>();
                            nextrow = gims.IsMoving || gobj.GetComponent<GameItem>().IsTouched;
                            if (nextrow)
                                break;
                            //DropsCount++;
                            Items[col][row] = Items[col][fromrow];
                            Items[col][fromrow] = null;
                            CallbacksCount++;
                            gims.MoveTo(null, GetCellCoordinates(col, row).y, Game.StandartItemSpeed, (item, result) =>
                            {
                                CallbacksCount--;
                                if (!result) return;
                                if (_clearAfterDropDone && CallbacksCount == 0)
                                {
                                    _clearAfterDropDone = false;
                                    ClearChains();
                                }
                                LogFile.Message("New item droped Items[" + col + "][" + fromrow + "] DC: " + DropsCount, true);
                            });
                            break;
                        }
                    }
                    if (nextrow)
                        break;
                }
            }
            if (generateAfterDrop)
            {
                _isDropDone = true;
                GenerateField(true);
            }
        }

        public virtual void GenerateField(bool completeCurrent = false, bool mixCurrent = false, bool onlyNoMovesLabel = false, LabelAnimationFinishedDelegate callback = null)
        {
            if (!mixCurrent)
            {
                for (var i = FieldSize - 1; i >= 0; i--)
                {
                    var generateOnY = 1;

                    for (var j = FieldSize - 1; j >= 0; j--)
                    {
                        if (Items[i][j] != null || Items[i][j] == DisabledItem)
                            continue;
                        var deniedList = new List<GameItemType>();
                        if (completeCurrent)
                        {
                            switch (Game.Difficulty)
                            {
                                case DifficultyLevel._medium:
                                    if (ToMoveItemsCount < MaxAdditionalItemsCount)
                                    {
                                        var resCol = RandomObject.Next(ToMoveItemsCount == 0 ? 0 : (FieldSize / 2),ToMoveItemsCount == 0 ? (FieldSize / 2) : FieldSize);
                                        if (resCol == i)
                                        {
                                            Items[i][j] = GenerateGameItem(GameItemType._ToMoveItem, i, j, new Vector2(0, generateOnY), GameItemScale);
                                            ToMoveItemsCount++;
                                            generateOnY++;
                                            continue;
                                        }
                                    }
                                    break;
                            }
                            LogFile.Message("New gameItem need to i:" + i + "j: " + j, true);
                            Items[i][j] = GenerateGameItem(i, j, null, new Vector2(0, generateOnY));
                            generateOnY++;
                            continue;
                        }

                        //Horizontal before
                        if (i > 1 && CheckForLine(i - 2, j, LineOrientation.Horizontal, false) > 1)
                        {
                            var o = Items[i - 1][j] as GameObject;
                            if (o != null)
                                deniedList.Add(o.GetComponent<GameItem>().Type);
                        }
                        //Horizontal after
                        if (i < FieldSize - 2 && CheckForLine(i + 1, j, LineOrientation.Horizontal, false) > 1)
                        {
                            var gameObject1 = Items[i + 1][j] as GameObject;
                            if (gameObject1 != null)
                                deniedList.Add(gameObject1.GetComponent<GameItem>().Type);
                        }
                        //Vertical before
                        if (j > 1 && CheckForLine(i, j - 2, LineOrientation.Vertical, false) > 1)
                        {
                            var o1 = Items[i][j - 1] as GameObject;
                            if (o1 != null)
                                deniedList.Add(o1.GetComponent<GameItem>().Type);
                        }
                        //Vertical after
                        if (j < FieldSize - 2 && CheckForLine(i, j + 1, LineOrientation.Vertical, false) > 1)
                        {
                            var gameObject2 = Items[i][j + 1] as GameObject;
                            if (gameObject2 != null)
                                deniedList.Add(gameObject2.GetComponent<GameItem>().Type);
                        }
                        Items[i][j] = GenerateGameItem(i, j, deniedList);
                    }
                }
                SavedataHelper.SaveData(SavedataObject);
            }
            else
            {
                LogFile.Message("Mix field...", true);
                var o = Instantiate(LabelShowing.LabelPrefab) as GameObject;
                if (o == null) return;
                if (!onlyNoMovesLabel)
                {
                    PlaygroundProgressBar.ProgressBarRun = false;
                    callback = () =>
                    {
                        MixField();
                    };
                }
                var noMovesLabel = o.GetComponent<LabelShowing>();
                noMovesLabel.ShowScalingLabel(new Vector3(0, -2, -4),
                    LanguageManager.Instance.GetTextValue("NoMovesTitle"), GameColors.DifficultyLevelsColors[Game.Difficulty], GameColors.DefaultDark, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 2, null,
                    true, callback, true);
                //if (!onlyNoMovesLabel) MixField();
            }
        }

        protected virtual void MixField()
        {
            if (IsMixing) return;
            var dis = GetComponent<DragItemScript>();
            if (dis.IsDragging)
            {
                GetComponent<DragItemScript>().CancelDragging((s, e) => MixField());
                return;
            }
            IsMixing = true;
            DeviceButtonsHelpers.OnSoundAction(Power2Sounds.MixField, false);
            GameItem gi;
            while (!CheckForPossibleMoves())
            {
                var toMixList = new List<object>();
                for (var i = FieldSize - 1; i >= 0; i--)
                {
                    for (var j = FieldSize - 1; j >= 0; j--)
                    {
                        if (Items[i][j] == null || Items[i][j] == DisabledItem)
                            continue;
                        var go = Items[i][j] as GameObject;
                        if (go == null || (gi = go.GetComponent<GameItem>()).MovingType == GameItemMovingType.Static || gi.Type == GameItemType._ToMoveItem) continue;
                        toMixList.Add(Items[i][j]);
                    }
                }
                for (var i = FieldSize - 1; i >= 0; i--)
                {
                    for (var j = FieldSize - 1; j >= 0; j--)
                    {
                        if (Items[i][j] == null || Items[i][j] == DisabledItem)
                            continue;
                        var go = Items[i][j] as GameObject;
                        if (go == null || (gi = go.GetComponent<GameItem>()).MovingType == GameItemMovingType.Static || gi.Type == GameItemType._ToMoveItem) continue;

                        var index = RandomObject.Next(0, toMixList.Count);
                        Items[i][j] = toMixList[index];
                        toMixList.RemoveAt(index);
                    }
                }
            }
            var mixSpeed = Game.StandartItemSpeed / 3;
            for (var i = FieldSize - 1; i >= 0; i--)
            {
                for (var j = FieldSize - 1; j >= 0; j--)
                {
                    if (Items[i][j] == null || Items[i][j] == DisabledItem)
                        continue;
                    var gameObject1 = Items[i][j] as GameObject;
                    if (gameObject1 == null || (gi = gameObject1.GetComponent<GameItem>()).MovingType == GameItemMovingType.Static || gi.Type == GameItemType._ToMoveItem) continue;

                    var moving = gameObject1.GetComponent<GameItemMovingScript>();
                    var toCell = GetCellCoordinates(i, j);
                    CallbacksCount++;
                    moving.MoveTo(toCell.x, toCell.y, mixSpeed, (item, result) =>
                    {
                        CallbacksCount--;
                        //if (!result) return;
                        if (CallbacksCount == 0)
                        {
                            IsMixing = false;
                            ClearChains();
                        }
                    });
                }
            }
        }

        public virtual bool GameItemsExchange(int x1, int y1, int x2, int y2, float speed, bool isReverse, MovingFinishedDelegate exchangeCallback = null)
        {
            var item1 = Items[x1][y1] as GameObject;
            var item2 = Items[x2][y2] as GameObject;

            if (Game.isExtreme)
            {
                if (item1 != null && Items[x1][y1] != DisabledItem)
                {
                    lastMoved = item1.GetComponent<GameItem>().Type;
                }
            }

            GameMovesCount++;

            if (item2 == null)
            {
                RevertMovedItem(x1, y1);
                if (exchangeCallback != null)
                    exchangeCallback(gameObject, false);
                return false;
            }
            if (!isReverse)
            {
                Items[x1][y1] = item2;
                Items[x2][y2] = item1;
            }
            var position1 = GetCellCoordinates(x1, y1);
            var position2 = GetCellCoordinates(x2, y2);
            LogFile.Message("Exchange items: " + position1.x + position1.y + " " + position2.x + " " + position2.y, true);

            if (item1 == null || item2 == null) return false;
            isExchanging = true;
                CallbacksCount++;
                CurrentExchangeItemsCount++;
                item1.GetComponent<GameItemMovingScript>()
                    .MoveTo(position2.x,
                       position2.y,
                        Game.StandartItemSpeed, (item, result) =>
                        {
                            CallbacksCount--; 
                            //if (!result) return;
                            var currentItem = item as GameObject;
                            if (currentItem != null && isReverse)
                            {
                                CallbacksCount++;
                                currentItem.GetComponent<GameItemMovingScript>()
                                    .MoveTo(position1.x,
                                        position1.y,
                                        Game.StandartItemSpeed, (reverseItem, reverseResult) =>
                                        {
                                            CallbacksCount--;
                                            CurrentExchangeItemsCount--;
                                            if (CurrentExchangeItemsCount == 0)
                                                isExchanging = false;
                                            if (exchangeCallback != null)
                                                exchangeCallback(gameObject, true);
                                            if (CallbacksCount == 0 && CallClearChainsAfterExchange)
                                            {
                                                CallClearChainsAfterExchange = false;
                                                ClearChains();
                                            }
                                        });
                                return;
                            }
                            CurrentExchangeItemsCount--;
                            if (CurrentExchangeItemsCount == 0)
                                isExchanging = false;
                            if (exchangeCallback != null)
                                exchangeCallback(gameObject, true);
                            if (CallbacksCount == 0)
                            {
                                CallClearChainsAfterExchange = false;
                                ClearChains();
                            }

                        });

            CallbacksCount++;
            CurrentExchangeItemsCount++;
            item2.GetComponent<GameItemMovingScript>()
                    .MoveTo(position1.x,
                        position1.y,
                        Game.StandartItemSpeed, (item, result) =>
                        {
                            CallbacksCount--;
                            //if (!result) return;
                            var currentItem = item as GameObject;
                            if (currentItem != null && isReverse)
                            {
                                CallbacksCount++;
                                currentItem.GetComponent<GameItemMovingScript>()
                                    .MoveTo(position2.x,
                                        position2.y,
                                        Game.StandartItemSpeed, (reverseItem, reverseResult) =>
                                        {
                                            CallbacksCount--;
                                            CurrentExchangeItemsCount--;
                                            if (CurrentExchangeItemsCount == 0)
                                                isExchanging = false;
                                            if (exchangeCallback != null)
                                                exchangeCallback(gameObject, true);
                                            if (CallbacksCount == 0 && CallClearChainsAfterExchange)
                                            {
                                                CallClearChainsAfterExchange = false;
                                                ClearChains();
                                            }
                                        });
                                return;
                            }
                            CurrentExchangeItemsCount--;
                            if (CurrentExchangeItemsCount == 0)
                                isExchanging = false;
                            if (exchangeCallback != null)
                                exchangeCallback(gameObject, true);
                            if (CallbacksCount == 0)
                            {
                                CallClearChainsAfterExchange = false;
                                ClearChains();
                            }
                        });

            return true;
        }

        public virtual bool IsItemMovingAvailable(int col, int row, MoveDirections mdir)
        {
            if (!AvailableMoveDirections.ContainsKey(mdir)) return false;
            if (col < 0 || row < 0 || col >= FieldSize || row >= Items[col].Length)
                return false;

            var gameObject1 = Items[col][row] as GameObject;
            if (gameObject1 == null || gameObject1.GetComponent<GameItem>().MovingType == GameItemMovingType.Static) return false;

            switch (mdir)
            {
                case MoveDirections.Up:
                    if (row == 0 || Items[col][row - 1] == DisabledItem)
                        return false;
                    var objectUp = Items[col][row - 1] as GameObject;

                    if (objectUp == null || objectUp.GetComponent<GameItemMovingScript>().IsMoving || objectUp.GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                        return false;
                    break;
                case MoveDirections.Down:
                    if (row == (FieldSize - 1) || Items[col][row + 1] == DisabledItem)
                        return false;
                    var objectDown = Items[col][row + 1] as GameObject;

                    if (objectDown == null || objectDown.GetComponent<GameItemMovingScript>().IsMoving || objectDown.GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                        return false;
                    break;
                case MoveDirections.Left:
                    if (col == 0 || Items[col - 1][row] == DisabledItem)
                        return false;
                    var objectLeft = Items[col - 1][row] as GameObject;

                    if (objectLeft == null || objectLeft.GetComponent<GameItemMovingScript>().IsMoving || objectLeft.GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                        return false;
                    break;
                case MoveDirections.Right:
                    if (col == (FieldSize - 1) || Items[col + 1][row] == DisabledItem)
                        return false;
                    var objectRight = Items[col + 1][row] as GameObject;

                    if (objectRight == null || objectRight.GetComponent<GameItemMovingScript>().IsMoving || objectRight.GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                        return false;
                    break;
            }
            return true;
        }

        public virtual bool IsPointInLine(int col, int row)
        {
            var hor = 0;

            //Horizontal before
            var gobj = Items[col][row] as GameObject;
            if (gobj == null) return false;
            var gi = gobj.GetComponent<GameItem>();
            for (var i = col - 1; i >= 0; i--)
            {
                var gobj2 = Items[i][row] as GameObject;
                if (gobj2 == null) continue;
                if (gobj2.GetComponent<GameItemMovingScript>().IsMoving) break;
                var gi2 = gobj2.GetComponent<GameItem>();
                if (gi.Type == gi2.Type)
                    hor++;
                else
                    break;
            }
            if (hor < 3)
            {
                for (var i = col + 1; i < FieldSize; i++)
                {
                    var gobj2 = Items[i][row] as GameObject;
                    if (gobj2 == null) continue;
                    if (gobj2.GetComponent<GameItemMovingScript>().IsMoving) break;
                    var gi2 = gobj2.GetComponent<GameItem>();
                    if (gi.Type == gi2.Type)
                        hor++;
                    else
                        break;
                }
            }
            else return true;
            if (hor > 1) return true;
            var vert = 0;

            //Vertical before
            for (var j = row - 1; j >= 0; j--)
            {
                var gobj2 = Items[col][j] as GameObject;
                if (gobj2 == null) continue;
                if (gobj2.GetComponent<GameItemMovingScript>().IsMoving) break;
                var gi2 = gobj2.GetComponent<GameItem>();
                if (gi.Type == gi2.Type)
                    vert++;
                else break;
            }
            if (vert < 3)
            {
                for (var j = row + 1; j < FieldSize; j++)
                {
                    var gobj2 = Items[col][j] as GameObject;
                    if (gobj2 == null) continue;
                    if (gobj2.GetComponent<GameItemMovingScript>().IsMoving) break;
                    var gi2 = gobj2.GetComponent<GameItem>();
                    if (gi.Type == gi2.Type)
                        vert++;
                    else break;
                }
            }
            else return true;
            return vert > 1;
        }

        public virtual bool TryMakeMove(int x1, int y1, int x2, int y2)
        {
            if (Items[x1][y1] == null || Items[x1][y1] == DisabledItem ||
                Items[x2][y2] == null || Items[x2][y2] == DisabledItem)
                return false;
            var o = Items[x1][y1] as GameObject;
            if (o != null && o.GetComponent<GameItem>().MovingType == GameItemMovingType.StandartExchangable)
                return true;
            var tItem = Items[x1][y1];
            Items[x1][y1] = Items[x2][y2];
            Items[x2][y2] = tItem;
            var ret = IsPointInLine(x1, y1) || IsPointInLine(x2, y2);
            tItem = Items[x2][y2];
            Items[x2][y2] = Items[x1][y1];
            Items[x1][y1] = tItem;
            return ret;
        }

        public virtual bool CheckForPossibleMoves()
        {
            for (var col = 0; col < FieldSize; col++)
                for (var row = 0; row < FieldSize; row++)
                {
                    if (Items[col][row] == null)
                        return true;
                    if (Items[col][row] == DisabledItem)
                        //if (Items[col][row] == null) Debug.LogError("Items[col][row] null in checkForPossibleMoves");
                        continue;
                    var gobj = Items[col][row] as GameObject;
                    if (gobj == null) continue;
                    var gi = gobj.GetComponent<GameItem>();
                    var positionPoint = new Point { X = col, Y = row };
                    // возможна горизонтальная, две подряд      
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 1, Y = 0 }, new List<Point>
                    {
                        new Point{X = -2, Y = 0}, new Point{X = -1, Y = -1}, new Point{X = -1, Y = 1}, new Point{X = 2, Y = -1},
                        new Point{X = 2, Y = 1}, new Point{X = 3, Y = 0}
                    }
                        ))
                        return true;
                    // возможна горизонтальная, две по разным сторонам      
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 2, Y = 0 }, new List<Point>
                    {
                        new Point{X = 1, Y = -1}, new Point{X = 1, Y = 1}
                    }
                        ))
                        return true;
                    // возможна вертикальная, две подряд      
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 0, Y = 1 }, new List<Point>
                    {
                        new Point{X = 0, Y = -2}, new Point{X = -1, Y = -1}, new Point{X = 1, Y = -1}, new Point{X = -1, Y = 2},
                        new Point{X = 1, Y = 2}, new Point{X = 0, Y = 3}
                    }
                        ))
                        return true;
                    // возможна вертикальная, две по разным сторонам       
                    if (MatchPattern(gi.Type, positionPoint, new Point { X = 0, Y = 2 }, new List<Point>
                    {
                        new Point{X = -1, Y = 1}, new Point{X = 1, Y = 1}
                    }
                        ))
                        return true;
                }
            return false;
        }

        protected void RaisePoints(int points)
        {
            if (points == 0)
            {
                LogFile.Message("Rised 0 ponts", true);
                return;
            }

            LogFile.Message("Points " + points, true);

            CurrentScore += points;
        }

        public void ShowComboLabel(int count)
        {
            var labelObject = Instantiate(LabelShowing.LabelPrefab) as GameObject;
            if (labelObject == null) return;
            var comboLabel = labelObject.GetComponent<LabelShowing>();
            comboLabel.name = "ComboLabel";
            comboLabel.transform.SetParent(transform);
            var isLeft = count % 2 == 0;
            if (isLeft)
            {
                if (_leftComboLabel != null)
                {
                    var shadow = _leftComboLabel.GetComponent<LabelShowing>().Shadow;
                    if (shadow != null)
                        Destroy(shadow.gameObject);
                    Destroy(_leftComboLabel);
                }
                _leftComboLabel = labelObject;
            }
            else
            {
                if (_rightComboLabel != null)
                {
                    var shadow = _rightComboLabel.GetComponent<LabelShowing>().Shadow;
                    if (shadow != null)
                        Destroy(shadow.gameObject);
                    Destroy(_rightComboLabel);
                }
                _rightComboLabel = labelObject;
            }

            comboLabel.ShowScalingLabel(new Vector3(isLeft ? -150 : 150, InitialGameItem.Y + GameItemSize * 2.5f, -1),
                LanguageManager.Instance.GetTextValue("ComboTitle") + count, GameColors.DifficultyLevelsColors[Game.Difficulty], GameColors.DifficultyLevelsColors[Game.Difficulty], LabelShowing.minLabelFontSize - 20, LabelShowing.minLabelFontSize, 1, null, true, null, false,
                count % 2 == 0 ? 30 : -30);
            //DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Combo, false);
        }

        public virtual void RevertMovedItem(int col, int row, MovingFinishedDelegate callback = null)
        {
            var gobj = Items[col][row] as GameObject;
            if (gobj == null) return;
            var toCell = GetCellCoordinates(col, row);
            LogFile.Message("Revert item to place: " + toCell.x + " " + toCell.y, true);
            var gims = gobj.GetComponent<GameItemMovingScript>();
            CallbacksCount++;
            gims.MoveTo(toCell.x, toCell.y, Game.StandartItemSpeed, (s, e) =>
                {
                    CallbacksCount--;
                    if (callback != null)
                        callback(s, e);
                });
        }

        public virtual void ResetPlayground()
        {
            for (var i = 0; i < FieldSize; i++)
                for (var j = 0; j < FieldSize; j++)
                {
                    /*var item = Items[i][j] as GameObject;
                    if (item != null)
                    {
                        var gims = item.GetComponent<GameItemMovingScript>();
                        if (gims != null) gims.CancelMoving(true);
                    }
                    Items[i][j] = null;
                    Destroy(item);*/
                    RemoveGameItem(i, j);
                }
            Items = null;
            MaxType = GameItemType._3;
            CurrentScore = 0;
        }

        public void RemoveGameItem(int i, int j, MovingFinishedDelegate removingCallback = null)
        {
            var o = Items[i][j] as GameObject;
            if (o == null) return;

            if (o.GetComponent<GameItem>().IsTouched)
            {
                GetComponent<DragItemScript>().CancelDragging((s, e) => RemoveGameItem(i, j));
                return;
            }

            var giss = o.GetComponent<GameItemScalingScript>();
            var toSize = 0.5f;
            CallbacksCount++;
            Items[i][j] = null;
            _isDropDone = false;
            giss.ScaleTo(new Vector3(toSize, toSize, 0), 8, (item, r) =>
            {
                CallbacksCount--;
                DestroyGameItem((GameObject)item);
                if (removingCallback != null)
                    removingCallback(item, r);
            });
        }

        protected void CreateInGameHelpModule(string modulePostfix, LabelAnimationFinishedDelegate callback = null)
        {
            if (PlayerPrefs.HasKey(modulePostfix))
            {
                if (callback != null)
                    callback();
                return;
            }
            if (Game.Foreground == null) return;
            var manualPrefab = LanguageManager.Instance.GetPrefab("UserHelp." + modulePostfix);
            if (manualPrefab == null)
            {
                if (callback != null)
                    callback();
                return;
            }
            var manual = Instantiate(manualPrefab);
            var resource = Resources.Load("Prefabs/InGameHelper");
            Time.timeScale = 0F;
            PauseButtonScript.PauseMenuActive = true;
            UserHelpScript.InGameHelpModule = Instantiate(resource) as GameObject;
            if (UserHelpScript.InGameHelpModule != null)
            {
                UserHelpScript.InGameHelpModule.transform.SetParent(Game.Foreground.transform);
                UserHelpScript.InGameHelpModule.transform.localScale = Vector3.one;
                UserHelpScript.InGameHelpModule.transform.localPosition = new Vector3(0, 0, -6);
                manual.transform.SetParent(UserHelpScript.InGameHelpModule.transform);
            }
            //manual.transform.localScale = new Vector3(45, 45, 0);
            manual.transform.localPosition = new Vector3(0, 60, -2);
            UserHelpScript.ShowUserHelpCallback = callback;
            PlayerPrefs.SetInt(modulePostfix, 1);
        }

    }
}