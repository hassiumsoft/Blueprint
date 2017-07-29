using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Player : ISerializable {
	public const string KEY_NAME = "NAME";
	public const string KEY_POS = "POS";

	//TODO スキン別にPrefabを作成
	public static PlayerEntity playerPrefab;

	public PlayerEntity obj;

	//描画されるチャンクの範囲はプレイヤーを中心にした正方形である。大きさは描画距離(Main.drawDistance)となる。

	public Map map;
	public string name;
	public Vector3 pos { get; private set; }

	public Player (Map map, string name) {
		this.map = map;
		this.name = name;

		/*for (int x = -1; x < 1; x++) {
			for (int z = -1; z < 1; z++) {
				Chunk chunk = map.getChunk (getChunkX () + x, getChunkZ () + z);
				chunk.generateChunk ();
			}
		}*/
		map.getChunk (getChunkX (), getChunkZ ()).generateChunk ();

		if (pos == null) {
			respawn ();
		}
	}

	protected Player (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		name = info.GetString (KEY_NAME);
		pos = ((SerializableVector3)info.GetValue (KEY_POS, typeof(SerializableVector3))).toVector3 ();
	}

	public virtual void GetObjectData (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		info.AddValue (KEY_NAME, name);
		info.AddValue (KEY_POS, new SerializableVector3 (pos));
	}

	public void generate () {
		if (obj == null) {
			(obj = GameObject.Instantiate (playerPrefab)).init (this);
		}
	}

	public void respawn () {
		teleport (map.getPlayerSpawnPoint ());
		Debug.Log (DateTime.Now + " プレイヤー\"" + name + "\"がリスポーン: " + pos);
	}

	public void teleport (Vector3 pos) {
		this.pos = pos;
		if (obj != null) {
			obj.transform.position = pos;
		}
	}

	public int getChunkX () {
		return obj == null ? Mathf.FloorToInt (pos.x / Chunk.size) : obj.getChunkX ();
	}

	public int getChunkZ () {
		return obj == null ? Mathf.FloorToInt (pos.z / Chunk.size) : obj.getChunkZ ();
	}

	public void SyncEntity () {
		if (obj != null) {
			pos = obj.transform.position;
		}
	}
}
