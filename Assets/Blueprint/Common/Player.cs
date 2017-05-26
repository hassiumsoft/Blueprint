using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Player : ISerializable {
	public const string KEY_NAME = "NAME";
	public const string KEY_POS = "POS";

	public PlayerObject playerPrefab;
	public PlayerObject obj;

	public string name;
	public Map map;
	public Vector3 pos;

	public Player (Map map, string name) {
		this.map = map;
		this.name = name;

		pos = new Vector3 ();
		respawn ();
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
		if (obj != null) {
			pos = obj.transform.position;
		}
		info.AddValue (KEY_POS, new SerializableVector3 (pos));
	}

	public IEnumerator generate (MonoBehaviour behaviour) {
		if (obj == null) {
			yield return null;//TODO 仮

			(obj = GameObject.Instantiate (playerPrefab)).init (this);

			obj.transform.position = pos;
		}
	}

	public void respawn () {
		pos = map.getPlayerSpawnPoint ();
		if (obj != null) {
			obj.transform.position = pos;
		}
		Debug.Log (DateTime.Now + " プレイヤー\"" + name + "\"がリスポーン: " + pos);
	}
}
