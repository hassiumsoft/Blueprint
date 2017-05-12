using System;
using UnityEngine;

[Serializable]
public class SerializableVector3 {
	float x;
	float y;
	float z;

	public SerializableVector3 (Vector3 v) {
		x = v.x;
		y = v.y;
		z = v.z;
	}

	public Vector3 toVector3 () {
		return new Vector3 (x, y, z);
	}
}
