using UnityEngine;

public class TitlePanel : MonoBehaviour {

	void OnEnable () {

	}

	public void show (bool show) {
		gameObject.SetActive (show);
	}
}
