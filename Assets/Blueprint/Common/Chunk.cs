using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Chunk : ISerializable {
	public const string KEY_X = "X";
	public const string KEY_Z = "Z";
	public const string KEY_MESH = "MESH";
	public const int size = 4096; //チャンクサイズ
	public const int height = 1024; //高低差の基準値（基準値よりズレが生じる場合がある）
	public const int fineness = 5;

	public Material mat;
	public GameObject obj;

	public Map map;
	public int x { get; }
	public int z { get; }
	public Mesh mesh;

	//public SerializableMaterial sMat;

	public Chunk (Map map, int x, int z) {
		this.map = map;
		this.x = x;
		this.z = z;
	}

	protected Chunk (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		x = info.GetInt32 (KEY_X);
		z = info.GetInt32 (KEY_Z);
		SerializableMesh sMesh = ((SerializableMesh)info.GetValue (KEY_MESH, typeof(SerializableMesh)));
		if (sMesh != null) {
			mesh = sMesh.toMesh ();
		}
	}

	public virtual void GetObjectData (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		info.AddValue (KEY_X, x);
		info.AddValue (KEY_Z, z);
		info.AddValue (KEY_MESH, mesh == null ? null : new SerializableMesh (mesh));
	}

	public IEnumerator generate (MonoBehaviour behaviour) {
		if (obj == null) {
			obj = new GameObject ();
			obj.AddComponent<MeshFilter> ();
			obj.AddComponent<MeshRenderer> ();
			obj.AddComponent<MeshCollider> ().convex = true;

			obj.transform.position = new Vector3 (x * size, 0, z * size);
		}

		obj.GetComponent<MeshRenderer> ().material = mat;
		yield return null;

		if (mesh == null) {
			List<Vector3> points = new List<Vector3> ();
			for (int x2 = x - 1; x2 <= x + 1; x2++) {
				for (int z2 = z - 1; z2 <= z + 1; z2++) {
					if (x2 != x || z2 != z) {
						int a = map.getChunk (x2, z2);
						if (a != -1) {
							Chunk chunk = map.chunks [a];
							if (chunk.mesh != null) {
								List<Vector3> verts1 = new List<Vector3> (chunk.mesh.vertices);
								for (int b = 0; b < verts1.Count;) {
									if (verts1 [b].x == 0 || verts1 [b].z == 0 || verts1 [b].x == size || verts1 [b].z == size) {
										b++;
									} else {
										verts1.RemoveAt (b);
									}
								}
								Vector3[] verts2 = verts1.ToArray ();
								BPMesh.scale (verts2, Vector3.one / size);
								for (int c = 0; c < verts2.Length; c++) {
									verts2 [c] += Vector3.right * (x2 - x) + Vector3.forward * (z2 - z);
								}
								points.AddRange (verts2);
							}
						}
					}
				}
			}

			IEnumerator routine = BPMesh.getBPFractalTerrain (fineness, points.ToArray ());
			yield return behaviour.StartCoroutine (routine);
			if (routine.Current is Mesh) {
				mesh = (Mesh)routine.Current;

				Vector3[] verts3 = mesh.vertices;
				BPMesh.scale (verts3, Vector3.right * size + Vector3.up * height + Vector3.forward * size);
				mesh.vertices = verts3;

				BPMesh.recalc (mesh);
			}
			yield return null;
		}

		MeshFilter meshfilter = obj.GetComponent<MeshFilter> ();
		meshfilter.sharedMesh = mesh;

		obj.GetComponent<MeshCollider> ().sharedMesh = meshfilter.sharedMesh;
	}
}
