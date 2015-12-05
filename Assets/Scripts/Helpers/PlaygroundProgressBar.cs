using System;
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
        private static readonly float ProgressBarBaseSize = 460;
        private GameObject _progressBar;
        private GameObject _progressBarLine;
        private float _moveTimerMultiple = 16;
        private float _moveTimerMultipleUpper = 32;
        private float _progressBarBank = -1;
        private float _progressBarBankUpper;
        private float _maxBarYSize = 0;
        private float _barYSize = 0;
        private float _deltaBarYSize = 0;

        public readonly Vector3 Coordinate = new Vector3(0, 190, 0);
        public bool ProgressBarRun;

        public static event EventHandler ProgressBarOver;
        
        public float Multiplier { get { return _moveTimerMultiple; } }
        public float State { get { return _progressBarBank; } }
        public float Upper { get { return _progressBarBankUpper; } }
        public double CriticalCount { get { return 100; } }

        public float MoveTimerMultiple { set { _moveTimerMultiple = value; } }

        void Awake()
        {
            var fg = GameObject.Find("/Foreground");
            _progressBar = Instantiate(Resources.Load("Prefabs/ProgressBar")) as GameObject;
            _progressBar.transform.SetParent(fg.transform);
            _progressBar.transform.localPosition = Coordinate;
            _progressBar.transform.localScale = Vector3.one;
            _progressBarLine = GameObject.Find("ProgressBarLine");
            var rtrans = _progressBarLine.transform as RectTransform;
            _barYSize = rtrans.sizeDelta.y;
            var deltabYS = _barYSize / 4;
            _maxBarYSize = _barYSize + deltabYS;
            _deltaBarYSize = - deltabYS / 16;

            if (_progressBarBank < 0)
                _progressBarBank = ProgressBarBaseSize;
            else
            {
                rtrans.sizeDelta = new Vector2(_progressBarBank, rtrans.sizeDelta.y);
            }
            var pg = gameObject.GetComponent<IPlayground>();
            _moveTimerMultiple = pg.MoveTimerMultiple;
            _moveTimerMultipleUpper = _moveTimerMultiple * 3f;
            //ProgressBarRun = true;
        }

        void Update()
        {
            if (_progressBarBank == 0 || !ProgressBarRun) return;

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
                    _progressBarBank += upperDelta;
                    _progressBarBankUpper -= upperDelta;
                }
            }
            else
            {
                var deltaX = _moveTimerMultiple * Time.deltaTime;
                _progressBarBank = _progressBarBank - deltaX < 0 ? 0 : _progressBarBank - deltaX;
            }

            
            var audio = _progressBar.GetComponent<AudioSource>();
            if (_progressBarBank < CriticalCount)
                {
                    if (!audio.isPlaying && !PauseButtonScript.PauseMenuActive && GeneralSettings.SoundEnabled)
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

        public void InnitializeBar(float count, float upper, float timeMultiple)
        {
            ProgressBarRun = false;
            _progressBarBank = count >= ProgressBarBaseSize ? ProgressBarBaseSize : count;
            _progressBarBankUpper = upper;
            _moveTimerMultiple = timeMultiple;
        }

        public void UpdateTexture()
        {
            ProgressBarRun = false;
            _progressBarLine.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("SD/GradientAtlas")
               .SingleOrDefault(t => t.name.Contains(Game.Difficulty.ToString()));
            //ProgressBarRun = true;
        }
    }
}
