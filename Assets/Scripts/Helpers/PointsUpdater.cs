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

            if (!raise) return;
            raise = false;

            plbelText.text = CurrentScore.ToString(CultureInfo.InvariantCulture);
            plbelText.fontSize = 47;
        }

        public void RisePoints(int currentScore)
        {
            raise = true;
            _currentScore = currentScore;
        }     
    }
}
