using System;
using UnityEngine;

[Serializable]
public class Chunk {
	public Material mat;
	public GameObject obj;

	public Chunk () {
		generateMesh ();
	}

	public void generateMesh () {
		if (obj == null) {
			obj = new GameObject ();
			obj.AddComponent<MeshFilter> ();
			obj.AddComponent<MeshRenderer> ();
			obj.AddComponent<MeshCollider> ();
		}
		MeshFilter meshfilter = obj.GetComponent<MeshFilter> ();

		Mesh mesh = BPMesh.getBPFractalTerrain (4, 0.5f);

		meshfilter.sharedMesh = mesh;

		obj.GetComponent<MeshRenderer> ().material = mat;
		obj.GetComponent<MeshCollider> ().sharedMesh = meshfilter.sharedMesh;
	}
}
