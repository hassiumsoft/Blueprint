using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {
	public const string VERSION = "0.001alpha";
	public const string KEY_FIRSTSTART = "FIRSTSTART";
	public const string KEY_SETUPPED = "SETUPPED";
	public const string KEY_DRAW_DISTANCE = "DRAWDISTANCE";

	public const int MIN_DRAW_DISTANCE = 1;
	public const int MAX_DRAW_DISTANCE = 8;
	public const int DEFAULT_DRAW_DISTANCE = MAX_DRAW_DISTANCE / 4;

	public static Main main;
	public static Map playingmap { get; private set; }
	public static Player masterPlayer { get; private set; }
	public static string ssdir { get; private set; }
	public static int min_fps = 15;
	public static float min_reflectionIntensity = 1f / 32;

	private static bool firstStart = false;
	public static bool isFirstStart {
		get { return firstStart; }
		private set { firstStart = value; }
	}
	public static DateTime[] firstStartTimes { get; private set; }
	public static bool isSetupped = false;
	public static int drawDistance = DEFAULT_DRAW_DISTANCE;

	private static float lasttick = 0; //時間を進ませた時の余り
	public Light sun; //太陽

	//TODO ポーズメニューでプレイヤーなどの動きを停止させる。
	//TODO セーブ中の画面
	//TODO 時間が実時間と同じスピードで進むため、時間を早く進ませたりスキップしたりする機能を追加する必要がある。

	//TODO 以下、一時的
	public Material mat; //Chunk.csにて使用中
	public PlayerEntity playerPrefab;

	void Awake () {
		Main.main = this;

		//QualitySettings.vSyncCount = 0;//初期値は1
		Application.targetFrameRate = 60;//初期値は-1

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

		//初回起動かどうか（初期設定などをせずに一度ゲームを終了した場合などに対応できないため、あまり使えない）
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
		//print ("firstStart: " + firstStart);
		/*a = "{ ";
		for (int f = 0; f < firstStartTimes.Length; f++) {
			if (f != 0) {
				a += ", ";
			}
			a += firstStartTimes [f].Year + "/" + firstStartTimes [f].Month + "/" + firstStartTimes [f].Day + "-" + firstStartTimes [f].Hour + ":" + firstStartTimes [f].Minute + ":" + firstStartTimes [f].Second;
		}
		a += " }";
		print ("firstStartTimes: " + a);*/

		ssdir = Path.Combine (Application.persistentDataPath, "screenshots");

		//初期設定を行っているかどうか
		isSetupped = PlayerPrefs.GetInt(KEY_SETUPPED, 0) == 1;
		//print ("isSetupped: " + isSetupped);

		drawDistance = PlayerPrefs.GetInt (KEY_DRAW_DISTANCE, DEFAULT_DRAW_DISTANCE);

		Player.playerPrefab = playerPrefab;
	}

	void Start () {
		/*if (isSetupped) {
			BPCanvas.bpCanvas.titlePanel.show (true);
		} else {
			//TODO 初期設定
		}*/

		BPCanvas.titlePanel.show (true);
	}

	void Update () {
		//主に操作などを追加する。プレイヤー関連はプレイヤーにある。
		if (Input.GetKeyDown (KeyCode.F2)) {
			screenShot ();
		} else if (Input.GetKeyDown (KeyCode.Escape)) {
			//TODO 新しい画面を追加したときは同時に反応してしまうのを防ぐため、対応させる必要がある。
			if (playingmap != null) {
				if (!BPCanvas.settingPanel.isShowing () && !BPCanvas.titleBackPanel.isShowing ()) {
					BPCanvas.pausePanel.show (!BPCanvas.pausePanel.isShowing ());
				}
			}
		}

		if (playingmap != null && !playingmap.pause) {
			//時間を進ませる
			lasttick += Time.deltaTime * 1000f;

			int ticks = Mathf.FloorToInt (lasttick);
			lasttick -= ticks;
			playingmap.TimePasses (ticks);

			sun.transform.eulerAngles = new Vector3 (0, 0, 0);

			float t = Mathf.Repeat (playingmap.time, 86400000f); //86400000ms = 1日
			float r = t * 360f / 86400000f - 75f;
			sun.transform.localEulerAngles = new Vector3 (r, -90f, 0f);
			float intensity = Mathf.Max (1f - Mathf.Abs ((r + 90f) / 180f - 1f), min_reflectionIntensity);
			sun.intensity = RenderSettings.ambientIntensity = RenderSettings.reflectionIntensity = intensity;
		}
	}

	public void quit () {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#elif !UNITY_WEBPLAYER
		Application.Quit ();
		#endif
	}

	public void screenShot () {
		Directory.CreateDirectory (ssdir);
		string fileName = DateTime.Now.Ticks + ".png";
		Application.CaptureScreenshot (Path.Combine (ssdir, fileName));
		print (DateTime.Now + " ScreenShot: " + fileName);
	}

	public void openSSDir () {
		Directory.CreateDirectory (ssdir);
		Process.Start (ssdir);
	}

	public static void saveSettings () {
		PlayerPrefs.SetInt (KEY_DRAW_DISTANCE, drawDistance);
	}

	public static IEnumerator openMap (string mapname) {
		if (playingmap != null) {
			closeMap ();
		}
		BPCanvas.titlePanel.show (false);
		BPCanvas.loadingMapPanel.show (true);

		//一回だとフレーム等のズレによってTipsが表示されない
		yield return null;
		yield return null;
		yield return null;
		Map map = MapManager.loadMap (mapname);
		yield return null;
		if (map == null) {
			//マップが対応していない
			BPCanvas.loadingMapPanel.show (false);
			BPCanvas.titlePanel.show (true);
			BPCanvas.selectMapPanel.setOpenMap ();
			BPCanvas.selectMapPanel.show (true);
			BPCanvas.unsupportedMapPanel.show (true);
		} else {
			playingmap = map;

			//TODO プレイヤーの生成に時間がかかる

			int pid = playingmap.getPlayer ("master");//TODO 仮
			if (pid == -1) {
				playingmap.players.Add (masterPlayer = new Player (playingmap, "master"));
			} else {
				masterPlayer = playingmap.players [pid];
			}
			masterPlayer.generate ();
			BPCanvas.loadingMapPanel.show (false);

			print (DateTime.Now + " マップを開きました: " + map.mapname);
		}
	}

	public static void closeMap () {
		if (playingmap != null) {
			masterPlayer = null;

			playingmap.DestroyAll ();
			playingmap = null;
		}
	}

	//描画を優先して負荷のかかる処理を行うため、描画状態に応じてyield returnを行う条件を返すメソッド
	public static bool yrCondition () {
		return 1 / Time.deltaTime < Main.min_fps;
	}
}
