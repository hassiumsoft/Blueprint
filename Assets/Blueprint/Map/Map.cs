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

	//TODO マップ全体を読み込まなくてもファイルヘッダにマップの基本情報を書き込む。
	//マップの基本情報にはマップのチャンクやプレイヤーデータを除いたマップの作成日時などがある。
	public DateTime created { get; }
	public List<Chunk> chunks { get; private set; } //TODO 後にチャンク呼び出しが遅くなる可能性があるためHashMapなどで高速化する必要がある
	public List<Player> players { get; private set; }
	public long time { get; private set; } //マップの時間。0時から始まり1tickが1msである。
	public bool pause { get; private set; } //ポーズ中か

	//TODO マップに変更があるかどうかの判定（自動セーブ用）

	public Map (string mapname) {
		this.mapname = mapname;
		created = DateTime.Now;
		chunks = new List<Chunk> ();
		players = new List<Player> ();
		time = 6 * 60 * 60000; //朝6時からスタート
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
			players [a].chunk = getChunk (players [a].getChunkX (), players [a].getChunkZ ());
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

	public static int getChunkX (float x) {
		return Mathf.FloorToInt (x / Chunk.size);
	}

	public static int getChunkZ (float z) {
		return Mathf.FloorToInt (z / Chunk.size);
	}

	public void addObject (MapObject obj) {
		if (!(obj is Player))
			getChunk (getChunkX (obj.pos.x), getChunkZ (obj.pos.z)).objs.Add (obj);
	}

	public bool removeObject (MapObject obj) {
		return getChunk (getChunkX (obj.pos.x), getChunkZ (obj.pos.z)).objs.Remove (obj);
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
		Chunk chunk;
		if (chunks.Count > 0) {
			chunk = chunks [UnityEngine.Random.Range (0, chunks.Count)];
		} else {
			chunks.Add (chunk = new Chunk (this, 0, 0));
		}
		float x = chunk.x * Chunk.size + UnityEngine.Random.Range (0, (float)Chunk.size);
		float z = chunk.z * Chunk.size + UnityEngine.Random.Range (0, (float)Chunk.size);
		return new Vector3 (x, getTerrainHeight (x, z), z);
	}

	public float getHeight (float x, float z) {
		getChunk (Map.getChunkX (x), Map.getChunkZ (z)).generate ();

		RaycastHit hit;
		if (Physics.Raycast (new Ray (new Vector3(x, int.MaxValue / 2, z), Vector3.down), out hit, int.MaxValue)) {
			return hit.point.y;
		}

		return ABYSS_HEIGHT;
	}

	public float getTerrainHeight (float x, float z) {
		float r = ABYSS_HEIGHT;

		Chunk chunk = getChunk (Map.getChunkX (x), Map.getChunkZ (z));
		if (chunk.mesh == null)
			chunk.generateChunk ();

		float w = 32768f;

		GameObject obj = new GameObject ("terrain-" + x + "," + z);
		obj.transform.position = new Vector3 (chunk.x * Chunk.size, -w, chunk.z * Chunk.size);
		MeshFilter meshfilter = obj.AddComponent<MeshFilter> ();
		MeshCollider meshcollider = obj.AddComponent<MeshCollider> ();
		meshcollider.sharedMesh = meshfilter.sharedMesh = chunk.mesh;

		RaycastHit hit;
		if (Physics.Raycast (new Ray (new Vector3 (x, ABYSS_HEIGHT, z), Vector3.down), out hit, int.MaxValue)) {
			r = hit.point.y + w;
		}

		GameObject.Destroy (obj);

		return r;
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
		Chunk.stopAllGenerating ();
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

	public long getRawHours () {
		return Mathf.FloorToInt (getRawMinutes () / 60);
	}

	public long getRawMinutes () {
		return Mathf.FloorToInt (getRawSeconds () / 60);
	}

	public long getRawSeconds () {
		return Mathf.FloorToInt (time / 1000);
	}

	public long getDays () {
		return Mathf.FloorToInt (getRawHours () / 24);
	}

	public long getHours () {
		return getRawHours () - getDays () * 24;
	}

	public long getMinutes () {
		return getRawMinutes () - getRawHours () * 60;
	}

	public long getSeconds () {
		return getRawSeconds () - getRawMinutes () * 60;
	}

	public long getMilliSeconds () {
		return time - getRawSeconds () * 1000;
	}
}
