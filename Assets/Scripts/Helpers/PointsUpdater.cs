using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Helpers
{
    public class PointsUpdater : MonoBehaviour
    {

        private int _pscoreBankUpper;
        private int _currentScore;
        private GameObject _plabel;

        public int Upper { get { return _pscoreBankUpper; } }
        public int CurrentScore { get { return _currentScore; } }

        void Awake()
        {
            _plabel = GameObject.Find("Points");
        }


        void Update()
        {
            var plbelText = _plabel.GetComponent<Text>();
            if (plbelText.fontSize != 40)
                plbelText.fontSize--;

            if (Upper == 0) return;
            
            var s = Upper;
            _currentScore += s;
            _pscoreBankUpper -= s;

            plbelText.text = CurrentScore.ToString(CultureInfo.InvariantCulture);
            plbelText.fontSize = 60;
        }

        public void RisePoints(int points)
        {
            _pscoreBankUpper += points;
        }     
    }
}
