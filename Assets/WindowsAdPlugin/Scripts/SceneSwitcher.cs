using UnityEngine;
using System.Collections;

/// <summary>
/// Script that switches scenes, used to test the DestroyAd script
/// </summary>
public class SceneSwitcher : MonoBehaviour {
    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width * 0.01f,Screen.height * 0.01f,100.0f,100.0f),"Scene: " + Application.loadedLevel);

        if(GUI.Button(new Rect(Screen.width * 0.01f,Screen.height * 0.05f,100.0f,25.0f),"Change Level"))
        {
            int levelToLoad = Application.loadedLevel + 1;
            if (levelToLoad > Application.levelCount - 1)
                levelToLoad = 0;

            Application.LoadLevel(levelToLoad);
        }
    }
}
