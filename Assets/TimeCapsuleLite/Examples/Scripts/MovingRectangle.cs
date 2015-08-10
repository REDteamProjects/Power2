using UnityEngine;
using System.Collections;

public class MovingRectangle : MonoBehaviour {

	public float location;
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3(1, Mathf.Abs(Mathf.Sin (location)) * 4, 1);
		location += Time.deltaTime;
	}
	
	public void Save() {
		TimeCapsuleManager.Instance.Save (location, this.name, SaveLoadTestGUI.directory, "save");
	}
	
	public void Load() {
		location = TimeCapsuleManager.Instance.Load<float> (this.name, SaveLoadTestGUI.directory, "save");
	}
}
