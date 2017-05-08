using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {
	//public Camera c;
	public Canvas title;

	void Start () {
		showTitle (true);
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			quit ();
		}
	}

	public void quit () {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#elif !UNITY_WEBPLAYER
		Application.Quit ();
		#endif
	}

	public void showTitle (bool show) {
		title.enabled = show;
	}
}
