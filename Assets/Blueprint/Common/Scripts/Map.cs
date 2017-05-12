using System;

[Serializable]
public class Map {
	//public const string latestver = "0"; //最新のマップバージョン
	//public const int mapsize = 1024; //マップのサイズ。mapsize x mapsizeとなる。数字は必ず16（レンダリングチャンクの大きさ）で割れる数にすること

	//public const long startmoney = 500L * 10000 * 10000; //初期の所持金
	//public const long tree_sale = 12000; //木の値段
	//public const long stone_sale = 1000; //石の値段
	//TODO 経済データに移行、経済はまだ無い

	//public string ver { private set; get; } //マップのバージョン
	//TODO マップのバージョンは必要か

	public string mapname { private set; get; } //マップ名

	//public Block[][] blockmap; //ブロックのデータ（blockmap[x][y]）
	//> 地形データ

	//public long money; //所持金
	//TODO プレイヤー等の所持金に変更

	//public string oldver { private set; get; } //マップの旧バージョン(読み込み時のバージョン)
	//public long starttime { private set; get; } //マップの開始時間
	//public long playtime; //マップのプレイ時間
	//public long time; //マップの時間
	//TODO 時間の概念は開発中

	public Map(/*string ver, */string mapname) {
		//oldver = ver;
		//this.ver = latestver;
		this.mapname = mapname;
		/*blockmap = new Block[mapsize][];
		for (int a = 0; a < blockmap.Length; a++) {
			Block[] b = new Block[mapsize];
			for (int c = 0; c < b.Length; c++) {
				switch (UnityEngine.Random.Range(0, 1024)) {
				case 0:
					b[c] = Block.tree;
					break;
				case 1:
					b[c] = Block.stone;
					break;
				default:
					b[c] = Block.grass;
					break;
				}
			}
			blockmap[a] = b;
		}
		for (int z = 0; z < 10; z++) {
			for (int a = 0; a < blockmap.Length; a++) {
				for (int c = 0; c < blockmap[a].Length; c++) {
					if (blockmap[a][c].id == Block.tree.id) {
						for (int d = -1; d <= 1; d++) {
							for (int e = -1; e <= 1; e++) {
								if (0 < a + d && a + d < mapsize && 0 < c + e && c + e < mapsize && UnityEngine.Random.Range(0, 8) == 0) {
									if (blockmap[a + d][c + e].id == Block.grass.id) {
										blockmap[a + d][c + e] = Block.tree;
									} else if (blockmap[a + d][c + e].id == Block.tree.id) {
										blockmap[a + d][c + e] = Block.forest;
									}
								}
							}
						}
					}
				}
			}
		}*/
		//money = startmoney;
		//starttime = DateTime.Now.Ticks;
		//playtime = 0;
		//time = 0;
	}

	/*public Map(string ver, string mapname, Block[][] blockmap, long money,long starttime, long playtime, long time) {
		oldver = ver;
		this.ver = latestver;
		this.mapname = mapname;
		this.blockmap = blockmap;
		this.money = money;
		this.starttime = starttime;
		this.playtime = playtime;
		this.time = time;
	}*/
}
