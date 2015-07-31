using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.DataClasses
{
    public class PlaygroundProgressBar : MonoBehaviour
    {
        private GameObject _progressBar;
        private GameObject _progressBarLine;
        private float _moveTimerMultiple = 10;


        public event EventHandler ProgressBarOver;

        void Awake()
        {
            var fg = GameObject.Find("/Foreground");
            _progressBar = Instantiate(Resources.Load("Prefabs/ProgressBar")) as GameObject;
            _progressBar.transform.SetParent(fg.transform);

            _progressBarLine = GameObject.Find("ProgressBarLine");
        }

        void Update()
        {
            var rtrans = _progressBarLine.transform as RectTransform;
            rtrans.sizeDelta = new Vector2(rtrans.sizeDelta.x - _moveTimerMultiple * Time.deltaTime, rtrans.sizeDelta.y);
        }
    }
}
