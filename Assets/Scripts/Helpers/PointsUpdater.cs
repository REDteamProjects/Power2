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
        private bool raise = false;
        private int stockFont;
        public int CurrentScore { get { return _currentScore; } }

        void Awake()
        {
            _plabel = GameObject.Find("Points");
            stockFont = _plabel.GetComponent<Text>().fontSize;
        }


        void Update()
        {
            var plbelText = _plabel.GetComponent<Text>();
            if (plbelText.fontSize != stockFont)
                plbelText.fontSize--;

            if (!raise) return;
            raise = false;

            plbelText.text = CurrentScore.ToString(CultureInfo.InvariantCulture);
            plbelText.fontSize = stockFont + 10;
        }

        public void RisePoints(int currentScore)
        {
            raise = true;
            _currentScore = currentScore;
        }     
    }
}
