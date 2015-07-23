using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class CoinsHelper : MonoBehaviour
    {
        private static CoinsHelper _instance;

        public static CoinsHelper Instance
        {
            get { return _instance; }
        }

        public int CoinsCount { get; set; }
        public int CoinPrice { get; set; }

        void Awake()
        {
            if (_instance != null)
                Debug.LogError("Multiple instances of ScoreHelper!");

            _instance = this;
        }

    }
}
