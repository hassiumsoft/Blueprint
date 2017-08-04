using UnityEngine;

public class ChunkEntity : MonoBehaviour {
	public Chunk chunk { get; private set; }
	bool initialized = false;

	void Start () {
		reload ();
	}

	void Update () {
		
	}

	public void init (Chunk chunk) {
		//if (initialized)
		//	return;
		this.chunk = chunk;

		initialized = true;
	}

	public void reload () {
		if (gameObject == null || chunk == null)
			return;
		transform.position = new Vector3 (chunk.x * Chunk.size, 0, chunk.z * Chunk.size);

		MeshFilter meshfilter = GetComponent<MeshFilter> ();
		MeshRenderer meshrenderer = GetComponent<MeshRenderer> ();
		MeshCollider meshcollider = GetComponent<MeshCollider> ();
		if (meshfilter == null)
			meshfilter = gameObject.AddComponent<MeshFilter> ();
		if (meshrenderer == null)
			meshrenderer = gameObject.AddComponent<MeshRenderer> ();
		if (meshcollider == null)
			meshcollider = gameObject.AddComponent<MeshCollider> ();

		meshrenderer.material = Main.main.mat; //TODO 一時的。（Main.csも確認）

		//meshcollider.convex = true;
		/*BoxCollider box = gameObject.AddComponent<BoxCollider> ();
			box.center = new Vector3 (Chunk.size / 2, -0.5f, Chunk.size / 2);
			box.size = new Vector3 (Chunk.size, 1, Chunk.size);*/
		
		meshcollider.sharedMesh = meshfilter.sharedMesh = chunk.mesh;
	}

	public void Destroy () {
		chunk.obj = null;

		foreach (MapObject a in chunk.objs) {
			//TODO 何故かnull Checkが必要
			if (a.obj != null) {
				a.obj.Destroy ();
			}
		}
		
		Destroy (gameObject);
	}
}
