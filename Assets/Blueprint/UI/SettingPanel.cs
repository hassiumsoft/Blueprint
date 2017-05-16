using UnityEngine;

public class SettingPanel : MonoBehaviour {

	void OnEnable () {
		
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			show (false);
		}
	}

	void load () {

	}

	public void show (bool show) {
		if (show) {
			load ();
		}
		gameObject.SetActive (show);
	}

	public void save () {
		
		show (false);
	}
}
