using UnityEngine;

public class PausePanel : BPPanel {
	
	public void TitleButton () {
		//TODO マルチプレイ対応予定

		show (false);
		Main.closeMap ();
		BPCanvas.bpCanvas.titlePanel.show (true);
	}

	//Escapeキーでポーズメニューの切替時にポーズメニューを表示するかを返す
	public bool a () {
		return BPCanvas.bpCanvas.settingPanel.isShowing () ? isShowing () : !isShowing ();
	}
}
