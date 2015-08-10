using UnityEngine;
using UnityEditor;
using System.Collections;

public class TimeCapsuleEditor : EditorWindow {
	
	private static string m_filePath = "Whisper/";
	
	[MenuItem ("GameObject/Create Time Capsule", false, 900)]
	static void Init () {
		CreateTimeCapsule ();
	}
	
	static void CreateTimeCapsule() {
		var capsuleInstance = Resources.Load(m_filePath + "(Whisper)TimeCapsule", typeof(GameObject));
		if (capsuleInstance == null) {
			Debug.LogError("Could not find Time Capsule prefab. Please drag it into the scene yourself. It is located under Resources/Whisper.");
			return;
		}
		GameObject go = Instantiate (capsuleInstance) as GameObject;
		Undo.RegisterCreatedObjectUndo(go, "Added Time Capsule");
		go.name = "(Whisper)TimeCapsule";
		Debug.Log ("Time Capsule instantiated!");
	}
}
