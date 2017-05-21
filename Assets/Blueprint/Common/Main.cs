﻿using System;
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

	public static Main main;
	private static bool firstStart = false;
	public static bool isFirstStart {
		get { return firstStart; }
		private set { firstStart = value; }
	}
	public static DateTime[] firstStartTimes { get; private set; }
	public static string ssdir { get; private set; }
	public static bool isSetupped = false;
	public static Map playingmap { get; private set; }

	//public Camera c;

	//TODO 一時的
	public Material mat;
	public GameObject playerPrefab;

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

		ssdir = Path.Combine (Application.persistentDataPath, "screenshots");

		//初期設定を行っているかどうか
		isSetupped = PlayerPrefs.GetInt(KEY_SETUPPED, 0) == 1;
		print ("isSetupped: " + isSetupped);
	}

	void Start () {
		/*if (isSetupped) {
			BPCanvas.bpCanvas.titlePanel.show (true);
		} else {
			//TODO 初期設定
		}*/

		BPCanvas.bpCanvas.titlePanel.show (true);
	}

	void Update () {
		//ScreenShot
		if (Input.GetKeyDown (KeyCode.F2)) {
			Directory.CreateDirectory (ssdir);
			string fileName = DateTime.Now.Ticks + ".png";
			Application.CaptureScreenshot (Path.Combine (ssdir, fileName));
			print ("ScreenShot: " + fileName);
		}
	}

	public void quit () {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#elif !UNITY_WEBPLAYER
		Application.Quit ();
		#endif
	}

	public void openSSDir () {
		Directory.CreateDirectory (ssdir);
		Process.Start (ssdir);
	}

	public static IEnumerator openMap (string mapname) {
		if (playingmap != null) {
			closeMap ();
		}
		BPCanvas.bpCanvas.titlePanel.show (false);
		BPCanvas.bpCanvas.loadingMapPanel.show (true);

		//一回だとフレーム等のズレによってTipsが表示されない
		yield return null;
		yield return null;

		Map map = MapManager.loadMap (mapname);
		BPCanvas.bpCanvas.loadingMapPanel.show (false);
		yield return null;
		if (map == null) {
			//TODO マップが対応していない場合のダイアログを表示

			BPCanvas.bpCanvas.titlePanel.show (true);
			BPCanvas.bpCanvas.selectMapPanel.setOpenMap ();
			BPCanvas.bpCanvas.selectMapPanel.show (true);
		} else {
			playingmap = map;
			print ("マップを開きました: " + map.mapname);
		}
	}

	public static void closeMap () {
		//TODO
		if (playingmap != null) {
			/*BlockRenderer[] renderers = GameObject.FindObjectsOfType<BlockRenderer> ();
			for (int a = 0; a < renderers.Length; a++) {
				Object.Destroy (renderers [a].gameObject);
			}*/
		}
		playingmap = null;
	}

	public void a () {
		StartCoroutine (b ());
	}

	public IEnumerator b () {
		if (playingmap != null) {
			for (int x = -2; x < 2; x++) {
				for (int y = -2; y < 2; y++) {
					if (playingmap.getChunk (x, y) == -1) {
						playingmap.chunks.Add (new Chunk (playingmap, x, y));
					}
				}
			}

			for (int a = 0; a < playingmap.chunks.Count; a++) {
				//TODO 一時的
				playingmap.chunks [a].mat = mat;

				yield return StartCoroutine (playingmap.chunks [a].generate (this));
			}

			int pid = playingmap.getPlayer ("master");//TODO 仮
			Player player;
			if (pid == -1) {
				playingmap.players.Add (player = new Player (playingmap, "master"));
			} else {
				player = playingmap.players [pid];
			}
			player.playerPrefab = playerPrefab;
			yield return StartCoroutine (player.generate (this));

			yield return StartCoroutine (MapManager.saveMapAsync (playingmap));
		}
	}
}
