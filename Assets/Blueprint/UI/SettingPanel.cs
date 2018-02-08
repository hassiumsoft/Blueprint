using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BPPanel {
	public Text drawDistanceText;
	public Slider drawDistanceSlider;
    public InputField secretgate;
    public string pass;

    void Start()
    {
        secretgate = GetComponent<InputField>();

        ResetString(secretgate.text);
        ActivateIF(secretgate);

        string pass = 66666666;
    }

	void Update () {
		drawDistanceText.text = "描画距離: " + (int)drawDistanceSlider.value;
		if (Input.GetKeyDown (KeyCode.Escape)) {
			show (false);
		}
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (secretgate.text == pass)
            {

            }
        }
	}

	void load () {
		drawDistanceSlider.minValue = Main.MIN_DRAW_DISTANCE;
		drawDistanceSlider.maxValue = Main.MAX_DRAW_DISTANCE;
		drawDistanceSlider.value = Main.drawDistance;
	}

    void ResetString(string A)
    {
        A = "";
    }

    void ActivateIF(InputField A)
    {
        A.ActivateInputField();
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
