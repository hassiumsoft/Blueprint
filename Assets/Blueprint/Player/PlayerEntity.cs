using System;
using System.Collections;
using UnityEngine;

public class PlayerEntity : MonoBehaviour {
	public static Vector3 CAMERA_POS = new Vector3 (0f, 2f, -2.5f);
	public static Vector3 CAMERA_ANGLE = new Vector3 (15f, 0f, 0f);

	Player player;
	bool initialized = false;
	Camera p_camera;
	Vector3 lastPos;

	void Start () {
		lastPos = transform.position;
	}
	
	void Update () {
		if ((int)lastPos.x / Chunk.size != transform.position.x / Chunk.size || (int)lastPos.z / Chunk.size != transform.position.z / Chunk.size) {
			reloadChunk (false);
		}

		//TODO メニューを開いていない状態でのみ操作できるようにする
		if (Input.GetMouseButtonDown (0)) {
			Vector3 pos = p_camera.ViewportToWorldPoint (Input.mousePosition);
			Chunk chunk = player.map.chunks [player.map.getChunk ((int)transform.position.x / Chunk.size, (int)transform.position.z / Chunk.size)];
			MapObject mapobj = new MapObject (chunk, pos);
			chunk.objs.Add (mapobj);
			mapobj.generate (Main.main);
		}
		if (transform.position.y < Map.ABYSS_HEIGHT) {
			print (DateTime.Now + " プレイヤー\"" + player.name + "\"が奈落に落ちました");
			if (player == null) {
				Destroy (gameObject);
			} else {
				player.respawn ();
			}
		}

		lastPos = transform.position;
	}

	void OnDestroy () {
		p_camera.transform.SetParent (null);//TODO Can't destroy Transform component of 'Main Camera'. If you want to destroy the game object, please call 'Destroy' on the game object instead. Destroying the transform component is not allowed.
		player.pos = transform.position;
	}

	public void init (Player player) {
		this.player = player;

		transform.position = player.pos;

		(p_camera = FindObjectOfType<Camera> ()).transform.SetParent (this.transform);
		p_camera.transform.localPosition = CAMERA_POS;
		p_camera.transform.localEulerAngles = CAMERA_ANGLE;

		reloadChunk (true);
		transform.position = player.pos;

		initialized = true;
	}

	public void reloadChunk (bool pause) {
		StartCoroutine (a (pause));
	}

	private IEnumerator a (bool pause) {
		for (int x = -Main.drawDistance; x < Main.drawDistance; x++) {
			for (int z = -Main.drawDistance; z < Main.drawDistance; z++) {
				if (player.map.getChunk ((int)transform.position.x / Chunk.size + x, (int)transform.position.z / Chunk.size + z) == -1) {
					Chunk chunk = new Chunk (player.map, (int)transform.position.x / Chunk.size + x, (int)transform.position.z / Chunk.size + z);
					player.map.chunks.Add (chunk);
					yield return StartCoroutine (chunk.generate (this, pause));
				}
			}
		}
	}
}
