using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class PlantObject : MapObject {
	public const string KEY_AGE = "AGE";

	public float age;

	public PlantObject (Map map, Vector3 pos, Quaternion rot, float age = 0) : base (map, pos, rot) {
		this.age = age;
	}

	protected PlantObject (SerializationInfo info, StreamingContext context) : base (info, context) {
		age = info.GetSingle (KEY_AGE);
	}

	public override void GetObjectData (SerializationInfo info, StreamingContext context) {
		base.GetObjectData (info, context);
		info.AddValue (KEY_AGE, age);
	}

	public override void generate () {
		if (entity == null)
			(entity = new GameObject ("plant-" + getChunkX () + "," + getChunkZ ()).AddComponent<MapEntity> ()).init (this);
		else
			reloadEntity ();
	}

	public override void TimePasses (long ticks) {
		age += ticks / 31557600000f;
	}
}
