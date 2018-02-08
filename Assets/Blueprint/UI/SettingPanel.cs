using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BPPanel {
	public Text drawDistanceText;
	public Slider drawDistanceSlider;
   /* public InputField secretgate;
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
    }
    */
	void Update () {
		drawDistanceText.text = "描画距離: " + (int)drawDistanceSlider.value;
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
