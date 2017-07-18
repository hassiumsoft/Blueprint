using UnityEngine;

public class MapEntity : MonoBehaviour {
	MapObject obj;
	bool initialized = false;

	void Start () {

	}

	void Update () {
		//TODO 移動する物体は移動したときにチャンクを移動するようにする
		//obj.moveToChunk(chunk);

	}

	public void init (MapObject obj) {
		if (initialized)
			return;
		this.obj = obj;

		transform.position = obj.pos;

		initialized = true;
	}

	public void Destroy () {
		Destroy (gameObject);
	}
}
