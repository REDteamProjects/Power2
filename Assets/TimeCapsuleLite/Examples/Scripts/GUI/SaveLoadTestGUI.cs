using UnityEngine;
using System.Collections;

public class SaveLoadTestGUI : MonoBehaviour {

	public static string directory;

	// Use this for initialization
	void Start () {
		directory = Application.dataPath + "/TimeCapsuleLite/Examples/Saves/Test/" + Application.loadedLevelName;
	}
	
	void OnGUI () {

		GUI.Box(new Rect(10, 10, 600, 130), "Save/Load Menu");
	
		if(GUI.Button(new Rect(20,40,80,20), "Save")) {

			foreach (Transform child in transform) {
				child.SendMessage("Save", SendMessageOptions.DontRequireReceiver);
			}
		}
		

		if(GUI.Button(new Rect(20,70,80,20), "Load")) {
			foreach (Transform child in transform) {
				child.SendMessage("Load", SendMessageOptions.DontRequireReceiver);
			}
		}

		directory = GUI.TextField(new Rect(20, 100, 580, 20), directory);
	}
}
