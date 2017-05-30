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
	public Vector3 _pos;
	public Vector3 pos { get { return obj == null ? _pos : obj.transform.position; } set { teleport (_pos = value); } }

	public Player (Map map, string name) {
		this.map = map;
		this.name = name;

		_pos = new Vector3 ();
		respawn ();
	}

	protected Player (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		name = info.GetString (KEY_NAME);
		_pos = ((SerializableVector3)info.GetValue (KEY_POS, typeof(SerializableVector3))).toVector3 ();
	}

	public virtual void GetObjectData (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		info.AddValue (KEY_NAME, name);
		info.AddValue (KEY_POS, new SerializableVector3 (pos));
	}

	public IEnumerator generate (MonoBehaviour behaviour) {
		yield return null;//TODO 一時的
		if (obj == null) {
			(obj = GameObject.Instantiate (playerPrefab)).init (this);
		}
	}

	public void respawn () {
		teleport (map.getPlayerSpawnPoint ());
		Debug.Log (DateTime.Now + " プレイヤー\"" + name + "\"がリスポーン: " + pos);
	}

	public void teleport (Vector3 pos) {
		_pos = pos;
		if (obj != null) {
			obj.transform.position = pos;
		}
	}
}
