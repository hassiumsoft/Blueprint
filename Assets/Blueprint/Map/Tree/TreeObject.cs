using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class TreeObject : MapObject {
	//TODO オブジェクト別にPrefabを作成
	//public static MapEntity objPrefab;

	//TODO
	//幹
	//枝
	//葉
	//実

	//葉緑体
	//水分
	//デンプン
	//光合成

	//肥料分

	//TODO 二酸化炭素の検出

	public TreeObject (Map map, Vector3 pos) : base (map, pos) {
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

	/*public void generate () {
		if (obj == null) {
			(obj = GameObject.Instantiate (objPrefab)).init (this);
		}
	}*/
}
