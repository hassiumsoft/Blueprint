using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BPPanel {
	//TODO 多言語対応化
	public static string drawDistanceText_DEF = "描画距離";
	public static string bgmVolumeText_DEF = "BGM音量";
	public static string seVolumeText_DEF = "SE音量";
	public static string dragRotSpeedText_DEF = "マウスドラッグによるカメラ回転速度";
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

	/*public InputField secretgate;
    public string passon;
    public string passoff;
    public Text DebugText;
    public bool debug;*/
	
    /*void Start()
    {
        secretgate = GetComponent<InputField>();

        ResetString(secretgate.text);
        ActivateIF(secretgate);

        passon = "68098898aa";
        passoff = "423324434bb";
    }*/

	void Update () {
		drawDistanceText.text = drawDistanceText_DEF + ": " + (int)drawDistanceSlider.value;
		bgmVolumeText.text = bgmVolumeText_DEF + ": " + (int)(bgmVolumeSlider.value * 100f);
		seVolumeText.text = seVolumeText_DEF + ": " + (int)(seVolumeSlider.value * 100f);
		dragRotSpeedText.text = dragRotSpeedText_DEF + ": " + Mathf.Round (dragRotSpeedSlider.value * 100f) / 100f;
		bloomText.text = bloomText_DEF + ": ";
		if (Input.GetKeyDown (KeyCode.Escape)) {
			show (false);
		}
        /*if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (secretgate.text == passon)
            {
                debug = true;
            }
            if (secretgate.text == passoff)
            {
               debug = false;
            }
        }
        if (debug)
        {
            DebugText.text = "welcome to debug mode.";
        }else
        {
            DebugText.text = "";
        }*/
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
		bloomToggle.isOn = Main.bloom;
	}

    void ResetString(string A)
    {
        A = "";
    }

    void ActivateIF(InputField A)
    {
        A.ActivateInputField();
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
