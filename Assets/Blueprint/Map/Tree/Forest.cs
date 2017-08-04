using System;
using UnityEngine;

[Serializable]
public class Forest {
	public int size = 20;
	public Map map;
	public SerializableVector3 pos;
	public SerializableVector3[] trees_pos;

	public Forest (Map map, Vector3 pos, Vector3[] trees_pos) {
		this.map = map;
		this.pos = new SerializableVector3 (pos);
		SerializableVector3[] a = new SerializableVector3[trees_pos.Length];
		for (int b = 0; b < a.Length; b++) {
			a [b] = new SerializableVector3 (trees_pos [b]);
		}
		this.trees_pos = a;
	}
}
