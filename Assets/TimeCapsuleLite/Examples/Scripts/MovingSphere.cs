using UnityEngine;
using System.Collections;

public class MovingSphere : MonoBehaviour {
	
	public float location;
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3(Mathf.Sin (location)  * 4, Mathf.Sin (location)  * 4, Mathf.Sin (location)  * 4);
		location += Time.deltaTime;
	}
	
	public void Save() {
		TimeCapsuleManager.Instance.Save (location, this.name, SaveLoadTestGUI.directory, "save");
	}
	
	public void Load() {
		location = TimeCapsuleManager.Instance.Load<float> (this.name, SaveLoadTestGUI.directory, "save");
	}
}
