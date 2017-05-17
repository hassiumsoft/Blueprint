using System;
using System.Collections.Generic;

[Serializable]
public class Map {
	public string mapname { private set; get; } //マップ名

	//チャンクデータ
	//初期状態では全チャンクが未生成(null)
	public List<Chunk> chunks;

	//public SerializableVector2 spawnpoint; TODO スポーン地点。状況に応じる必要があるためgetメソッド化にした方がいいかもしれない。
	//Vector3にした方がいいかもしれない。

	//public long starttime { private set; get; } //マップの開始時間
	//public long playtime; //マップのプレイ時間
	//public long time; //マップの時間
	//TODO 時間の概念は開発中

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
