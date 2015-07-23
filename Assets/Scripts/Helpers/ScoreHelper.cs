using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class ScoreHelper : MonoBehaviour
    {

        private static ScoreHelper _instance;

        public static ScoreHelper Instance
        {
            get { return _instance; }
        }

        void Awake()
        {
            if (_instance != null)
                Debug.LogError("Multiple instances of ScoreHelper!");

            _instance = this;
        }

    }
}
