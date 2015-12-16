using SmartLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class UserHelpScript : MonoBehaviour
    {
        public static GameObject InGameHelpModule;
        public static LabelAnimationFinishedDelegate ShowUserHelpCallback = null;

        public void CloseInGameHelpModule()
        {
            Time.timeScale = 1F;
            Destroy(InGameHelpModule);
            PauseButtonScript.PauseMenuActive = false;
            if (ShowUserHelpCallback != null)
            {
                var callback = ShowUserHelpCallback;
                ShowUserHelpCallback = null;
                callback();
            }
        }
    }
}
