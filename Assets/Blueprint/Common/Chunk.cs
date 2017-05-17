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
	public int y { get; }
	[NonSerialized]
	public Mesh mesh;
	public SerializableMesh sMesh { get { return new SerializableMesh (mesh); } }

	public Chunk (Map map, int x, int y) {
		this.map = map;
		this.x = x;
		this.y = y;
	}

	public IEnumerator generate (MonoBehaviour behaviour) {
		if (obj == null) {
			obj = new GameObject ();
			obj.AddComponent<MeshFilter> ();
			obj.AddComponent<MeshRenderer> ();
			obj.AddComponent<MeshCollider> ();

			obj.transform.position = new Vector3 (x * size, 0, y * size);
		}

		obj.GetComponent<MeshRenderer> ().material = mat;
		yield return null;

		if (mesh == null) {
			List<Vector3> points = new List<Vector3> ();
			for (int x2 = x - 1; x2 <= x + 1; x2++) {
				for (int y2 = y - 1; y2 <= y + 1; y2++) {
					if (x2 != x && y2 != y) {
						int a = map.getChunk (x2, y2);
						if (a != -1 && map.chunks [a].mesh != null) {
							List<Vector3> verts1 = new List<Vector3> (map.chunks [a].mesh.vertices);
							for (int b = 0; b < verts1.Count;) {
								if (verts1 [b].x != 0 && verts1 [b].z != 0 && verts1 [b].x != size && verts1 [b].z != size) {
									verts1.RemoveAt (b);
								} else {
									b++;
								}
							}
							Vector3[] verts2 = verts1.ToArray ();
							BPMesh.scale (ref verts2, Vector3.one / size);
							BPMesh.move (ref verts2, Vector3.right * (map.chunks [a].x - x) + Vector3.forward * (map.chunks [a].y - y));
							points.AddRange (verts2);
						}
					}
				}
			}

			IEnumerator routine = BPMesh.getBPFractalTerrain (2, points.ToArray ());
			yield return behaviour.StartCoroutine (routine);
			if (routine.Current is Mesh) {
				mesh = (Mesh)routine.Current;
			}

			Vector3[] verts3 = mesh.vertices;
			BPMesh.scale (ref verts3, Vector3.one * size);
			mesh.vertices = verts3;
			BPMesh.recalc (mesh);
			yield return null;
		}

		MeshFilter meshfilter = obj.GetComponent<MeshFilter> ();
		meshfilter.sharedMesh = mesh;

		//TODO [Physics.PhysX] Cooking::cook/createTriangleMesh: user-provided triangle mesh descriptor is invalid!
		//TODO [Physics.PhysX] cleaning the mesh failed
		obj.GetComponent<MeshCollider> ().sharedMesh = meshfilter.sharedMesh;
	}
}
