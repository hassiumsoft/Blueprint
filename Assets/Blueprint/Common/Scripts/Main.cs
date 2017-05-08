using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
	
	void Start () {
		
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#elif UNITY_WEBPLAYER
			//Application.OpenURL (webplayerQuitURL);
			#else
			Application.Quit ();
			#endif
		}
	}
}
