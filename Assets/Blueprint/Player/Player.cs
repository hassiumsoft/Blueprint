using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Player : MapObject {
	public const string KEY_NAME = "NAME";

	//TODO スキン別にPrefabを作成
	public static PlayerEntity playerPrefab;

	//描画されるチャンクの範囲はプレイヤーを中心にした正方形である。大きさは描画距離(Main.drawDistance)となる。

	public PlayerEntity playerEntity { get { return entity as PlayerEntity; } }

	public string name; //TODO プレイヤー名(仮)
	public long playtime; //プレイ時間

	public Player (Map map, string name) : base (map, map.getPlayerSpawnPoint ()) {
		this.name = name;
	}

	protected Player (SerializationInfo info, StreamingContext context) : base (info, context) {
		name = info.GetString (KEY_NAME);
	}

	public override void GetObjectData (SerializationInfo info, StreamingContext context) {
		base.GetObjectData (info, context);
		info.AddValue (KEY_NAME, name);
	}

	public override void generate () {
		if (entity == null)
			(entity = GameObject.Instantiate (playerPrefab)).init (this);
		else
			reloadEntity ();
	}

	public void respawn () {
		teleport (chunk.map.getPlayerSpawnPoint ());
	}
}
