using System;
using UnityEngine;

[Serializable]
public class SerializableVector4 {
	float x;
	float y;
	float z;
	float w;

	public SerializableVector4 (Vector4 v) {
		x = v.x;
		y = v.y;
		z = v.z;
		w = v.w;
	}

	public Vector4 toVector4 () {
		return new Vector4 (x, y, z, w);
	}
}
