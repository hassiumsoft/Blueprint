using UnityEngine;

public class Quitter : MonoBehaviour {
	//デバッグ用のゲーム終了機能
    //TODO [奇想天外]タイトルの終了ボタンが効かないのでこれの13-19行目をTitlePanel.csに移植する。

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
