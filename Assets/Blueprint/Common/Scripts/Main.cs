using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {
	public const string VERSION = "0.001alpha";
	public const string KEY_FIRSTSTART = "FIRSTSTART";

	//public static Main main;
	private static bool firstStart = false;
	public static bool isFirstStart {
		get { return firstStart; }
		private set { firstStart = value; }
	}
	public static DateTime[] firstStartTimes { get; private set; }
	public static String ssdirpath { get; private set; }
	
	//public Camera c;
	public Canvas title;
	public Text title_version;
	public Canvas mapSelect;

	public static Map playingmap;

	void Awake () {
		//main = this;

		//QualitySettings.vSyncCount = 0;
		//Application.targetFrameRate = 60;

		MapManager.init ();

		//ゲーム起動日時の取得
		string a = PlayerPrefs.GetString (KEY_FIRSTSTART);//変数aは使いまわしているので注意
		bool b = false;
		List<DateTime> c = new List<DateTime> ();
		try {
			String[] d = a.Split (',');
			for (int e = 0; e < d.Length; e++) {
				c.Add (new DateTime (long.Parse (d [e].Trim ())));
			}
			if (d.Length == 0) {
				b = true;
			}
		} catch (FormatException) {
			b = true;
		}

		//初回起動かどうか
		if (b) {
			firstStart = true;
		}

		//今回の起動日時を追加
		c.Add (DateTime.Now);
		firstStartTimes = c.ToArray ();
		a = "";
		for (int f = 0; f < firstStartTimes.Length; f++) {
			if (f != 0) {
				a += ", ";
			}
			a += firstStartTimes [f].Ticks;
		}
		PlayerPrefs.SetString (KEY_FIRSTSTART, "" + a);

		//ゲーム起動日時及び、ゲーム初回起動情報をコンソールに出力
		print ("firstStart: " + firstStart);
		a = "{ ";
		for (int f = 0; f < firstStartTimes.Length; f++) {
			if (f != 0) {
				a += ", ";
			}
			a += firstStartTimes [f].Year + "/" + firstStartTimes [f].Month + "/" + firstStartTimes [f].Day + "-" + firstStartTimes [f].Hour + ":" + firstStartTimes [f].Minute + ":" + firstStartTimes [f].Second;
		}
		a += " }";
		print ("firstStartTimes: " + a);

		ssdirpath = Path.Combine (Application.persistentDataPath, "screenshots");
	}

	void Start () {
		showTitle ();
	}

	void Update () {
		//ScreenShot
		if (Input.GetKeyDown (KeyCode.F2)) {
			Directory.CreateDirectory (ssdirpath);
			string fileName = DateTime.Now.Ticks + ".png";
			Application.CaptureScreenshot (Path.Combine (ssdirpath, fileName));
			print ("ScreenShot: " + fileName);
		}

		//Close Window
		if (Input.GetKeyDown (KeyCode.Escape)) {
			clearWindow ();
			/*if (title.enabled) {
				quit ();
			}*/
		}
	}

	public void quit () {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#elif !UNITY_WEBPLAYER
		Application.Quit ();
		#endif
	}

	public void showTitle () {
		clearCanvas ();
		title_version.text = "Ver: " + VERSION;
		title.enabled = true;
	}

	public void clearCanvas () {
		title.enabled = false;
	}

	public void showMapSelectWindow () {
		clearWindow ();
		mapSelect.enabled = true;
	}

	public void clearWindow () {
		mapSelect.enabled = false;
	}

	public void openSSDir () {
		Directory.CreateDirectory (ssdirpath);
		Process.Start (ssdirpath);
	}
}
