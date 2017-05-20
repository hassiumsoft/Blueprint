using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Chunk {
	public const int size = 1024; //チャンクサイズ
	[NonSerialized]
	public Material mat;
	[NonSerialized]
	public GameObject obj;

	public Map map { get; }
	public int x { get; }
	public int z { get; }
	public SerializableMesh sMesh;
	[NonSerialized]
	public Mesh mesh;
	//public SerializableMaterial sMat;

	public Chunk (Map map, int x, int z) {
		this.map = map;
		this.x = x;
		this.z = z;
	}

	public IEnumerator generate (MonoBehaviour behaviour) {
		if (obj == null) {
			obj = new GameObject ();
			obj.AddComponent<MeshFilter> ();
			obj.AddComponent<MeshRenderer> ();
			obj.AddComponent<MeshCollider> ();

			obj.transform.position = new Vector3 (x * size, 0, z * size);
		}

		obj.GetComponent<MeshRenderer> ().material = mat;
		yield return null;

		if (sMesh != null) {
			mesh = sMesh.toMesh ();
		}

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
								BPMesh.scale (ref verts2, Vector3.one / size);
								for (int c = 0; c < verts2.Length; c++) {
									verts2 [c] += Vector3.right * (x2 - x) + Vector3.forward * (z2 - z);
								}
								points.AddRange (verts2);
							}
						}
					}
				}
			}

			IEnumerator routine = BPMesh.getBPFractalTerrain (4, points.ToArray ());
			yield return behaviour.StartCoroutine (routine);
			if (routine.Current is Mesh) {
				mesh = (Mesh)routine.Current;

				Vector3[] verts3 = mesh.vertices;
				BPMesh.scale (ref verts3, Vector3.one * size);
				mesh.vertices = verts3;

				BPMesh.recalc (ref mesh);

				sMesh = new SerializableMesh (mesh);
			}
			yield return null;
		}

		MeshFilter meshfilter = obj.GetComponent<MeshFilter> ();
		meshfilter.sharedMesh = mesh;

		obj.GetComponent<MeshCollider> ().sharedMesh = meshfilter.sharedMesh;
	}
}
