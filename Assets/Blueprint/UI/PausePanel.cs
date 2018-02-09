using UnityEngine;

public class PausePanel : BPPanel {

	new public void show (bool show) {
		base.show (show);
		if (show) {
			Main.playingmap.Pause ();
		} else {
			Main.playingmap.Resume ();
		}
	}

	public void TitleButton () {
		BPCanvas.titleBackPanel.show (true);
	}

	public void SaveButton () {
		MapManager.saveMap (Main.playingmap);
	}
}
