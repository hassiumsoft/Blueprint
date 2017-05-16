using System;
using UnityEngine;

[Serializable]
public class Chunk {
	public Material mat;
	public GameObject obj;

	public Map map { get; }
	public int x { get; }
	public int y { get; }

	public Chunk (Map map, int x, int y) {
		this.map = map;
		this.x = x;
		this.y = y;
	}

	//TODO チャンクの生成は時間がかかるためゲームプレイに影響する。
	public void generate () {
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
