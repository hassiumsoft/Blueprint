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

	void Start () {
		lastPos = transform.position;
	}
	
	void Update () {
		if (Mathf.FloorToInt (lastPos.x / Chunk.size) != getChunkX () || Mathf.FloorToInt (lastPos.z / Chunk.size) != getChunkZ ()) {
			reloadChunk ();
			//reloadChunk (false); TODO
		}

		if (!BPCanvas.pausePanel.isShowing ()) {
			if (Input.GetMouseButtonDown (0)) {
				//TODO

				Vector3 pos = p_camera.ViewportToWorldPoint (Input.mousePosition);
				Chunk chunk = player.map.getChunk ((int)transform.position.x / Chunk.size, (int)transform.position.z / Chunk.size);
				MapObject mapobj = new MapObject (chunk, pos);
				chunk.objs.Add (mapobj);
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

		(p_camera = FindObjectOfType<Camera> ()).transform.SetParent (this.transform);
		p_camera.transform.localPosition = CAMERA_POS;
		p_camera.transform.localEulerAngles = CAMERA_ANGLE;

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
		return Mathf.FloorToInt (transform.position.x / Chunk.size);
	}

	public int getChunkZ () {
		return Mathf.FloorToInt (transform.position.z / Chunk.size);
	}

	public void Destroy () {
		p_camera.transform.SetParent (null);
		Destroy (gameObject);
	}
}
