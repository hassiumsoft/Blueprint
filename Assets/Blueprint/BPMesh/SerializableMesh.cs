using System;
using UnityEngine;

[Serializable]
public class SerializableMesh {
	public Matrix4x4[] bindposes;
	//TODO BlendShape
	public BoneWeight[] boneWeights;
	//TODO
	//public Bounds bounds;
	public Color[] colors;
	public Color32[] colors32;
	public SerializableVector3[] normals;
	public int subMeshCount;
	public SerializableVector4[] tangents;
	public int[] triangles;
	public SerializableVector2[] uv;
	public SerializableVector2[] uv2;
	public SerializableVector2[] uv3;
	public SerializableVector2[] uv4;
	public SerializableVector3[] vertices;

	//Object
	public HideFlags hideFlags;
	public string name;

	public SerializableMesh (Mesh mesh) {
		bindposes = mesh.bindposes;
		boneWeights = mesh.boneWeights;
		//bounds = mesh.bounds;
		colors = mesh.colors;
		colors32 = mesh.colors32;

		SerializableVector3[] a = new SerializableVector3[mesh.vertices.Length];
		for (int b = 0; b < a.Length; b++) {
			a [b] = new SerializableVector3 (mesh.vertices [b]);
		}
		vertices = a;

		a = new SerializableVector3[mesh.normals.Length];
		for (int b = 0; b < a.Length; b++) {
			a [b] = new SerializableVector3 (mesh.normals [b]);
		}
		normals = a;

		subMeshCount = mesh.subMeshCount;

		SerializableVector4[] c = new SerializableVector4[mesh.tangents.Length];
		for (int b = 0; b < c.Length; b++) {
			c [b] = new SerializableVector4 (mesh.tangents [b]);
		}
		tangents = c;

		triangles = mesh.triangles;

		SerializableVector2[] d = new SerializableVector2[mesh.uv.Length];
		for (int b = 0; b < d.Length; b++) {
			d [b] = new SerializableVector2 (mesh.uv [b]);
		}
		uv = d;

		d = new SerializableVector2[mesh.uv2.Length];
		for (int b = 0; b < d.Length; b++) {
			d [b] = new SerializableVector2 (mesh.uv2 [b]);
		}
		uv2 = d;

		d = new SerializableVector2[mesh.uv3.Length];
		for (int b = 0; b < d.Length; b++) {
			d [b] = new SerializableVector2 (mesh.uv3 [b]);
		}
		uv3 = d;

		d = new SerializableVector2[mesh.uv4.Length];
		for (int b = 0; b < d.Length; b++) {
			d [b] = new SerializableVector2 (mesh.uv4 [b]);
		}
		uv4 = d;

		//Object
		hideFlags = mesh.hideFlags;
		name = mesh.name;
	}

	public Mesh toMesh () {
		Mesh mesh = new Mesh ();

		mesh.bindposes = bindposes;
		mesh.boneWeights = boneWeights;
		//mesh.bounds = bounds;
		mesh.colors = colors;
		mesh.colors32 = colors32;

		Vector3[] a = new Vector3[vertices.Length];
		for (int b = 0; b < a.Length; b++) {
			a [b] = vertices [b].toVector3 ();
		}
		mesh.vertices = a;

		a = new Vector3[normals.Length];
		for (int b = 0; b < a.Length; b++) {
			a [b] = normals [b].toVector3 ();
		}
		mesh.normals = a;

		mesh.subMeshCount = subMeshCount;
		mesh.triangles = triangles;

		Vector2[] c = new Vector2[uv.Length];
		for (int b = 0; b < c.Length; b++) {
			c [b] = uv [b].toVector2 ();
		}
		mesh.uv = c;

		c = new Vector2[uv2.Length];
		for (int b = 0; b < c.Length; b++) {
			c [b] = uv2 [b].toVector2 ();
		}
		mesh.uv2 = c;

		c = new Vector2[uv3.Length];
		for (int b = 0; b < c.Length; b++) {
			c [b] = uv3 [b].toVector2 ();
		}
		mesh.uv3 = c;

		c = new Vector2[uv4.Length];
		for (int b = 0; b < c.Length; b++) {
			c [b] = uv4 [b].toVector2 ();
		}
		mesh.uv4 = c;

		Vector4[] d = new Vector4[tangents.Length];
		for (int b = 0; b < d.Length; b++) {
			d [b] = tangents [b].toVector4 ();
		}
		mesh.tangents = d;

		mesh.hideFlags = hideFlags;
		mesh.name = name;

		return mesh;
	}
}
