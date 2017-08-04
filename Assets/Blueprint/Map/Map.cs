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
	public const string KEY_SPAWNPOINT = "SPAWNPOINT";
	public const string KEY_SPAWNRADIUS = "SPAWNRADIUS";
	public const float ABYSS_HEIGHT = -100f;

	//・複数のマップを同時に読み込んではいけない。

	public static Vector3 DEFAULT_SPAWN = Vector3.zero;

	public string mapname { get; }

	//TODO マップの作成日時を読み込むにはマップを読み込まなければいけないため、
	//マップのチャンクやプレイヤーデータを除いた基本情報のみを読み込んだり出来るようにする。
	public DateTime created { get; }
	public List<Chunk> chunks; //TODO 後にチャンク呼び出しが遅くなる可能性があるためMapなどで高速化する必要がある
	public List<Player> players;
	public long time { get; private set; } //マップの時間。0時から始まり1tickが1msである。
	public bool pause { get; private set; } //ポーズ中か
	public Vector3 spawnPoint;
	public float spawnRadius;

	//TODO マップに変更があるかどうかの判定（自動セーブ用）

	public Map (string mapname) {
		this.mapname = mapname;
		created = DateTime.Now;
		chunks = new List<Chunk> ();
		players = new List<Player> ();
		time = 6 * 60 * 60000; //朝6時からスタート
		spawnPoint = DEFAULT_SPAWN;
		spawnRadius = Chunk.size / 2;
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
		spawnPoint = ((SerializableVector3)info.GetValue (KEY_SPAWNPOINT, typeof(SerializableVector3))).toVector3 ();
		spawnRadius = info.GetSingle (KEY_SPAWNRADIUS);
	}

	public virtual void GetObjectData (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		info.AddValue (KEY_MAPNAME, mapname);
		info.AddValue (KEY_CREATED, created.Ticks);
		info.AddValue (KEY_CHUNKS, chunks);
		info.AddValue (KEY_PLAYERS, players);
		info.AddValue (KEY_TIME, time);
		info.AddValue (KEY_SPAWNPOINT, new SerializableVector3 (spawnPoint));
		info.AddValue (KEY_SPAWNRADIUS, spawnRadius);
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
		float x = spawnPoint.x + UnityEngine.Random.Range (-spawnRadius, spawnRadius);
		float z = spawnPoint.z + UnityEngine.Random.Range (-spawnRadius, spawnRadius);
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
		if (chunk.generateChunk ()) {
			GameObject obj = new GameObject ("terrain-" + x + "," + z);
			obj.transform.position = new Vector3 (chunk.x * Chunk.size, -32768f, chunk.z * Chunk.size);
			MeshFilter meshfilter = obj.AddComponent<MeshFilter> ();
			MeshCollider meshcollider = obj.AddComponent<MeshCollider> ();
			meshcollider.sharedMesh = meshfilter.sharedMesh = chunk.mesh;

			RaycastHit hit;
			if (Physics.Raycast (new Ray (new Vector3 (x, int.MaxValue / 2, z), Vector3.down), out hit, int.MaxValue)) {
				r = hit.point.y + 32768f;
			}

			GameObject.Destroy (obj);
		}

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
