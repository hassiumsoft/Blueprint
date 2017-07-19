using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class MapManager {
	public const string mapfilename = "map.bin";
	public static string dir; //マップファイルを格納するフォルダ

	public static string[] randommapnames = new string[] {"green city", "new bridge", "blue forest", "thousand leaf",
		"stone river", "fortune island", "mountain hand", "northeast",
		"red castle", "new lodge", "east capital", "small mountain",
		"white river", "white stone", "old river", "plateau",
		"long hill", "long field", "sakura city", "big ship",
		"east city", "west city", "south city", "north city",
		"central city", "forest city", "black stone", "sunset beach",
		"blue leaf", "chestnut field", "pine island", "harbor mountain",
		"east river", "west river", "south river", "north river",
		"pine mountain", "pine village", "pine river", "pine hill",
		"pine beach", "mountain hill", "big river", "big mountain"
	}; //マップ名のサンプル

	public static void reloadDir () {
		dir = Path.Combine (Application.persistentDataPath, "maps");
		Directory.CreateDirectory (dir);
	}

	public static string[] getMapList () {
		List<string> maplist = new List<string> ();
		reloadDir ();
		string[] mapdirs = Directory.GetDirectories (dir);
		for (int a = 0; a < mapdirs.Length; a++) {
			string mapname = new DirectoryInfo (mapdirs [a]).Name;
			string[] files = Directory.GetFiles (Path.Combine (dir, mapname));
			string datpath = null;
			for (int b = 0; b < files.Length; b++) {
				if (Path.GetFileName (files [b]).Equals (mapfilename)) {
					datpath = files [b];
					break;
				}
			}
			if (datpath != null) {
				maplist.Add (mapname);
			}
		}
		return maplist.ToArray ();
	}

	public static string getRandomMapName () {
		/*if (randommapnames.Length == 0) {
			return "";
		}*/
		//マップが存在するかどうか確認する場合は無限ループを避けるためwhileを使っていはいけない。
		return randommapnames [UnityEngine.Random.Range (0, randommapnames.Length)];
	}

	public static Map loadMap (string mapname) {
		reloadDir ();
		string mapdir = Path.Combine (dir, mapname);
		if (Directory.Exists (mapdir)) {
			string[] files = Directory.GetFiles (mapdir);
			string datpath = null;
			for (int a = 0; a < files.Length; a++) {
				if (Path.GetFileName (files [a]).Equals (mapfilename)) {
					datpath = files [a];
					break;
				}
			}
			if (datpath != null) {
				IFormatter formatter = new BinaryFormatter ();
				Stream stream = new FileStream (Path.Combine (mapdir, mapfilename), FileMode.Open, FileAccess.Read, FileShare.Read);
				Map map = null;
				try {
					map = (Map)formatter.Deserialize (stream);
				} catch (EndOfStreamException) {
					Debug.LogError (DateTime.Now + " マップが対応していません: " + mapfilename);
				}
				stream.Close ();
				return map;
			}
		}
		return null;
	}

	public static void aaa (Map map) {
		reloadDir ();
		string mapdir = Path.Combine (dir, map.mapname);
		Directory.CreateDirectory (mapdir);
		IFormatter formatter = new BinaryFormatter ();
		Stream stream = new FileStream (Path.Combine (mapdir, mapfilename), FileMode.Create, FileAccess.Write, FileShare.None);
		formatter.Serialize (stream, map);
		stream.Close ();
	}

	public static void saveMap (Map map) {
		Debug.Log (DateTime.Now + " マップ\"" + map.mapname + "\"をセーブ中...");
		aaa (map);
		Debug.Log (DateTime.Now + " マップをセーブしました");
	}

	public static bool deleteMap (string mapname) {
		reloadDir ();
		string mapdir = Path.Combine (dir, mapname);
		if (Directory.Exists (mapdir)) {
			string[] files = Directory.GetFiles (mapdir);
			string datpath = null;
			for (int a = 0; a < files.Length; a++) {
				if (Path.GetFileName (files [a]).Equals (mapfilename)) {
					datpath = files [a];
					break;
				}
			}
			if (datpath != null) {
				File.Delete (datpath);
				try {
					Directory.Delete (mapdir);
				} catch (IOException) {
					//フォルダの中身がある場合はフォルダを削除できない。
				}
				Debug.Log (DateTime.Now + " マップを削除しました: " + mapname);
				return true;
			}
		}
		return false;
	}
}
