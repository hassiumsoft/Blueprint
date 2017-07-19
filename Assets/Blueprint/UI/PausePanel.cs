using UnityEngine;

public class PausePanel : BPPanel {
	
	public void TitleButton () {
		BPCanvas.titleBackPanel.show (true);
	}

	public void SaveButton () {
		MapManager.saveMap (Main.playingmap);
	}
}
