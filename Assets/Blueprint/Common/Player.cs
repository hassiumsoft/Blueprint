using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Player : ISerializable {
	public const string KEY_NAME = "NAME";
	public const string KEY_POS = "POS";

	public GameObject playerPrefab;
	public GameObject obj;

	public string name;
	public Map map;
	public Vector3 pos;

	public Player (Map map, string name) {
		this.map = map;
		this.name = name;

		pos = map.getPlayerSpawnPoint ();
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
			obj = GameObject.Instantiate (playerPrefab);

			obj.transform.position = pos;
		}
		yield return null;//TODO 仮
	}
}
