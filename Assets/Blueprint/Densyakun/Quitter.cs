using UnityEngine;

public class Quitter : MonoBehaviour {
	//デバッグ用のゲーム終了機能

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
}
