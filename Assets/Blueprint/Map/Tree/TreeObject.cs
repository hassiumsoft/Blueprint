using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class TreeObject : MapObject {
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

	public TreeObject (Map map, Vector3 pos, Quaternion rot) : base (map, pos, rot) {
	}

	protected TreeObject (SerializationInfo info, StreamingContext context) : base (info, context) {
		//name = info.GetString (KEY_NAME);
	}

	public override void GetObjectData (SerializationInfo info, StreamingContext context) {
		base.GetObjectData (info, context);
		//info.AddValue (KEY_NAME, name);
	}

	public override void generate () {
		if (entity == null)
			(entity = new GameObject ("tree-" + getChunkX () + "," + getChunkZ ()).AddComponent<MapEntity> ()).init (this);
		else
			reloadEntity ();
	}

	public override void reloadEntity () {
		if (entity == null)
			return;
		base.reloadEntity ();
	}
}
