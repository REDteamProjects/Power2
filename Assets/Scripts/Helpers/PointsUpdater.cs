using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Helpers
{
    public class PointsUpdater : MonoBehaviour
    {
        private float _moveTimerMultiple;
        private float _pscoreBankUpper;
        private int _currentScore;

        public float Multiplier { get { return _moveTimerMultiple; } }
        public float Upper { get { return _pscoreBankUpper; } }
        public int CurrentScore { get { return _currentScore; } }
        
        void Update()
        {
            //TODO: update score object text
            var plabel = GameObject.Find("Points");
            var plbelText = plabel.GetComponent<Text>();
            plbelText.text = CurrentScore.ToString(CultureInfo.InvariantCulture);
        }

        protected void RisePoints(int points)
        {
            _pscoreBankUpper += points;
        }     
    }
}
