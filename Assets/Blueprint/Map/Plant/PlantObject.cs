using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class PlantObject : MapObject {
	public const string KEY_AGE = "AGE";

	public float age;

	public PlantObject (Map map, Vector3 pos, Quaternion rot) : base (map, pos, rot) {
		age = 0;
	}

	protected PlantObject (SerializationInfo info, StreamingContext context) : base (info, context) {
		age = info.GetSingle (KEY_AGE);
	}

	public override void GetObjectData (SerializationInfo info, StreamingContext context) {
		base.GetObjectData (info, context);
		info.AddValue (KEY_AGE, age);
	}
}
