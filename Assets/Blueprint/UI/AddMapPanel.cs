using UnityEngine;
using UnityEngine.UI;

public class AddMapPanel : MonoBehaviour {
	public SelectMapPanel selectMapPanel;
	public InputField mapnameInput;

	void OnEnable () {
		mapnameInput.text = MapManager.getRandomMapName ();
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			show (false);
		}
	}

	public void show (bool show) {
		gameObject.SetActive (show);
	}

	public void AddMap () {
		string mapname = mapnameInput.text.Trim ();

		if (mapname.Length == 0) {
			//TODO 警告ダイアログ
		} else {
			string[] a = MapManager.getMapList ();
			for (int b = 0; b < a.Length; b++) {
				if (a [b].ToLower ().Equals (mapname.ToLower ())) {
					//TODO ダイアログ
					return;
				}
			}

			MapManager.saveMap (new Map (mapname));
			selectMapPanel.reloadContents ();
			show (false);
		}
	}
}
