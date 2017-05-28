using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class MapObject {
	public const string KEY_ID = "ID";
	public const string KEY_POS = "POS";

	public static MapEntity objPrefab;
	public MapEntity obj;

	public Chunk chunk;
	//public int id;
	public Vector3 pos;

	public MapObject (Chunk chunk, int id, Vector3 pos) {
		this.chunk = chunk;
		//this.id = id;
		this.pos = pos;
	}

	protected MapObject (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		//id = info.GetInt32 (KEY_ID);
		pos = ((SerializableVector3)info.GetValue (KEY_POS, typeof(SerializableVector3))).toVector3 ();
	}

	public virtual void GetObjectData (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		//info.AddValue (KEY_ID, id);
		if (obj != null) {
			pos = obj.transform.position;
		}
		info.AddValue (KEY_POS, new SerializableVector3 (pos));
	}

	public void moveToChunk (Chunk chunk) {
		this.chunk = chunk;
	}

	public IEnumerator generate (MonoBehaviour behaviour) {
		if (obj == null) {
			yield return null;//TODO 仮

			(obj = GameObject.Instantiate (objPrefab)).init (this);

			obj.transform.position = pos;
		}
	}
}
