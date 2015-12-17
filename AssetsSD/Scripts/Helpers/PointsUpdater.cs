using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Helpers
{
    public class PointsUpdater : MonoBehaviour
    {

        private int _pscoreBankUpper;
        private int _currentScore;
        private Text _plabelText;
        private Text _plabelBackText;
        private bool raise = false;
        private int stockFont;
        public int CurrentScore { get { return _currentScore; } }

        void Awake()
        {
            _plabelText = GameObject.Find("Points").GetComponent<Text>();
            _plabelBackText = GameObject.Find("PointsBack").GetComponent<Text>();
            stockFont = _plabelText.GetComponent<Text>().fontSize;
        }


        void Update()
        {
            if (_plabelText.fontSize != stockFont)
            {
                _plabelText.fontSize--;
                _plabelBackText.fontSize--;
            }

            if (!raise) return;
            raise = false;

            _plabelText.text = CurrentScore.ToString(CultureInfo.InvariantCulture);
            _plabelText.fontSize = stockFont + 10;
            _plabelBackText.text = CurrentScore.ToString(CultureInfo.InvariantCulture);
            _plabelBackText.fontSize = stockFont + 10;
        }

        public void RisePoints(int currentScore)
        {
            raise = true;
            _currentScore = currentScore;
        }     
    }
}
