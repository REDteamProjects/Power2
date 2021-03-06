﻿using System;
using System.Linq;
using Assets.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.DataClasses;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.Helpers
{
    public class PlaygroundProgressBar : MonoBehaviour
    {
        public static readonly float ProgressBarBaseSize = 460;
        private static GameObject _progressBar;
        private GameObject _progressBarLine;
        private float _moveTimerMultiple = 34;
        private float _moveTimerMultipleUpper;
        private float _progressBarBank = -1;
        private float _progressBarBankUpper;
        private float _maxBarYSize = 0;
        private float _barYSize = 0;
        private float _deltaBarYSize = 0;
        private static  float _timeActionBorder = 160;
        private readonly float _timeActionBorderMaximumSize = ProgressBarBaseSize / 2;
        private event EventHandler _timeBorderActivated;
        private event EventHandler _timeBorderDeActivated;
        private GameObject LeftSmallX = null;
        private GameObject RightSmallX = null;

        public readonly Vector3 Coordinate = new Vector3(0, 190, 0);
        public static bool ProgressBarRun;

        public event EventHandler ProgressBarOver;

        public event EventHandler TimeBorderActivated
        {
            add 
            {
                if(_timeBorderActivated==null || !_timeBorderActivated.GetInvocationList().Contains(value))
                _timeBorderActivated += value;
                if (LeftSmallX == null)
                {
                    LeftSmallX = Instantiate(Resources.Load("Prefabs/ProgressBarSmallX")) as GameObject;
                    LeftSmallX.transform.SetParent(_progressBar.transform);
                    LeftSmallX.transform.localPosition = new Vector3(-_timeActionBorder/2, 0, -4);
                    LeftSmallX.transform.localScale = Vector3.one;
                }
                if (RightSmallX == null)
                {
                    RightSmallX = Instantiate(Resources.Load("Prefabs/ProgressBarSmallX")) as GameObject;
                    RightSmallX.transform.SetParent(_progressBar.transform);
                    RightSmallX.transform.localPosition = new Vector3(_timeActionBorder/2, 0, -4);
                    RightSmallX.transform.localScale = Vector3.one;
                }
            }
            remove 
            {
                _timeBorderActivated -= value;
                if(_timeBorderActivated == null)
                {
                    _timeBorderDeActivated = null;
                    if (LeftSmallX != null)
                    {
                        Destroy(LeftSmallX);
                        LeftSmallX = null;
                    }
                    if (RightSmallX != null)
                    {
                        Destroy(RightSmallX);
                        RightSmallX = null;
                    }
                }
            }
        }

        public event EventHandler TimeBorderDeActivated
        {
            add
            {
                if (_timeBorderDeActivated == null || !_timeBorderDeActivated.GetInvocationList().Contains(value))
                    _timeBorderDeActivated += value;
            }
            remove
            {
                _timeBorderDeActivated -= value;
            }
        }


        public bool Exists { get { return _progressBar != null; } }
        public float Multiplier { get { return _moveTimerMultiple; } }
        public float State { get { return _progressBarBank; } }
        public float Upper { get { return _progressBarBankUpper; } }
        public double CriticalCount { get { return 100; } }

        public float MoveTimerMultiple {
            get { return _moveTimerMultiple; }
            set {
                if (value < 1)
                    _moveTimerMultiple = 1;
                else
                _moveTimerMultiple = value;
                _moveTimerMultipleUpper = _moveTimerMultiple * 3f;
            } 
        }

        public float TimeActionBorder
        {
            get { return _timeActionBorder; }
            private set
            {
                if (value <= _timeActionBorderMaximumSize)
                {
                    _timeActionBorder = value;
                    if (LeftSmallX != null)
                        LeftSmallX.transform.localPosition = new Vector3(-_timeActionBorder / 2, 0, -4);
                    if (RightSmallX != null)
                        RightSmallX.transform.localPosition = new Vector3(_timeActionBorder / 2, 0, -4);
                }
                else
                    _timeActionBorder = _timeActionBorderMaximumSize;
            }
        }

        void Awake()
        {
            var pg = gameObject.GetComponent<IPlayground>();
            _moveTimerMultiple = pg.MoveTimerMultiple;
            _moveTimerMultipleUpper = _moveTimerMultiple * 3f;
            //ProgressBarRun = true;
        }

        void Update()
        {
            if (_progressBarBank == 0 || !ProgressBarRun || _progressBarLine == null) return;

            var rtrans = _progressBarLine.transform as RectTransform;

            
            if (_progressBarBankUpper > 0)
            {
                var deltaXUpper = (_moveTimerMultipleUpper - _moveTimerMultiple) * Time.deltaTime;
                if (_progressBarBank + deltaXUpper >= ProgressBarBaseSize)
                {
                    _progressBarBank = ProgressBarBaseSize;
                    _progressBarBankUpper = -1;
                }
                else
                {
                    var upperDelta = deltaXUpper > _progressBarBankUpper ? _progressBarBankUpper : deltaXUpper;
                    var progressBarBankValue = _progressBarBank;
                    _progressBarBank += upperDelta;
                    if (_timeBorderDeActivated != null && _progressBarBank > TimeActionBorder && TimeActionBorder > progressBarBankValue)
                    {
                        _timeBorderDeActivated(gameObject, EventArgs.Empty);
                    }
                    _progressBarBankUpper -= upperDelta;
                }
            }
            else
            {
                var deltaX = _moveTimerMultiple * Time.deltaTime;
                var progressBarBankValue = _progressBarBank;
                _progressBarBank = _progressBarBank - deltaX < 0 ? 0 : _progressBarBank - deltaX;
                if (_timeBorderActivated != null && _progressBarBank < TimeActionBorder && TimeActionBorder < progressBarBankValue)
                {
                    _timeBorderActivated(gameObject, EventArgs.Empty);
                    TimeActionBorder += 4;
                }
            }

            
            var audio = _progressBar.GetComponent<AudioSource>();
            if (_progressBarBank < CriticalCount)
                {
                    if (!audio.isPlaying && !PauseButtonScript.PauseMenuActive && GeneralSettings.SoundEnabled == SoundState.on)
                        audio.Play();
                    else
                        if (PauseButtonScript.PauseMenuActive || _progressBarBank == 0)
                            audio.Stop();
                    rtrans.sizeDelta = new Vector2(_progressBarBank, rtrans.sizeDelta.y - _deltaBarYSize);
                    if (rtrans.sizeDelta.y == _maxBarYSize || rtrans.sizeDelta.y == _barYSize)
                    _deltaBarYSize *= -1;
                }
                else
                {
                    if (audio.isPlaying)
                        audio.Stop();
                    if (rtrans.sizeDelta.y != _barYSize)
                    {
                            _deltaBarYSize = -Math.Abs(_deltaBarYSize);
                            rtrans.sizeDelta = new Vector2(_progressBarBank, _barYSize);
                    }
                    else
                        rtrans.sizeDelta = new Vector2(_progressBarBank, rtrans.sizeDelta.y);
                }
                if (_progressBarBank != 0) return;
                //audio.Stop();
            
            var eventHandler = ProgressBarOver;
            if (eventHandler != null)
                eventHandler(gameObject, EventArgs.Empty);

            ProgressBarRun = false;


            //rtrans.position = new Vector3(rtrans.position.x - _moveTimerMultiple * Time.deltaTime, rtrans.position.y, rtrans.position.z);
        }
 
        public void AddTime(float count)
        {
            _progressBarBankUpper += count * _moveTimerMultiple;
            /*if (_progressBarBank + _progressBarBankUpper > ProgressBarBaseSize)
                _moveTimerMultiple *= ProgressBarBaseSize/(_progressBarBank + _progressBarBankUpper);*/ // возможно, здесь причина затормаживания счётчика времени

        }

        public void CreateBar()
        {
            if (_progressBar == null)
            {
                _progressBar = Instantiate(Resources.Load("Prefabs/ProgressBar")) as GameObject;
                _progressBar.transform.SetParent(Game.Foreground.transform);
                _progressBar.transform.localPosition = Coordinate;
                _progressBar.transform.localScale = Vector3.one;
            }
                _progressBarLine = GameObject.Find("ProgressBarLine");
            var rtrans = _progressBarLine.transform as RectTransform;
            _barYSize = rtrans.sizeDelta.y;
            var deltabYS = _barYSize / 4;
            _maxBarYSize = _barYSize + deltabYS;
            _deltaBarYSize = -deltabYS / 16;

            if (_progressBarBank < 0)
                _progressBarBank = ProgressBarBaseSize;
            else
            {
                rtrans.sizeDelta = new Vector2(_progressBarBank, rtrans.sizeDelta.y);
            }
        }

        public void InnitializeBar(float count, float upper, float timeMultiple)
        {
            ProgressBarRun = false;
            _progressBarBank = count >= ProgressBarBaseSize ? ProgressBarBaseSize : count;
            _progressBarBankUpper = upper;
            _moveTimerMultiple = timeMultiple;
            if (!Exists) return;
            var rtrans = _progressBarLine.transform as RectTransform;
            rtrans.sizeDelta = new Vector2(_progressBarBank, rtrans.sizeDelta.y);
        }

        public void UpdateTexture()
        {
            if (_progressBarLine == null) return;
            ProgressBarRun = false;
            _progressBarLine.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("GradientAtlas")
               .SingleOrDefault(t => t.name.Contains(Game.Difficulty.ToString()));
        }

        public void SetSmallXsColor(Color color)
        {
            var material = Resources.Load("Fonts/SEGUIBL", typeof(Material)) as Material;
            if (LeftSmallX != null)
            {
                var img = LeftSmallX.GetComponent<Image>();
                img.material = material;
                img.color = color;
            }
            if (RightSmallX != null)
            {
                var img = RightSmallX.GetComponent<Image>();
                img.material = material;
                img.color = color;
            }
        }
    }
}
