using UnityEngine;

public class MapEntity : MonoBehaviour {
	MapObject obj;
	bool initialized = false;
	Camera p_camera;

	void Start () {

	}

	void Update () {
		//TODO 移動する物体は移動したときにチャンクを移動するようにする
		//obj.moveToChunk(chunk);

	}

	public void init (MapObject obj) {
		this.obj = obj;

		transform.position = obj.pos;

		initialized = true;
	}
}
