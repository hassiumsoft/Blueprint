using System;
using System.Collections;
using UnityEngine;

public class PlayerEntity : MonoBehaviour {
	public static Vector3 CAMERA_POS = new Vector3 (0f, 2f, -2.5f);
	public static Vector3 CAMERA_ANGLE = new Vector3 (15f, 0f, 0f);

	public Player player { get; private set; }
	bool initialized = false;
	Camera p_camera;
	Vector3 lastPos;
	Quaternion lastRot;

	void Start () {
		lastPos = transform.position;
		lastRot = transform.rotation;
	}
	
	void Update () {
		if (Map.getChunkX (lastPos.x) != getChunkX () || Map.getChunkZ (lastPos.z) != getChunkZ ()) {
			reloadChunk ();
			//reloadChunk (false); TODO
		}

		if (!BPCanvas.pausePanel.isShowing ()) {
			if (Input.GetMouseButtonDown (0)) {
				//TODO クリックすると目線の先に空のオブジェクトを置くテスト用機能
				Vector3 pos = p_camera.ViewportToWorldPoint (Input.mousePosition);

				MapObject mapobj = new MapObject (player.map, pos);
				player.map.addObject (mapobj);
				mapobj.generate ();
			}
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
		lastRot = transform.rotation;
		player.SyncEntity ();
	}

	void OnDestroy () {
		player.SyncEntity ();
		player.obj = null;
		p_camera.transform.SetParent (null);//TODO Can't destroy Transform component of 'Main Camera'. If you want to destroy the game object, please call 'Destroy' on the game object instead. Destroying the transform component is not allowed.
	}

	public void init (Player player) {
		if (initialized)
			return;
		this.player = player;

		lastPos = transform.position = player.pos;
		lastRot = transform.rotation = player.rot;

		(p_camera = FindObjectOfType<Camera> ()).transform.SetParent (this.transform);
		p_camera.transform.localPosition = CAMERA_POS;
		p_camera.transform.localEulerAngles = CAMERA_ANGLE;
		p_camera.clearFlags = CameraClearFlags.Skybox;

		reloadChunk ();

		initialized = true;
	}

	public void reloadChunk () {
		int cx = getChunkX ();
		int cz = getChunkZ ();
		foreach (ChunkEntity c in FindObjectsOfType<ChunkEntity> ()) {
			if (c.chunk.x < cx - Main.drawDistance || c.chunk.x > cx + Main.drawDistance ||
				c.chunk.z < cz - Main.drawDistance || c.chunk.z > cz + Main.drawDistance) {
				c.Destroy ();
			}
		}
		for (int a = 0; a <= Main.drawDistance; a++) {
			for (int x = -a; x <= a; x++) {
				for (int z = -a; z <= a; z++) {
					if (x == -a || x == a || z == -a || z == a) {
						Chunk chunk = player.map.getChunk (cx + x, cz + z);
						chunk.generateObj ();
						if (chunk.generated)
							continue;
						if (a == 0) {
							chunk.generate ();
						}
						if (chunk.generating)
							continue;
						
						Main.main.StartCoroutine (chunk.generateAsync ());
					}
				}
			}
		}
	}

	public int getChunkX () {
		return Map.getChunkX (transform.position.x);
	}

	public int getChunkZ () {
		return Map.getChunkZ (transform.position.z);
	}

	public void Destroy () {
		p_camera.clearFlags = CameraClearFlags.SolidColor;
		p_camera.transform.SetParent (null);
		Destroy (gameObject);
	}
}
