using UnityEngine;
using UnityEngine.UI;

public class AddMapPanel : BPPanel {
	public SelectMapPanel selectMapPanel;
	public InputField mapnameInput;
    public InputField mapmodeInput;

    GameObject pv;
    DialogPanel DialogPanelScript;

    void Start()
    {
        pv = GameObject.Find("DialogPanel");
        DialogPanelScript = pv.GetComponent<DialogPanel>();
    }


	void OnEnable () {
		mapnameInput.text = MapManager.getRandomMapName ();
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			show (false);
		}
	}

	public void AddMap () {
		string mapname = mapnameInput.text.Trim ();

		if (mapname.Length == 0) {
            DialogPanelScript.information("map Length is too lower");
        } else {
			string[] a = MapManager.getMapList ();
			for (int b = 0; b < a.Length; b++) {
				if (a [b].ToLower ().Equals (mapname.ToLower ())) {
                    //TODO ダイアログ
					return;
				}
			}

			//TODO マップ追加を非同期に対応させる
			MapManager.saveMap (new Map (mapname));
			//selectMapPanel.reloadContents ();
			show (false);
			selectMapPanel.show (false);
			Main.main.StartCoroutine (Main.openMap (mapname));
		}
	}
}
