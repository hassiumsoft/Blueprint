using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Map : ISerializable {
	public const string KEY_MAPNAME = "MAPNAME";
	public const string KEY_CREATED = "CREATED";
	public const string KEY_CHUNKS = "CHUNKS";
	public const string KEY_PLAYERS = "PLAYERS";
	public const string KEY_TIME = "TIME";
	public const float ABYSS_HEIGHT = -100f;

	//・複数のマップを同時に読み込んではいけない。

	public string mapname { get; }

	//TODO マップの作成日時を読み込むにはマップを読み込まなければいけないため、
	//マップのチャンクやプレイヤーデータを除いた基本情報のみを読み込んだり出来るようにする。
	public DateTime created { get; }
	public List<Chunk> chunks; //TODO 後にチャンク呼び出しが遅くなる可能性があるためMapなどで高速化する必要がある
	public List<Player> players;
	public long time { get; private set; } //マップの時間
	public bool pause { get; private set; } //ポーズ中か

	//TODO マップのプレイ時間->プレイヤー毎に記録
	//public long playtime;

	//TODO マップに変更があるかどうかの判定（自動セーブ用）

	public Map (string mapname) {
		this.mapname = mapname;
		created = DateTime.Now;
		chunks = new List<Chunk> ();
		players = new List<Player> ();
		time = 0;
	}

	protected Map (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		mapname = info.GetString (KEY_MAPNAME);
		created = new DateTime (info.GetInt64 (KEY_CREATED));
		chunks = (List<Chunk>)info.GetValue (KEY_CHUNKS, typeof(List<Chunk>));
		for (int a = 0; a < chunks.Count; a++)
			chunks [a].map = this;
		players = (List<Player>)info.GetValue (KEY_PLAYERS, typeof(List<Player>));
		for (int a = 0; a < players.Count; a++)
			players [a].map = this;
		time = info.GetInt64 (KEY_TIME);
	}

	public virtual void GetObjectData (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		info.AddValue (KEY_MAPNAME, mapname);
		info.AddValue (KEY_CREATED, created.Ticks);
		info.AddValue (KEY_CHUNKS, chunks);
		info.AddValue (KEY_PLAYERS, players);
		info.AddValue (KEY_TIME, time);
	}

	public int getChunkIndex (int chunkx, int chunkz) {
		for (int n = 0; n < chunks.Count; n++) {
			if (chunks [n].x == chunkx && chunks [n].z == chunkz) {
				return n;
			}
		}
		return -1;
	}

	public Chunk getChunk (int chunkx, int chunkz) {
		for (int n = 0; n < chunks.Count; n++) {
			if (chunks [n].x == chunkx && chunks [n].z == chunkz) {
				return chunks [n];
			}
		}
		Chunk chunk = new Chunk (this, chunkx, chunkz);
		chunks.Add (chunk);
		return chunk;
	}

	public int getPlayer (string name) {
		for (int n = 0; n < players.Count; n++) {
			if (players [n].name.ToLower ().Equals (name.ToLower ())) {
				return n;
			}
		}
		return -1;
	}

	public Vector3 getPlayerSpawnPoint () {
		//TODO

		return new Vector3 (0, getTerrainHeight (0, 0), 0);
	}

	public float getHeight (float x, float z) {
		RaycastHit hit;
		if (Physics.Raycast (new Ray (new Vector3(x, int.MaxValue / 2, z), Vector3.down), out hit, int.MaxValue)) {
			return hit.point.y;
		}

		return ABYSS_HEIGHT;
	}

	public float getTerrainHeight (float x, float z) {
		//TODO 障害物に当たらず地形の標高を取得できるようにする

		return getHeight (x, z);
	}

	public void DestroyPlayerEntities () {
		foreach (PlayerEntity player in GameObject.FindObjectsOfType<PlayerEntity> ()) {
			player.Destroy ();
		}
	}

	public void DestroyChunkEntities () {
		foreach (ChunkEntity chunk in GameObject.FindObjectsOfType<ChunkEntity> ()) {
			chunk.Destroy ();
		}
	}

	public void DestroyAll () {
		DestroyPlayerEntities ();
		DestroyChunkEntities ();
	}

	public void Pause () {
		pause = true;
	}

	public void Resume () {
		pause = false;
	}

	//時間が経過するメソッド。ticksには経過時間を指定。
	public void TimePasses (long ticks) {
		time += ticks;
	}
}
