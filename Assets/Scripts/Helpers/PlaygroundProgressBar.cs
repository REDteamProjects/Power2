using System;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class PlaygroundProgressBar : MonoBehaviour
    {
        private static float ProgressBarBaseSize = 460;
        private GameObject _progressBar;
        private GameObject _progressBarLine;
        private float _moveTimerMultiple = 10;
        private float _moveTimerMultipleUpper = 30;
        private float _progressBarBank = -1;
        private float _progressBarBankUpper;

        public bool ProgressBarRun;

        public static event EventHandler ProgressBarOver;
        
        public float Multiplier { get { return _moveTimerMultiple; } }
        public float State { get { return _progressBarBank; } }
        public float Upper { get { return _progressBarBankUpper; } }

        void Awake()
        {
            var fg = GameObject.Find("/Foreground");
            _progressBar = Instantiate(Resources.Load("Prefabs/ProgressBar")) as GameObject;
            _progressBar.transform.SetParent(fg.transform);
            _progressBar.transform.localPosition = new Vector3(0,215,0);
            _progressBar.transform.localScale = Vector3.one;
            _progressBarLine = GameObject.Find("ProgressBarLine");

            if (_progressBarBank < 0)
                _progressBarBank = ProgressBarBaseSize;
            else
            {
                var rtrans = _progressBarLine.transform as RectTransform;
                rtrans.sizeDelta = new Vector2(_progressBarBank, rtrans.sizeDelta.y);
            }

            ProgressBarRun = true;
        }

        void Update()
        {
            if (_progressBarBank < 0 || !ProgressBarRun) return;

            var rtrans = _progressBarLine.transform as RectTransform;

            var deltaX = _moveTimerMultiple * Time.deltaTime;
            _progressBarBank = _progressBarBank - deltaX < 0 ? 0 : _progressBarBank - deltaX;
            if (_progressBarBankUpper > 0)
            {
                var deltaXUpper = _moveTimerMultipleUpper * Time.deltaTime;
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

            rtrans.sizeDelta = new Vector2(_progressBarBank, rtrans.sizeDelta.y);
            if (!(_progressBarBank <= 0)) return;
            var eventHandler = ProgressBarOver;
            if (eventHandler != null)
                eventHandler(gameObject, EventArgs.Empty);

            ProgressBarRun = false;


            //rtrans.position = new Vector3(rtrans.position.x - _moveTimerMultiple * Time.deltaTime, rtrans.position.y, rtrans.position.z);
        }

        public void AddTime(float count)
        {
            _progressBarBankUpper += count * _moveTimerMultiple;
            if (_progressBarBank + _progressBarBankUpper > ProgressBarBaseSize)
                _moveTimerMultiple *= ProgressBarBaseSize/(_progressBarBank + _progressBarBankUpper);
        }

        public void InnitializeBar(float count, float upper, float timeMultiple)
        {
            ProgressBarRun = false;
            _progressBarBank = count >= ProgressBarBaseSize ? ProgressBarBaseSize : count;
            _progressBarBankUpper = upper;
            _moveTimerMultiple = timeMultiple;
            ProgressBarRun = true;
        }
    }
}
