using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class MapManager {
	public const string mapfilename = "map.bin";
	public static string dir; //マップファイルを格納するフォルダ

	//TODO (Mapも参照)
	//public static string[] enablemapvers = new string[] { "0.1" } //使用可能なマップのバージョン

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

	/*public static bool createNewMap (string mapname) {
		reloadDir ();
		if (Directory.Exists (Path.Combine (dir, mapname))) {
			return false;
		}
		//saveMap (new Map (Map.latestver, mapname));
		saveMap (new Map (mapname));
		return true;
	}
	
	public static bool isEnableMapVer (string mapver) {
		for (int a = 0; a < enablemapvers.Length; a++) {
			if (enablemapvers [a].Equals (mapver)) {
				return true;
			}
		}
		return false;
	}*/

	public static string getRandomMapName () {
		/*if (randommapnames.Length == 0) {
			return "";
		}*/
		//マップが存在するかどうか確認する場合は無限ループを避けるためwhileを使っていはいけない。
		return randommapnames [Random.Range (0, randommapnames.Length)];
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
				Map map = (Map)formatter.Deserialize (stream);
				stream.Close ();
				/*if (isEnableMapVer(map.ver)) {
					return map;
				}*/
				return map;
			}
		}
		return null;
	}

	public static void saveMap (Map map) {
		reloadDir ();
		string mapdir = Path.Combine (dir, map.mapname);
		Directory.CreateDirectory (mapdir);
		IFormatter formatter = new BinaryFormatter ();
		Stream stream = new FileStream (Path.Combine (mapdir, mapfilename), FileMode.Create, FileAccess.Write, FileShare.None);
		formatter.Serialize (stream, map);
		stream.Close ();
		Debug.Log ("マップをセーブしました: " + map.mapname);
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
				Debug.Log ("マップを削除しました: " + mapname);
				return true;
			}
		}
		return false;
	}

	/*TODO 使う可能性もあるが、今のところは一般的な数字で表示させる。
	public static string getMoneyString(long money) {
		if (money == 0) {
			return "0円";
		}
		char[] a = money.ToString().ToCharArray();
		string str = money <= 10000 * 10000 ? "<color=red>" : "<color=white>";
		int b = a.Length;
		while (4 < b) {
			b -= 4;
		}
		bool c = false;
		bool d = false;
		for (int e = 0; e < a.Length; e++) {
			if (a[e].Equals('1')) {
				if (1 < a.Length && a.Length < 5) {
					str += "一";
					c = true;
					d = true;
				}
			} else if (a[e].Equals('2')) {
				str += "二";
				c = true;
				d = true;
			} else if (a[e].Equals('3')) {
				str += "三";
				c = true;
				d = true;
			} else if (a[e].Equals('4')) {
				str += "四";
				c = true;
				d = true;
			} else if (a[e].Equals('5')) {
				str += "五";
				c = true;
				d = true;
			} else if (a[e].Equals('6')) {
				str += "六";
				c = true;
				d = true;
			} else if (a[e].Equals('7')) {
				str += "七";
				c = true;
				d = true;
			} else if (a[e].Equals('8')) {
				str += "八";
				c = true;
				d = true;
			} else if (a[e].Equals('9')) {
				str += "九";
				c = true;
				d = true;
			}
			if (c) {
				if (b == 2) {
					str += "十";
					c = false;
				} else if (b == 3) {
					str += "百";
					c = false;
				} else if (b == 4) {
					str += "千";
					c = false;
				}
			}
			if (d) {
				if (e == a.Length - 5) {
					str += "万";
					b = 5;
					c = false;
					d = false;
				} else if (e == a.Length - 9) {
					str += "億";
					b = 5;
					c = false;
					d = false;
				} else if (e == a.Length - 13) {
					str += "兆";
					b = 5;
					c = false;
					d = false;
				} else if (e == a.Length - 17) {
					str += "京";
					b = 5;
					c = false;
					d = false;
				} else if (e == a.Length - 21) {
					str += "垓";
					b = 5;
					c = false;
					d = false;
				}
			}
			b--;
		}
		return str + "円</color>";
	}*/
}
