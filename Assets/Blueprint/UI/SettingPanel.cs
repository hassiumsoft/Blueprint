using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BPPanel {
	//TODO 多言語対応化
	public static string drawDistanceText_DEF = "描画距離";
	public static string dragRotSpeedText_DEF = "マウスドラッグによるカメラ回転速度";
	public static string contrastStretchText_DEF = "コントラストストレッチ（目の筋肉）";
	public static string bloomText_DEF = "ブルーム（光の漏れ）";
	public Text drawDistanceText;
	public Slider drawDistanceSlider;
	public Text dragRotSpeedText;
	public Slider dragRotSpeedSlider;
	public Text contrastStretchText;
	public Toggle contrastStretchToggle;
	public Text bloomText;
	public Toggle bloomToggle;

	void Update () {
		drawDistanceText.text = drawDistanceText_DEF + ": " + (int)drawDistanceSlider.value;
		dragRotSpeedText.text = dragRotSpeedText_DEF + ": " + Mathf.Round (dragRotSpeedSlider.value * 100f) / 100f;
		contrastStretchText.text = contrastStretchText_DEF + ": ";
		bloomText.text = bloomText_DEF + ": ";
		if (Input.GetKeyDown (KeyCode.Escape)) {
			show (false);
		}
	}

	void load () {
		drawDistanceSlider.minValue = Main.MIN_DRAW_DISTANCE;
		drawDistanceSlider.maxValue = Main.MAX_DRAW_DISTANCE;
		drawDistanceSlider.value = Main.drawDistance;
		dragRotSpeedSlider.minValue = Main.MIN_DRAG_ROT_SPEED;
		dragRotSpeedSlider.maxValue = Main.MAX_DRAG_ROT_SPEED;
		dragRotSpeedSlider.value = Main.dragRotSpeed;
		contrastStretchToggle.isOn = Main.contrastStretch;
		bloomToggle.isOn = Main.bloom;
	}

	new public void show (bool show) {
		if (show) {
			load ();
		}
		base.show (show);
	}

	public void save () {
		Main.drawDistance = (int)drawDistanceSlider.value;
		Main.dragRotSpeed = Mathf.Round (dragRotSpeedSlider.value * 100f) / 100f;
		Main.contrastStretch = contrastStretchToggle.isOn;
		Main.bloom = bloomToggle.isOn;
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
