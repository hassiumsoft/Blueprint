using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BPPanel {
	public Text drawDistanceText;
	public Slider drawDistanceSlider;

	void Update () {
		drawDistanceText.text = "描画距離: " + (int)drawDistanceSlider.value;
		if (Input.GetKeyDown (KeyCode.Escape)) {
			show (false);
		}
	}

	void load () {
		drawDistanceSlider.minValue = Main.MIN_DRAW_DISTANCE;
		drawDistanceSlider.maxValue = Main.MAX_DRAW_DISTANCE;
		drawDistanceSlider.value = Main.drawDistance;
	}

	public void show (bool show) {
		if (show) {
			load ();
		}
		base.show (show);
	}

	public void save () {
		Main.drawDistance = (int)drawDistanceSlider.value;
		Main.saveSettings ();

		if (Main.playingmap != null) {
			//TODO マルチに対応させる必要がある
			Main.playingmap.DestroyChunkEntities ();
			Main.masterPlayer.playerEntity.reloadChunk ();
		}

		show (false);
	}

	public void reset () {
		//TODO
	}
}
