using UnityEngine;

public class ChunkEntity : MonoBehaviour {
	Chunk chunk;
	bool initialized = false;

	void Start () {
		gameObject.AddComponent<MeshFilter> ();
		gameObject.AddComponent<MeshRenderer> ().material = Main.main.mat; //TODO 一時的。（Main.csも確認）
		gameObject.AddComponent<MeshCollider> ();
		//gameObject.AddComponent<MeshCollider> ().convex = true;
		/*BoxCollider box = gameObject.AddComponent<BoxCollider> ();
			box.center = new Vector3 (Chunk.size / 2, -0.5f, Chunk.size / 2);
			box.size = new Vector3 (Chunk.size, 1, Chunk.size);*/

		gameObject.transform.position = new Vector3 (chunk.x * Chunk.size, 0, chunk.z * Chunk.size);

		MeshFilter meshfilter = gameObject.GetComponent<MeshFilter> ();
		meshfilter.sharedMesh = chunk.mesh;

		gameObject.GetComponent<MeshCollider> ().sharedMesh = meshfilter.sharedMesh;
	}

	void Update () {
		
	}

	public void init (Chunk chunk) {
		if (initialized)
			return;
		this.chunk = chunk;

		initialized = true;
	}

	public void Destroy () {
		Destroy (gameObject);
	}
}
