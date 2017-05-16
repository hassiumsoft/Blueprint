using System;
using System.Collections.Generic;

[Serializable]
public class Map {
	//TODO シリアライズ機能を応用してバージョン判定を使わずにマップの適合性があるか判定することが出来る場合は必要ない
	//public const string latestver = "0"; //最新のマップバージョン

	//TODO 未使用
	public const int mapsize = 1024; //マップのサイズ。mapsize²となる

	//public const long startmoney = 500L * 10000 * 10000; //初期の所持金
	//public const long tree_sale = 12000; //木の値段
	//public const long stone_sale = 1000; //石の値段
	//TODO 経済データに移行、経済はまだ無い

	//public string ver { private set; get; } //マップのバージョン
	//TODO マップのバージョンは必要か

	public string mapname { private set; get; } //マップ名

	//チャンクデータ
	//初期状態では全チャンクが未生成(null)
	public List<Chunk> chunks;

	//public SerializableVector2 spawnpoint; TODO スポーン地点。状況に応じる必要があるためgetメソッド化にした方がいいかもしれない。
	//Vector3にした方がいいかもしれない。

	//public long money; //所持金
	//TODO プレイヤー等の所持金に変更

	//public string oldver { private set; get; } //マップの旧バージョン(読み込み時のバージョン)
	//public long starttime { private set; get; } //マップの開始時間
	//public long playtime; //マップのプレイ時間
	//public long time; //マップの時間
	//TODO 時間の概念は開発中

	/*public Map (string ver, string mapname) {
		oldver = ver;
		this.ver = latestver;
		this.mapname = mapname;
		
		money = startmoney;
		starttime = DateTime.Now.Ticks;
		playtime = 0;
		time = 0;
	}*/

	public Map (string mapname) {
		this.mapname = mapname;

		chunks = new List<Chunk> ();
	}

	public int getChunk (int x, int y) {
		for (int n = 0; n < chunks.Count; n++) {
			if (chunks [n].x == x && chunks [n].y == y) {
				return n;
			}
		}
		return -1;
	}
}
