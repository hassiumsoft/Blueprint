using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class MapObject : ISerializable {
	public const string KEY_MAP = "MAP";
	public const string KEY_ID = "ID";
	public const string KEY_POS = "POS";

	public MapEntity obj { get; private set; }

	public Map map { get; private set; }
	//public int id;
	public Vector3 pos { get; private set; }

	//TODO バウンディングボックス

	public MapObject (Map map/*, int id*/, Vector3 pos) {
		this.map = map;
		//this.id = id;
		this.pos = pos;
	}

	protected MapObject (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		map = (Map)info.GetValue (KEY_MAP, typeof(Map));
		//id = info.GetInt32 (KEY_ID);
		pos = ((SerializableVector3)info.GetValue (KEY_POS, typeof(SerializableVector3))).toVector3 ();
	}

	public virtual void GetObjectData (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		info.AddValue (KEY_MAP, map);
		//info.AddValue (KEY_ID, id);
		if (obj != null) {
			pos = obj.transform.position;
		}
		info.AddValue (KEY_POS, new SerializableVector3 (pos));
	}

	//TODO
	/*public void moveToChunk (Chunk chunk) {
		this.chunk = chunk;
	}*/

	public void generate () {
		if (obj == null)
			(obj = new MapEntity ()).init (this);

		if (obj == null)
			(obj = new GameObject ("mapobj-" + getChunkX () + "," + getChunkZ ()).AddComponent<MapEntity> ()).init (this);
		else
			obj.reload ();
	}

	public int getChunkX () {
		//return obj == null ? Map.getChunkX (pos.x) : obj.getChunkX (); 動く場合
		return Map.getChunkX (pos.x);
	}

	public int getChunkZ () {
		//return obj == null ? Map.getChunkZ (pos.z) : obj.getChunkZ (); 動く場合
		return Map.getChunkZ (pos.z);
	}

	//時間が経過するメソッド。ticksには経過時間を指定。
	public void TimePasses (long ticks) {
		//TODO
	}
}
