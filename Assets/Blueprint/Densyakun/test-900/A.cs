using UnityEngine;

public class A : MonoBehaviour {
	public Material mat;

	public void a () {
		GameObject obj = new GameObject ();
		MeshFilter meshfilter = obj.AddComponent<MeshFilter> ();

		Mesh mesh = BPMesh.getBPFractalTerrain (4, 0.5f);

		meshfilter.sharedMesh = mesh;

		obj.AddComponent<MeshRenderer> ().material = mat;
		obj.AddComponent<MeshCollider> ().sharedMesh = meshfilter.sharedMesh;
	}
}
