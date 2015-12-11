using System;
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
    public delegate void LabelAnimationFinishedDelegate();
    class SquarePlayground : MonoBehaviour, IPlayground
    {
        
        private static readonly System.Object _disabledItem = new object();
        private readonly AutoResetEvent _callbackReady = new AutoResetEvent(false);
        //private readonly ManualResetEvent _dropsReady = new ManualResetEvent(false);
        private const float HintDelayTime = 4;
        private volatile int _callbacksCount;
        private int _dropsCount;
        private RealPoint _item00;
        private float _timeCounter = -1;
        protected const float MixTimeCounterSize = 16; //every 16 seconds field mixes in veryhard difficulty mode
        //private float _moveTimer = 8;
        //private float _moveTimerMultiple = 10;
        private GameObject _selectedPoint1;
        private GameObject _selectedPoint2;
        private Point _selectedPoint1Coordinate;
        private Point _selectedPoint2Coordinate;
        private int _chainCounter;
        private GameItemType _nextUpperLevelGameItemType = GameItemType.NullItem;
        private float _currentTime;
        private int _minTypePlus = 0;
        //private Int32 _lowestNullItem;

        protected static readonly System.Random RandomObject = new System.Random();
        protected GameItemType MaxType = GameItemType._3;

        protected const int MaxAdditionalItemsCount = 2;
        protected int DropDownItemsCount;
        protected int XItemsCount;
        protected int _2xItemsCount = 0;
        private bool _isGameOver;
        private bool _isMixing = false;
        protected bool _callClearChainsAfterExchange = false;
        protected int _currentExchangeItemsCount = 0;
        protected float _initialMoveTimerMultiple = 32;
        protected bool _showTimeLabel = true;

        protected virtual String _userHelpPrefix
        {
            get { return ""; }
        }


        public virtual IGameSettingsHelper Preferenses
        {
            get { return GameSettingsHelper<SquarePlayground>.Preferenses; }
        }

        
        public float MoveTimerMultiple
        {
            get
            {
                return _initialMoveTimerMultiple;
            }
            protected set
            {
                gameObject.GetComponent<PlaygroundProgressBar>().MoveTimerMultiple = _initialMoveTimerMultiple;
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
        
        public int AdditionalItemCost { get { return 222; } }

        protected GameItemType MinType
        {
            get { return MaxType - FieldSize > GameItemType._1 ? MaxType - FieldSize : GameItemType._1; }
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

        public virtual float ScaleMultiplyer
        {
            get { return 5.4f; }
            //get { return 1.2F; }
        }

        public virtual String ItemPrefabName { get { return ItemsNameHelper.GetPrefabPath<SquarePlayground>(); } }

        public virtual string ItemBackgroundTextureName { get { return ItemsNameHelper.GetBackgroundTexturePrefix<SquarePlayground>(); } }

        public virtual bool AreStaticItemsDroppable { get { return false; } }

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
                if (value == null || Items[value.X][value.Y] == null) return;
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
                if (value == null || Items[value.X][value.Y] == null) return;
                _selectedPoint2Coordinate = value;
            }
        }

        protected bool _raiseMaxInitialElement = false;

        public GameItemType MaxInitialElementType
        {
            get { return MaxType; }
            set
            {
                if (MaxType == value) return;

                MaxType = value;
                if (Preferenses.CurrentItemType < MaxType)
                    Preferenses.CurrentItemType = MaxType;
                _raiseMaxInitialElement = true;
                ShowMaxInitialElement();
            }
        }

        protected virtual void MaxInitialElementTypeRaisedActions()
        {
            //if (!(_nextUpperLevelGameItemType != GameItemType.NullItem && MaxType == _nextUpperLevelGameItemType)
            if (MaxType < GameItemType._7)
                _nextUpperLevelGameItemType = GameItemType._7;
            else
            {
                switch (MaxType)
                {
                    case GameItemType._7:
                        Game.Difficulty = DifficultyLevel._medium;
                        _minTypePlus=1;
                        MoveTimerMultiple = _initialMoveTimerMultiple + 4;
                        DifficultyRaisedGUI(_nextUpperLevelGameItemType != GameItemType.NullItem, MaxInitialElementTypeRaisedActionsAdditional);
                        DeviceButtonsHelpers.OnSoundAction(Power2Sounds.NextLevel, false);
                        _nextUpperLevelGameItemType = GameItemType._10;
                        break;
                    case GameItemType._8:
                    case GameItemType._9:
                        _minTypePlus = 0;
                        _nextUpperLevelGameItemType = GameItemType._10;
                        break;
                    case GameItemType._10:
                        Game.Difficulty = DifficultyLevel._hard;
                        _minTypePlus=1;
                        MoveTimerMultiple = _initialMoveTimerMultiple + 8;
                        DifficultyRaisedGUI(_nextUpperLevelGameItemType != GameItemType.NullItem, MaxInitialElementTypeRaisedActionsAdditional);
                        if (_2xItemsCount < 1)
                        Generate2xItem();
                        DeviceButtonsHelpers.OnSoundAction(Power2Sounds.NextLevel, false);
                        SpawnXItems();
                        _nextUpperLevelGameItemType = GameItemType._13;
                        break;
                    case GameItemType._11:
                    case GameItemType._12:
                        _minTypePlus = 0;
                        _nextUpperLevelGameItemType = GameItemType._13;
                        break;
                    case GameItemType._13:
                        Game.Difficulty = DifficultyLevel._veryhard;
                        _minTypePlus=1;
                        MoveTimerMultiple = _initialMoveTimerMultiple + 12;
                        DifficultyRaisedGUI(_nextUpperLevelGameItemType != GameItemType.NullItem, MaxInitialElementTypeRaisedActionsAdditional);
                        if (_2xItemsCount < 2)
                        Generate2xItem();
                        DeviceButtonsHelpers.OnSoundAction(Power2Sounds.NextLevel, false);
                        _nextUpperLevelGameItemType = GameItemType._2x;
                        break;
                    case GameItemType._14:
                    case GameItemType._15:
                    case GameItemType._16:
                        _minTypePlus = 0;
                        _nextUpperLevelGameItemType = GameItemType._2x;
                        ProgressBar.TimeBorderActivated += VeryHardLevelAction;
                        break;
                    case GameItemType._2x:
                        IsGameOver = true;
                        GenerateGameOverMenu(true);
                        return;
                }
                if ((int)MaxType > FieldSize)
                {
                    DestroyElements(new List<GameItemType>() {MinType, MinType+_minTypePlus});
                    //GenerateField();
                }
            }
            
        }

        protected virtual void MaxInitialElementTypeRaisedActionsAdditional(object o, EventArgs e)
        {
            ProgressBar.TimeBorderActivated += VeryHardLevelAction;
        }


        protected virtual void SpawnXItems()
        {
            while (XItemsCount < MaxAdditionalItemsCount)
            {
                int col;
                int row;
                while ((col = RandomObject.Next(1, FieldSize - 1)) * RandomObject.Next(1, FieldSize - 1) >
                       (row = RandomObject.Next(1, FieldSize - 1)) * RandomObject.Next(1, FieldSize - 1) ||
                       Items[col][row] == null || Items[col][row] == DisabledItem ||
                       (Items[col][row] as GameObject).GetComponent<GameItem>().Type >= GameItemType._2x ||
                       (Items[col][row] as GameObject).GetComponent<GameItemScalingScript>().isScaling)
                {
                }
                RemoveGameItem(col, row, (item, r) =>
                {
                    Items[col][row] = GenerateGameItem(GameItemType._XItem, col, row, Vector2.zero,
                        false, null, null);
                });
                XItemsCount++;
            }
        }

        protected void DifficultyRaisedGUI(bool withLabel = true, EventHandler callback = null)
        {
            var go = GameObject.Find("/Middleground/Background");
            var oits = go.GetComponent<ObjectImageTransparencyScript>();

            oits.SetTransparency(0.1f, (obj, res) =>
            {
                var backgroundObject = GameObject.Find("/Middleground/Background");
                backgroundObject.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>(ItemBackgroundTextureName)
                    .SingleOrDefault(t => t.name.Contains(Game.Difficulty.ToString()));

                GetComponent<PlaygroundProgressBar>().UpdateTexture();

                var points = GameObject.Find("Points");
                points.GetComponent<Text>().color = GameColors.DifficultyLevelsColors[Game.Difficulty];

                oits.SetTransparency(1f, null);
            });


            #region Needn't case
            //switch (Game.Difficulty)
            //{
            //    case DifficultyLevel.medium:
            //        oits.SetTransparency(0.1f, (obj, res) =>
            //            {
            //                var backgroundObject = GameObject.Find("/Middleground/Background");
            //                backgroundObject.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>(ItemBackgroundTextureName)
            //                    .SingleOrDefault(t => t.name.Contains(DifficultyLevel.medium.ToString()));

            //                GetComponent<PlaygroundProgressBar>().UpdateTexture(DifficultyLevel.medium);

            //                oits.SetTransparency(1f, null);
            //            });
            //        break;
            //    case DifficultyLevel.hard:
            //        oits.SetTransparency(0.1f, (obj, res) =>
            //             {
            //                 var backgroundObject = GameObject.Find("/Middleground/Background");
            //                 backgroundObject.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>(ItemBackgroundTextureName)
            //                    .SingleOrDefault(t => t.name.Contains(DifficultyLevel.hard.ToString()));

            //                 GetComponent<PlaygroundProgressBar>().UpdateTexture(DifficultyLevel.hard);

            //                 oits.SetTransparency(1f, null);
            //             });
            //        break;
            //    case DifficultyLevel.veryhard:
            //        oits.SetTransparency(0.1f, (obj, res) =>
            //             {
            //                 var backgroundObject = GameObject.Find("/Middleground/Background");
            //                 backgroundObject.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>(ItemBackgroundTextureName)
            //                    .SingleOrDefault(t => t.name.Contains(DifficultyLevel.veryhard.ToString()));

            //                 GetComponent<PlaygroundProgressBar>().UpdateTexture(DifficultyLevel.veryhard);

            //                 oits.SetTransparency(1f, null);
            //             });
            //        break;
            //}
            #endregion

            if (!withLabel)
            {
                if (callback != null)
                    callback(null, EventArgs.Empty);
                return;
            }

            var difficultyRaisedLabel = (Instantiate(Resources.Load("Prefabs/Label")) as GameObject).GetComponent<LabelShowing>();

            difficultyRaisedLabel.ShowScalingLabel(new Vector3(0, -2, -4), LanguageManager.Instance.GetTextValue(Game.Difficulty.ToString()),
                GameColors.DifficultyLevelsColors[Game.Difficulty], GameColors.DefaultDark, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 2, null, true, () =>
                {
                    CreateInGameHelpModule(_userHelpPrefix + Game.Difficulty.ToString(), () =>
                    {
                            if (callback != null)
                                callback(null, EventArgs.Empty);
                            if (_showTimeLabel)
                            {
                                _showTimeLabel = false;
                                ShowTimeLabel();
                            }
                            else
                                PlaygroundProgressBar.ProgressBarRun = true;
                            
                        });
                }, true);
        }

        protected virtual void VeryHardLevelAction(object sender, EventArgs e)
        {
            var actionIndex = RandomObject.Next(0, 2);
            switch(actionIndex)
            {
                case 0:
                    int col = RandomObject.Next(0, FieldSize-1);
                    for (int row = 0; row < FieldSize; row++)
                    {
                        var gobj = Items[col][row] as GameObject;
                        if (gobj == null) continue;
                        var gi = gobj.GetComponent<GameItem>();
                        if (gi.MovingType != GameItemMovingType.Static)
                            RemoveGameItem(col, row);
                    }
                        return;
                case 1:
                    int row1 = RandomObject.Next(0, FieldSize-1);
                    for (int col1 = 0; col1 < FieldSize; col1++)
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
            var showTimeLabel = (Instantiate(Resources.Load("Prefabs/Label")) as GameObject).GetComponent<LabelShowing>();
            var fg = GameObject.Find("/Foreground");
            showTimeLabel.transform.SetParent(fg.transform);
            showTimeLabel.ShowScalingLabel(new Vector3(ProgressBar.Coordinate.x, ProgressBar.Coordinate.y, -4), LanguageManager.Instance.GetTextValue("TimeStart"),
               GameColors.DefaultLabelColor, GameColors.DefaultDark, LabelShowing.minLabelFontSize - 40, LabelShowing.maxLabelFontSize - 56, 1, null, true, () =>
               {
                   PlaygroundProgressBar.ProgressBarRun = true;
               });
        }

        private void Generate2xItem()
        {
            int col;
            int row;
            while ((col = RandomObject.Next(1, FieldSize - 1)) * RandomObject.Next(1, FieldSize - 1) >
                       (row = RandomObject.Next(1, FieldSize - 1)) * RandomObject.Next(1, FieldSize - 1))
            {
                var gi = (Items[col][row] as GameObject).GetComponent<GameItem>();
                if (gi.Type < GameItemType._2x && gi.Type != MaxType + 1 &&
                !gi.GetComponent<GameItemScalingScript>().isScaling)
                    break;
            }
            RemoveGameItem(col, row, (item, r) =>
            {
                Items[col][row] = GenerateGameItem(GameItemType._2x, col, row, Vector2.zero);
            });
        }

        public void ShowMaxInitialElement()
        {
            var fg = GameObject.Find("/Foreground");
            var cmi = GameObject.Find("/Foreground/MaximumItem");
            var gobj = Instantiate(Resources.Load(ItemPrefabName + MaxType)) as GameObject;
            if (gobj == null) return;
            if (cmi != null)
            cmi.transform.localPosition = new Vector3(cmi.transform.localPosition.x, cmi.transform.localPosition.y, 0);
            gobj.transform.SetParent(fg.transform);
            gobj.transform.localPosition = new Vector3(0, 400f, -1);
            gobj.transform.localScale = new Vector3(16, 16);
            gobj.name = "MaximumItem";
            gobj.AddComponent<Button>();
            Image image = gobj.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0);
            var buttonComponent = gobj.GetComponent<Button>();
            if (buttonComponent != null)
                buttonComponent.onClick.AddListener(() => ThemeChooserScript.OnScriptGameThemeChange());
            var c = gobj.GetComponent<GameItemMovingScript>();
            LogFile.Message("GameItem generated to X:" + gobj.transform.localPosition.x + " Y:" + (gobj.transform.localPosition.y), true);
            c.MoveTo(null, 340f, Game.standartItemSpeed / 2, (item, result) =>
            {
                if (!result) return;
                if (cmi != null)
                    Destroy(cmi);
            });
        }

        public void DestroyElements(List<GameItemType> withTypes)
        {
            foreach(var t in withTypes)
            LogFile.Message("Destroy elements above " + t);
            var pointsBank = 0;
           // var dis = GetComponent<DragItemScript>();
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
                        /*if (dis.IsDragging && dis.TouchedItem.X == i && dis.TouchedItem.Y == j)
                            dis.CancelDragging((s,e)=>RemoveGameItem(i, j));
                        else*/
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

        public float DeltaToMove { get { return GameItemSize / 4f; } }

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
            //var stat = GetComponent<Game>().Stats;
            //if (stat != null && stat.LongestSession < CurrentTime + Time.time)
            if (Preferenses.LongestSession < CurrentTime + Time.timeSinceLevelLoad)
                Preferenses.LongestSession = CurrentTime + Time.timeSinceLevelLoad;
        }

        public float CurrentTime
        {
            get { return _currentTime; }
            set
            {
                if (_currentTime == value) return;
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

        public RealPoint Item00
        {
            get { return _item00; }
            set { _item00 = value; }
        }

        protected AutoResetEvent CallbackReady
        {
            get { return _callbackReady; }
        }

        //protected ManualResetEvent DropsReady
        //{
        //    get { return _dropsReady; }
        //}

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

        protected bool RemoveAdditionalItems()
        {
            bool result = false;
            for (var i = 0; i < FieldSize; i++)
                if (Items[i][FieldSize - 1] != null && Items[i][FieldSize - 1] != DisabledItem)
                {
                    var gobj = Items[i][FieldSize - 1] as GameObject;
                    if (gobj == null ||
                        (gobj.GetComponent<GameItem>().Type != GameItemType._DropDownItem ||
                         gobj.GetComponent<GameItemMovingScript>().IsMoving)) continue;
                   /* var o = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                    if (o != null)
                    {
                        var pointsLabel = o.GetComponent<LabelShowing>();
                        pointsLabel.transform.SetParent(transform);*/
                        LabelShowing.ShowScalingLabel(gobj/*new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y + GameItemSize / 2, gobj.transform.localPosition.z - 1)*/,
                            "+" + 222, Color.white, Color.gray, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 3, Game.numbersFont, true, null, 0, GameItemType._DropDownItem);
                    //}
                    RaisePoints(AdditionalItemCost);
                    if (ProgressBar != null)
                        ProgressBar.AddTime(AdditionalItemCost * 2);

                    //Destroy(gobj);
                    RemoveGameItem(i, FieldSize - 1, (s, e) =>
                        {
                            //Items[i][FieldSize - 1] = null;
                            DropDownItemsCount--;
                        });
                    result = true;
                }
            return result;
        }

        protected virtual void Update()
        {
            if (IsGameOver || PauseButtonScript.PauseMenuActive) return;

            if (CallbacksCount == 0)
                Drop();
            if (!(HintTimeCounter >= 0)) return;
            if (HintTimeCounter > HintDelayTime && _selectedPoint1 == null && _selectedPoint2 == null)
            {
                var parentGobj = Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] as GameObject;
                if (parentGobj == null) return;
                _selectedPoint1 = Instantiate(Resources.Load(ItemPrefabName + "_SelectedItem")) as GameObject;
                if (_selectedPoint1 == null)
                {
                    LogFile.Message("SelectedPoint1 initialization failed", true);
                    return;
                }
                _selectedPoint1.transform.SetParent(parentGobj.transform);
                _selectedPoint1.transform.localScale = new Vector3(1.2f, 1.2f);
                _selectedPoint1.transform.localPosition = new Vector3(0, -0.03f, -1);
                parentGobj = Items[SelectedPoint2Coordinate.X][SelectedPoint2Coordinate.Y] as GameObject;
                if (parentGobj == null) return;
                _selectedPoint2 = Instantiate(Resources.Load(ItemPrefabName + "_SelectedItem")) as GameObject;
                if (_selectedPoint2 == null)
                {
                    LogFile.Message("SelectedPoint2 initialization failed", true);
                    return;
                }
                _selectedPoint2.transform.SetParent(parentGobj.transform);
                _selectedPoint2.transform.localScale = new Vector3(1.2f, 1.2f);
                _selectedPoint2.transform.localPosition = new Vector3(0, -0.03f, -1);
            }
            HintTimeCounter += Time.deltaTime;
        }

        public GameObject InstantiateGameItem(GameItemType itemType, Vector3 localPosition, Vector3 localScale, GameItemMovingType? movingType = null)
        {
            var newgobj = Instantiate(Resources.Load(ItemPrefabName + itemType)) as GameObject;
            if (newgobj == null) return null;
            var rectTransform = newgobj.transform as RectTransform;
            if (rectTransform != null)
                rectTransform.sizeDelta = new Vector2(GameItemSize * 0.9375f, GameItemSize * 0.9375f);
            newgobj.transform.SetParent(transform);
            newgobj.transform.localPosition = localPosition;
            newgobj.transform.localScale = localScale;
            if(movingType.HasValue)
                newgobj.GetComponent<GameItem>().MovingType = movingType.Value;
            return newgobj;
        }

        public GameObject GenerateGameItem(GameItemType itemType, int i, int j, Vector2? generateOn = null, bool isItemDirectionChangable = false, float? dropSpeed = null, MovingFinishedDelegate movingCallback = null, GameItemMovingType? movingType = null)
        {
            if (!generateOn.HasValue)
                generateOn = new Vector2(0, FieldSize - j);
            if (Item00 == null)
                Item00 = InitialGameItemPosition;
            var cell = GetCellCoordinates(i, j);
            var gobj = InstantiateGameItem(itemType, new Vector3(
                (float)Math.Round(cell.x + generateOn.Value.x * GameItemSize, 2),
                /*(float)Math.Round(*/Item00.Y + generateOn.Value.y * GameItemSize/*, 2)*/,
                Item00.Z), Vector3.zero, movingType);
            //if (generateOn == Vector2.zero) //TODO: WHAT?
            //{
            //    var c = gobj.GetComponent<GameItemScalingScript>();
            //    c.ScaleTo(new Vector3(GameItemSize / ScaleMultiplyer, GameItemSize / ScaleMultiplyer, 1f), 8, null);
            //}
            //else
            //{
            var c = gobj.GetComponent<GameItemMovingScript>();
            LogFile.Message("GameItem generated to X:" + gobj.transform.localPosition.x + " Y:" + (gobj.transform.localPosition.y - 6 * GameItemSize), true);
            CallbacksCount++;
            var toS = GameItemSize / ScaleMultiplyer;
            c.MoveTo(cell.x, cell.y, dropSpeed.HasValue ? dropSpeed.Value : 10 - i % 2 + j * 1.5f, (item, result) =>
            {
                CallbacksCount--;
                if (movingCallback != null)
                    movingCallback(item, result);
                else
                {
                    if (!result) return;
                    if (CallbacksCount == 0)
                        ClearChains();
                    else
                        if (CallbacksCount == _currentExchangeItemsCount)
                            _callClearChainsAfterExchange = true;
                }
            }, new Vector2(Item00.X, Item00.Y + GameItemSize / 2), new Vector3(toS, toS, 1f), isItemDirectionChangable,
            null, Power2Sounds.Drop);
            //}
            return gobj;
        }

        public GameObject GenerateGameItem(int i, int j, IList<GameItemType> deniedTypes = null, Vector2? generateOn = null, bool isItemDirectionChangable = false, float? dropSpeed = null, MovingFinishedDelegate movingCallback = null, GameItemMovingType? movingType = null)
        {
            //var minType = MaxType - FieldSize;
            var newType = RandomObject.Next((int)MaxType > FieldSize ? (int)MinType + 1 + _minTypePlus : (int)GameItemType._1, (int)MaxInitialElementType + 1);
            if (deniedTypes == null || deniedTypes.Count == 0)
                return GenerateGameItem((GameItemType)newType, i, j, generateOn, isItemDirectionChangable, dropSpeed, movingCallback, movingType);
            while (deniedTypes.Contains((GameItemType)newType))
                newType = RandomObject.Next((int)GameItemType._1, (int)MaxInitialElementType + 1);
            return GenerateGameItem((GameItemType)newType, i, j, generateOn, isItemDirectionChangable, dropSpeed, movingCallback);
        }

        //private bool IsEndPoint(IEnumerable<Line> lines, int currentX, int currentY)
        //{
        //    return lines.Any(l => (l.X2 == currentX && l.Y2 == currentY));
        //}

        public virtual bool IsInAnotherLine(IEnumerable<Line> lines, int currentX, int currentY)
        {
            return lines.Count(l => (l.X1 <= currentX && l.X2 >= currentX && l.Y1 <= currentY && l.Y2 >= currentY)) > 1;
        }

        public bool MatchType(int x, int y, GameItemType itemType)
        {
            // убедимся, что фишка не выходит за пределы поля    
            if ((x < 0) || (x >= FieldSize) || (y < 0) || (y >= FieldSize) || Items[x][y] == null || Items[x][y] == DisabledItem) return false;
            var gobj = Items[x][y] as GameObject;
            if (gobj == null) return false;
            var gi = gobj.GetComponent<GameItem>();
            return gi != null && gi.Type == itemType;
        }

        public virtual Vector3 GetCellCoordinates(int col, int row)
        {
            var roundedX = Math.Round(Item00.X + Convert.ToSingle(col) * GameItemSize, 2);
            var roundedY = Math.Round(Item00.Y - GameItemSize * Convert.ToSingle(row), 2);
            return new Vector3((float)roundedX, (float)roundedY, Item00.Z);
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
                if (Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] != null)
                {
                    var selectedObject = Items[SelectedPoint1Coordinate.X][SelectedPoint1Coordinate.Y] as GameObject;
                    if (selectedObject != null)
                    {
                        var gi = selectedObject.GetComponent<GameItem>();
                        if (gi.MovingType == GameItemMovingType.Static || gi.Type == GameItemType.NullItem || gi.Type == GameItemType.DisabledItem)
                            continue;
                    }
                }
                SelectedPoint2 = new Point { X = firstItem.X + lineGenerationPoint.X, Y = firstItem.Y + lineGenerationPoint.Y };
                return true;
            }
            return false;
        }

        public virtual IList<Line> GetAllLines()
        {
            //LogFile.Message("Finding lines...");
            var list = new List<Line>();
            for (var row = 0; row < FieldSize; row++)
                for (var col = 0; col < FieldSize - 2; col++)
                {
                    var o = Items[col][row] as GameObject;
                    if (o != null && (Items[col][row] != null && Items[col][row] != DisabledItem && (!o.GetComponent<GameItemMovingScript>().IsMoving
                                                                                                     || o.GetComponent<GameItem>().IsDraggableWhileMoving)))//TODO: Attention
                    {
                        var match = CheckForLine(col, row, LineOrientation.Horizontal);
                        if (match <= 2) continue;
                        list.Add(new Line { X1 = col, Y1 = row, X2 = col + match - 1, Y2 = row, Orientation = LineOrientation.Horizontal });
                        col += match - 1;
                    }
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
                        if(includeMovingItemsInLine)
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
                        if(includeMovingItemsInLine)
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
                if (_raiseMaxInitialElement && CallbacksCount == 0)
                {
                    _raiseMaxInitialElement = false;
                    MaxInitialElementTypeRaisedActions();
                }
                ChainCounter = 0;
                if (HintTimeCounter < 0) HintTimeCounter = 0;
                
                if (!RemoveAdditionalItems() && CallbacksCount == 0 && !CheckForPossibleMoves())
                {
                    LogFile.Message("No moves", true);
                    GenerateField(false, true);
                }
                UpdateTime();
                SavedataHelper.SaveData(SavedataObject);
                return 0;
            }
            HintTimeCounter = -1;

            LogFile.Message("Start clear chaines. Lines: " + lines.Count, true);
            var linesCount = lines.Count;
            var pointsBank = 0;
            var l = lines.FirstOrDefault();
            while (l != null && !IsGameOver)
            {
                var toObj = Items[l.X2][l.Y2] as GameObject;
                var toCell = GetCellCoordinates(l.X2, l.Y2);
                var pointsMultiple = 1;
                if (l.Orientation == LineOrientation.Vertical)
                {
                    pointsMultiple += l.Y2 - l.Y1 - 2;

                    for (var j = l.Y2 - 1; j >= l.Y1; j--)
                    {
                        if (Items[l.X1][j] == null || Items[l.X1][j] == DisabledItem)
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
                        var c = gobj.GetComponent<GameItemMovingScript>();
                        //if (c.IsMoving) continue;
                        var cX = l.X1;
                        var cY = j;
                        if (c.GetComponent<GameItem>().IsTouched)
                            GetComponent<DragItemScript>().CancelDragging((s,e) =>
                                {
                                    CallbacksCount++;
                                    Items[cX][cY] = null;
                                    c.MoveTo(null, toCell.y, Game.standartItemSpeed, (item, result) =>
                                    {
                                        LogFile.Message(cX + " " + cY, true);
                                        CallbacksCount--;
                                        if (!result) return;

                                        Destroy(item);
                                    });
                                });
                        else
                        {
                            CallbacksCount++;
                            Items[cX][cY] = null;
                            c.MoveTo(null, toCell.y, Game.standartItemSpeed, (item, result) =>
                            {
                                LogFile.Message(cX + " " + cY, true);
                                CallbacksCount--;
                                if (!result) return;
                                Destroy(item);
                            });
                        }


                    }
                }
                else
                {
                    pointsMultiple += l.X2 - l.X1 - 2;

                    for (var i = l.X2 - 1; i >= l.X1; i--)
                    {
                        if (Items[i][l.Y1] == null || Items[i][l.Y1] == DisabledItem)
                        {
                            //LogFile.Message("Items[i][l.Y1] == null");
                            continue;
                        }
                        if (IsInAnotherLine(lines, i, l.Y1))
                        {
                            //LogFile.Message("Items[" + i + "][" + l.Y1 + "] on another line");
                            continue;
                        }
                        var gobj = Items[i][l.Y1] as GameObject;
                        if (gobj == null) continue;

                        gobj.transform.localPosition = new Vector3(gobj.transform.localPosition.x, gobj.transform.localPosition.y, -0.5f);
                        var c = gobj.GetComponent<GameItemMovingScript>();
                        if (c.IsMoving) continue;

                        var cX = i;
                        var cY = l.Y1;

                        if (c.GetComponent<GameItem>().IsTouched)
                            GetComponent<DragItemScript>().CancelDragging((s, e) =>
                                {
                                    CallbacksCount++;
                                    Items[cX][cY] = null;
                                    c.MoveTo(toCell.x, null, Game.standartItemSpeed, (item, result) =>
                                    {
                                        LogFile.Message(cX + " " + cY, true);
                                        CallbacksCount--;
                                        if (!result) return;
                                        Destroy(item);
                                    });
                                });
                        else
                        {
                            CallbacksCount++;
                            Items[cX][cY] = null;
                            c.MoveTo(toCell.x, null, Game.standartItemSpeed, (item, result) =>
                            {
                                LogFile.Message(cX + " " + cY, true);
                                CallbacksCount--;
                                if (!result) return;
                                Destroy(item);
                            });
                        }
                    }
                }
                if (toObj != null)
                {
                    var toGi = toObj.GetComponent<GameItem>();
                    if(toGi.Type == MaxInitialElementType + 1)
                    MaxInitialElementType++;
                    var newgobjtype = toGi.Type != GameItemType._2x ? toGi.Type + 1 : GameItemType._2x;
                    var newgobj = InstantiateGameItem(newgobjtype, toCell,
                                new Vector3(GameItemSize / ScaleMultiplyer, GameItemSize / ScaleMultiplyer, 0f));
                    if (toObj.GetComponent<GameItemMovingScript>().IsMoving)
                        CallbacksCount--;
                    Destroy(toObj);
                    Items[l.X2][l.Y2] = newgobj;
                    var points = pointsMultiple * (int)Math.Pow(2, (double)newgobjtype);
                        if (newgobjtype <= MaxInitialElementType)
                                {
                                    pointsBank += points;
                                    LabelShowing.ShowScalingLabel(newgobj,//new Vector3(newgobj.transform.localPosition.x, newgobj.transform.localPosition.y + GameItemSize / 2, -3),
                                        "+" + points, GameColors.ItemsColors[newgobjtype], Color.gray, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 3, Game.numbersFont, true, null, 0, newgobjtype);
                                }
                         else
                                {
                                    pointsBank += 2 * points;
                                    LabelShowing.ShowScalingLabel(newgobj,//new Vector3(newgobj.transform.localPosition.x, newgobj.transform.localPosition.y + GameItemSize / 2, -3),
                                        "+" + points + "x2", GameColors.ItemsColors[newgobjtype], Color.gray, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 3, Game.numbersFont, true, null, 0, newgobjtype);
                                }               
                }
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
            RemoveAdditionalItems();
            
            return linesCount;
        }

        public void GenerateGameOverMenu(bool isWinning = false)
        {

            SavedataHelper.RemoveData(SavedataObject);
            var fg = GameObject.Find("/Foreground");

            var pausebackground = Instantiate(Resources.Load("Prefabs/PauseBackground")) as GameObject;
            var gameOverLabelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
            if (pausebackground == null || gameOverLabelObject == null) return;
            if (fg != null)
            {
                pausebackground.transform.SetParent(fg.transform);
                gameOverLabelObject.transform.SetParent(fg.transform);
            }

            pausebackground.transform.localPosition = Vector3.zero;
            var rectTransform = pausebackground.transform as RectTransform;
            if (rectTransform != null)
                rectTransform.sizeDelta = Vector2.zero;

            var gameOverLabel = gameOverLabelObject.GetComponent<LabelShowing>();
            gameOverLabel.ShowScalingLabel(new Vector3(0, 10, -3),
                isWinning ? LanguageManager.Instance.GetTextValue("YouWinTitle") : LanguageManager.Instance.GetTextValue("GameOverTitle"), GameColors.DifficultyLevelsColors[Game.Difficulty], GameColors.DefaultDark, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 1, null, false, () =>
                {
                    var gameOverMenu = Instantiate(Resources.Load("Prefabs/GameOverMenu")) as GameObject;
                    if (gameOverMenu == null) return;
                    if (fg != null)
                    {
                        gameOverMenu.transform.SetParent(fg.transform);
                    }
                    
                    gameOverMenu.transform.localScale = Vector3.one;
                    gameOverMenu.transform.localPosition = new Vector3(0, -70, 0);
                });
            if(!isWinning)
            DeviceButtonsHelpers.OnSoundAction(Power2Sounds.GameOver, false, true);
            else
                DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Winning, false, true);
        }

        public virtual void Drop()
        {
            if (Items == null) return;

            var generateAfterDrop = false;
			//_lowestNullItem = 0;
            //if (DropsCount == 0) GenerateField(true);

            //var counterOfNotMovingItems = 0;
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
                    var fromrow = row-1;
                    while (fromrow >= 0)
                    {
                        var gobj = Items[col][fromrow] as GameObject;
                        if (Items[col][fromrow] == null || Items[col][row] == DisabledItem || (!AreStaticItemsDroppable && gobj.GetComponent<GameItem>().MovingType == GameItemMovingType.Static))
                            fromrow--;
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
                            gims.MoveTo(null, GetCellCoordinates(col, row).y, Game.standartItemSpeed, (item, result) =>
                            {
                                //DropsCount--;
                                CallbacksCount--;
                                if (!result) return;
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
                GenerateField(true);
            /*
            if (Items[col][row - 1] == null || Items[col][row - 1] == DisabledItem)
                continue;
            var o = Items[col][row - 1] as GameObject;
            if (o != null && (!AreStaticItemsDroppable && o.GetComponent<GameItem>().MovingType == GameItemMovingType.Static))
            {
                    var rowStaticCounter = -1;
                    GameObject o1;
                    while ((row + rowStaticCounter) >= 0 && (o1 = Items[col][row + rowStaticCounter] as GameObject) != null &&
                        o1.GetComponent<GameItem>().MovingType == GameItemMovingType.Static)
                        rowStaticCounter--;
                    var newRow = row + rowStaticCounter;
                    if ((newRow) < 0 || Items[col][newRow] == null)
                        continue;
                    var gobjS = Items[col][newRow] as GameObject;
                    if (gobjS == null) continue;
                    var cS = gobjS.GetComponent<GameItemMovingScript>();
                    if (cS.IsMoving) continue;
                    counterOfNotMovingItems++;
                    Items[col][row] = gobjS;
                    Items[col][newRow] = null;
                    //if (newRow > _lowestNullItem)
                    //    _lowestNullItem = newRow;
                    //if (!cS.IsMoving)
                    DropsCount++;
                    var colS = col;
                    var rowS = row;
                    cS.MoveTo(null, GetCellCoordinates(col, row).y, Game.standartItemSpeed, (item, result) =>
                    {
                        if (!cS.IsMoving)
                            DropsCount--;
                        if (!result) return;
                        LogFile.Message("New item droped Items[" + colS + "][" + rowS + "] DC: " + DropsCount, true);
                    });
                if (row - 2 >= 0 && Items[col][row - 2] != null) generateAfterDrop = false;
                continue;
            }

            //var gobj = Items[col][row] as GameObject;
            if (o == null) continue;
            var c = o.GetComponent<GameItemMovingScript>();
            //var dis = GetComponent<DragItemScript>();
            if (c.IsMoving || o.GetComponent<GameItem>().IsTouched)//(dis.IsDragging && dis.TouchedItem.X == col && dis.TouchedItem.Y == row - 1)) 
                continue;
            counterOfNotMovingItems++;
            var newRow1 = row - 1;
            //if (newRow1 > _lowestNullItem)
            //    _lowestNullItem = newRow1;
            Items[col][row] = Items[col][newRow1];
            Items[col][row-1] = null;
            if (newRow1 - 1 >= 0 && Items[col][newRow1 - 1] != null) generateAfterDrop = false;//DropsCount++;
            if (!c.IsMoving) DropsCount++;
            var col1 = col;
            var row1 = newRow1;
            c.MoveTo(null, GetCellCoordinates(col, row).y, Game.standartItemSpeed, (item, result) =>
            {
                //if (!c.IsMoving)
                    DropsCount--;
                if (!result) return;
                LogFile.Message("New item droped Items[" + col1 + "][" + row1 + "] DC: " + DropsCount, true);

                //GenerateField(true);
            });
        }
    }
    //LogFile.Message("counter " + counter + " DropsCount " + DropsCount);

    if (counterOfNotMovingItems == 0 && DropsCount == 0 && generateAfterDrop)
        GenerateField(true);      
             */
        }


        public virtual void GenerateField(bool completeCurrent = false, bool mixCurrent = false, bool onlyNoMovesLabel = false)
        {
            if (!mixCurrent)
            {
                //if(!completeCurrent)
                //    _lowestNullItem = FieldSize - 1;
                for (var i = FieldSize - 1; i >= 0; i--)
                {
                    var generateOnY = 1;
                    var resCol = 0;
                    //if (Game.Difficulty > DifficultyLevel._easy && DropDownItemsCount < MaxAdditionalItemsCount)
                    for (var j = FieldSize - 1; j >= 0; j--)
                    {
                        //var itemsJ = FieldSize - 1 - j;
                        if (Items[i][j] != null || Items[i][j] == DisabledItem)
                            continue;
                        var deniedList = new List<GameItemType>();
                        if (completeCurrent)
                        {
                            switch (Game.Difficulty)
                            {
                                case DifficultyLevel._medium:
                                case DifficultyLevel._hard:
                                //case DifficultyLevel._veryhard:
                                    if (DropDownItemsCount < MaxAdditionalItemsCount && j <= FieldSize / 2)
                                    {
                                        resCol = RandomObject.Next(0, FieldSize);//it was in first cycle
                                        var resRow = RandomObject.Next(resCol, FieldSize);
                                        if (resCol == resRow)
                                        {
                                            Items[i][j] = GenerateGameItem(GameItemType._DropDownItem, i, j, new Vector2(0, generateOnY));
                                            DropDownItemsCount++;
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
            }
            else
            {
                LogFile.Message("Mix field...", true);
                    var o = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
                    if (o != null)
                    {
                        var noMovesLabel = o.GetComponent<LabelShowing>();
                        LabelAnimationFinishedDelegate callback = null;
                        if (!onlyNoMovesLabel)
                            callback = MixField;
                        noMovesLabel.ShowScalingLabel(new Vector3(0, -2, -4),
                            LanguageManager.Instance.GetTextValue("NoMovesTitle"), GameColors.DifficultyLevelsColors[Game.Difficulty], GameColors.DefaultDark, LabelShowing.minLabelFontSize, LabelShowing.maxLabelFontSize, 2, null, true, callback, true);
                        //noMovesLabel.ShowScalingLabel(new Vector3(0, Item00.Y + GameItemSize * 2.5f, -4), 
                        //    "No moves", new Color(240, 223, 206), new Color(240, 223, 206), 60, 90, null, true, null, true);
                    }
                
            }
            //if (!isNoLines && _callbackReady.WaitOne(1))
            // ClearChains();
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
                        if (go == null || go.GetComponent<GameItem>().MovingType == GameItemMovingType.Static) continue;
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
                        if (go == null || go.GetComponent<GameItem>().MovingType == GameItemMovingType.Static) continue;

                        var index = RandomObject.Next(0, toMixList.Count);
                        Items[i][j] = toMixList[index];
                        toMixList.RemoveAt(index);
                    }
                }
            }
            var mixSpeed = Game.standartItemSpeed / 3;
            for (var i = FieldSize - 1; i >= 0; i--)
            {
                for (var j = FieldSize - 1; j >= 0; j--)
                {
                    if (Items[i][j] == null || Items[i][j] == DisabledItem)
                        continue;
                    var gameObject1 = Items[i][j] as GameObject;
                    if (gameObject1 == null || gameObject1.GetComponent<GameItem>().MovingType == GameItemMovingType.Static) continue;

                    var moving = gameObject1.GetComponent<GameItemMovingScript>();
                    var toCell = GetCellCoordinates(i, j);
                    CallbacksCount++;
                    moving.MoveTo(toCell.x, toCell.y, mixSpeed, (item, result) =>
                    {
                        CallbacksCount--;
                        if (!result) return;
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

            if (item1 != null)
            {
                //item1.GetComponent<GameItem>().IsTouched = false;
                CallbacksCount++;
                _currentExchangeItemsCount++;
                item1.GetComponent<GameItemMovingScript>()
                    .MoveTo(position2.x,
                       position2.y,
                        Game.standartItemSpeed, (item, result) =>
                        {
                            CallbacksCount--;
                            if (!result) return;
                            var currentItem = item as GameObject;
                            if (currentItem != null && isReverse)
                            {
                                CallbacksCount++;
                                float? mtoX1 = position1.x;
                                currentItem.GetComponent<GameItemMovingScript>()
                                    .MoveTo(position1.x,
                                        position1.y,
                                        Game.standartItemSpeed, (reverseItem, reverseResult) =>
                                        {
                                            CallbacksCount--;
                                            _currentExchangeItemsCount--;
                                            if (exchangeCallback != null)
                                                exchangeCallback(gameObject, true);
                                            if (CallbacksCount == 0 && _callClearChainsAfterExchange)
                                            {
                                                _callClearChainsAfterExchange = false;
                                                ClearChains();
                                            }
                                        });
                                return;
                            }
                            _currentExchangeItemsCount--;
                            if (exchangeCallback != null)
                                exchangeCallback(gameObject, true);
                            if (CallbacksCount == 0)
                            {
                                _callClearChainsAfterExchange = false;
                                ClearChains();
                            }

                        });
            }

            //if (item2 != null)
            //{
                //item2.GetComponent<GameItem>().IsTouched = false;
                CallbacksCount++;
                _currentExchangeItemsCount++;
                float? mtoX = position1.x;
                float? mtoY = position1.y;
                item2.GetComponent<GameItemMovingScript>()
                    .MoveTo(position1.x,
                        position1.y,
                        Game.standartItemSpeed, (item, result) =>
                        {
                            CallbacksCount--;
                            if (!result) return;
                            var currentItem = item as GameObject;
                            if (currentItem != null && isReverse)
                            {
                                CallbacksCount++;
                                currentItem.GetComponent<GameItemMovingScript>()
                                    .MoveTo(position2.x,
                                        position2.y,
                                        Game.standartItemSpeed, (reverseItem, reverseResult) =>
                                        {
                                            CallbacksCount--;
                                            _currentExchangeItemsCount--;
                                            if (exchangeCallback != null)
                                                exchangeCallback(gameObject, true);
                                            if (CallbacksCount == 0 && _callClearChainsAfterExchange)
                                            {
                                                _callClearChainsAfterExchange = false;
                                                ClearChains();
                                            }
                                        });
                                return;
                            }
                            _currentExchangeItemsCount--;
                            if (exchangeCallback != null)
                                exchangeCallback(gameObject, true);
                            if (CallbacksCount == 0)
                            {
                                _callClearChainsAfterExchange = false;
                                ClearChains();
                            }

                        });
            //}
            return true;
        }

        public virtual bool IsItemMovingAvailable(int col, int row, MoveDirections mdir)
        {
            if (!AvailableMoveDirections.ContainsKey(mdir)) return false;
            if (col < 0 || row < 0 || col >=  FieldSize || row >= Items[col].Length)
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
            if ((Items[x1][y1] as GameObject).GetComponent<GameItem>().MovingType == GameItemMovingType.StandartExchangable)
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
                    if (Items[col][row] == null || Items[col][row] == DisabledItem)
                    {
                        //if (Items[col][row] == null) Debug.LogError("Items[col][row] null in checkForPossibleMoves");
                        continue;
                    }
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
            var labelObject = Instantiate(Resources.Load("Prefabs/Label")) as GameObject;
            if (labelObject == null) return;
            var comboLabel = labelObject.GetComponent<LabelShowing>();
            comboLabel.name = "ComboLabel";
            comboLabel.transform.SetParent(transform);

            //comboLabel.transform.RotateAround(Vector3.zero, Vector3.forward, count % 2 == 0 ? 30 : -30);
            comboLabel.ShowScalingLabel(new Vector3(count%2 == 0 ? -10 : 10, Item00.Y + GameItemSize*2.5f, -1),
                LanguageManager.Instance.GetTextValue("ComboTitle") + count, GameColors.DifficultyLevelsColors[Game.Difficulty], GameColors.DefaultDark, LabelShowing.minLabelFontSize / 3, LabelShowing.minLabelFontSize, 2, null, true, null, false,
                count % 2 == 0 ? 30 : -30);
            DeviceButtonsHelpers.OnSoundAction(Power2Sounds.Combo, false);
        }

        public virtual void RevertMovedItem(int col, int row, MovingFinishedDelegate callback = null)
        {
            var gobj = Items[col][row] as GameObject;
            if (gobj == null) return;
            var toCell = GetCellCoordinates(col, row);
            LogFile.Message("Revert item to place: " + toCell.x + " " + toCell.y, true);
            var gims = gobj.GetComponent<GameItemMovingScript>();
            CallbacksCount++;
            gims.MoveTo(toCell.x, toCell.y, Game.standartItemSpeed, (s,e)=>
                {
                    CallbacksCount--;
                    if (callback != null)
                        callback(s,e);
                });
        }

        public virtual void ResetPlayground()
        {
            for (var i = 0; i < FieldSize; i++)
                for (var j = 0; j < FieldSize; j++)
                {
                    var item = Items[i][j] as GameObject;
                    if (item != null)
                    {
                        var gims = item.GetComponent<GameItemMovingScript>();
                        if (gims != null) gims.CancelMoving(true);
                    }
                    Items[i][j] = null;
                    Destroy(item);
                }
            Items = null;
            MaxType = GameItemType.NullItem;
            CurrentScore = 0;
        }

        public void RemoveGameItem(int i, int j, MovingFinishedDelegate removingCallback = null)
        {
            var o = Items[i][j] as GameObject;
            if(o.GetComponent<GameItem>().IsTouched)
            {
                GetComponent<DragItemScript>().CancelDragging((s, e) => RemoveGameItem(i, j));
                return;
            }
            
            if (o == null) return;
            var giss = o.GetComponent<GameItemScalingScript>();
            var toSize = GameItemSize / ScaleMultiplyer / 4;
            CallbacksCount++;
            Items[i][j] = null;

            giss.ScaleTo(new Vector3(toSize, toSize, 0), 8, (item, r) =>
            {
                Destroy(item);
                if (removingCallback != null)
                    removingCallback(item, r);
                CallbacksCount--;
            });
        }

        protected virtual void CreateInGameHelpModule(string modulePostfix, LabelAnimationFinishedDelegate callback = null)
        {
            //check if needed
            if (PlayerPrefs.HasKey(modulePostfix))
            {
                if (callback != null)
                    callback();
                return;
            }
            var fg = GameObject.Find("/Foreground");
            if (fg == null) return;
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
            UserHelpScript.InGameHelpModule.transform.SetParent(fg.transform);
            UserHelpScript.InGameHelpModule.transform.localScale = Vector3.one;
            UserHelpScript.InGameHelpModule.transform.localPosition = new Vector3(0, 0, -2);
            manual.transform.SetParent(UserHelpScript.InGameHelpModule.transform);
            manual.transform.localScale = new Vector3(45, 45, 0);
            manual.transform.localPosition = new Vector3(0, 30, 0);
            UserHelpScript.ShowUserHelpCallback = callback;
            PlayerPrefs.SetInt(modulePostfix, 1);
        }
        
    }
}