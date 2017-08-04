using UnityEngine;

public class MapEntity : MonoBehaviour {
	public MapObject obj { get; private set; }
	bool initialized = false;

	void Start () {
		reload ();
	}

	void Update () {
		//TODO 移動する物体は移動したときにチャンクを移動するようにする
		//obj.moveToChunk(chunk);
	}

	public void init (MapObject obj) {
		//if (initialized)
		//	return;
		this.obj = obj;

		initialized = true;
	}

	public void reload () {
		if (gameObject == null || obj == null)
			return;
		transform.position = obj.pos;
	}

	public int getChunkX () {
		return Map.getChunkX (transform.position.x);
	}

	public int getChunkZ () {
		return Map.getChunkZ (transform.position.z);
	}

	public void Destroy () {
		Destroy (gameObject);
	}
}
