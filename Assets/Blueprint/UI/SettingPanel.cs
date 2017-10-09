using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BPPanel {
	//TODO 多言語対応化
	public static string drawDistanceText_DEF = "描画距離";
	public static string bgmVolumeText_DEF = "BGM音量";
	public static string seVolumeText_DEF = "SE音量";
	public static string dragRotSpeedText_DEF = "マウスドラッグによるカメラ回転速度";
	public static string contrastStretchText_DEF = "コントラストストレッチ（目の筋肉）";
	public static string bloomText_DEF = "ブルーム（光の漏れ）";
	public Text drawDistanceText;
	public Slider drawDistanceSlider;
	public Text bgmVolumeText;
	public Slider bgmVolumeSlider;
	public Text seVolumeText;
	public Slider seVolumeSlider;
	public Text dragRotSpeedText;
	public Slider dragRotSpeedSlider;
	public Text contrastStretchText;
	public Toggle contrastStretchToggle;
	public Text bloomText;
	public Toggle bloomToggle;

	void Update () {
		drawDistanceText.text = drawDistanceText_DEF + ": " + (int)drawDistanceSlider.value;
		bgmVolumeText.text = bgmVolumeText_DEF + ": " + (int)(bgmVolumeSlider.value * 100f);
		seVolumeText.text = seVolumeText_DEF + ": " + (int)(seVolumeSlider.value * 100f);
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
		bgmVolumeSlider.minValue = Main.MIN_BGM_VOLUME;
		bgmVolumeSlider.maxValue = Main.MAX_BGM_VOLUME;
		bgmVolumeSlider.value = Main.bgmVolume;
		seVolumeSlider.minValue = Main.MIN_SE_VOLUME;
		seVolumeSlider.maxValue = Main.MAX_SE_VOLUME;
		seVolumeSlider.value = Main.seVolume;
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
		Main.bgmVolume = bgmVolumeSlider.value;
		Main.seVolume = seVolumeSlider.value;
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
