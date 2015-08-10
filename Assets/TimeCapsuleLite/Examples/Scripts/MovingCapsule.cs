using UnityEngine;
using System.Collections;

public class MovingCapsule : MonoBehaviour {

	public float location;

	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(transform.position.x, Mathf.Sin (location)  * 4, transform.position.z);
		location += Time.deltaTime;
	}

	public void Save() {
		TimeCapsuleManager.Instance.Save (location, this.name, SaveLoadTestGUI.directory, "save");
	}
	
	public void Load() {
		location = TimeCapsuleManager.Instance.Load<float> (this.name, SaveLoadTestGUI.directory, "save");
	}
}
