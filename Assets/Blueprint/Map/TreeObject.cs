using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class TreeObject : MapObject {
	//TODO オブジェクト別にPrefabを作成
	public static MapEntity objPrefab;

	public TreeObject (Chunk chunk, Vector3 pos) : base (chunk, pos) {
	}

	/*protected TreeObject (SerializationInfo info, StreamingContext context) {
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
	}*/

	public void generate () {
		if (obj == null) {
			(obj = GameObject.Instantiate (objPrefab)).init (this);
		}
	}
}
