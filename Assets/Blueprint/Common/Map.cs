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

	public string mapname { get; }
	public DateTime created { get; }
	public List<Chunk> chunks;
	public List<Player> players;

	//public long starttime { private set; get; } //マップの開始時間
	//public long playtime; //マップのプレイ時間
	//public long time; //マップの時間
	//TODO 時間の概念は開発中

	public Map (string mapname) {
		this.mapname = mapname;
		created = DateTime.Now;
		chunks = new List<Chunk> ();
		players = new List<Player> ();
	}

	protected Map (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		mapname = info.GetString (KEY_MAPNAME);
		created = new DateTime (info.GetInt64 (KEY_CREATED));
		chunks = (List<Chunk>)info.GetValue (KEY_CHUNKS, typeof(List<Chunk>));
		for (int a = 0; a < chunks.Count; a++) {
			chunks [a].map = this;
		}
		players = (List<Player>)info.GetValue (KEY_PLAYERS, typeof(List<Player>));
		for (int a = 0; a < players.Count; a++) {
			players [a].map = this;
		}
	}

	public virtual void GetObjectData (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		info.AddValue (KEY_MAPNAME, mapname);
		info.AddValue (KEY_CREATED, created.Ticks);
		info.AddValue (KEY_CHUNKS, chunks);
		info.AddValue (KEY_PLAYERS, players);
	}

	public int getChunk (int x, int y) {
		for (int n = 0; n < chunks.Count; n++) {
			if (chunks [n].x == x && chunks [n].z == y) {
				return n;
			}
		}
		return -1;
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
		return new Vector3 (0, 1024, 0);
	}
}
