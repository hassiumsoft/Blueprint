using UnityEngine;
using UnityEngine.UI;

public class TimeText : MonoBehaviour {
	Text text;

	void OnEnable () {
		text = GetComponent<Text> ();
	}

	void Update () {
		if (Main.playingmap == null) {
			text.text = "---";
		} else {
			text.text = "現在時刻: " + Main.playingmap.time;
		}
	}
}
