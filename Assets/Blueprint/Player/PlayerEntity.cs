using System;
using System.Collections;
using UnityEngine;

public class PlayerEntity : MapEntity {
	public static Vector3 CAMERA_POS = new Vector3 (0f, 2f, -2.5f);
	public static Vector3 CAMERA_ANGLE = new Vector3 (15f, 0f, 0f);

	public Player player { get { return obj as Player; } }

	Camera p_camera;
	Vector3 lastPos;
	Quaternion lastRot;

	void Start () {
		obj.reloadEntity ();
		lastPos = transform.position;
		lastRot = transform.rotation;

		(p_camera = Main.main.mainCamera).transform.SetParent (this.transform);
		p_camera.transform.localPosition = CAMERA_POS;
		p_camera.transform.localEulerAngles = CAMERA_ANGLE;
		p_camera.clearFlags = CameraClearFlags.Skybox;

		reloadChunk ();
	}
	
	void Update () {
		if (Map.getChunkX (lastPos.x) != obj.getChunkX () || Map.getChunkZ (lastPos.z) != obj.getChunkZ ()) {
			reloadChunk ();
		}

		if (!BPCanvas.pausePanel.isShowing ()) {
			if (Input.GetMouseButtonDown (0)) {
				//TODO クリックすると目線の先に木を置くテスト用機能
				Vector3 pos = p_camera.ViewportToWorldPoint (Input.mousePosition);

				TreeObject mapobj = new TreeObject (obj.chunk.map, pos, transform.rotation);
				obj.chunk.map.addObject (mapobj);
				mapobj.generate ();
			}
		}
		if (transform.position.y < Map.ABYSS_HEIGHT) {
			print (DateTime.Now + " プレイヤー\"" + player.name + "\"が奈落に落ちました");
			if (obj == null) {
				Destroy (gameObject);
			} else {
				player.respawn ();
			}
		}

		lastPos = transform.position;
		lastRot = transform.rotation;
		obj.SyncFromEntity ();
		obj.moved ();
	}

	public override void init (MapObject obj) {
		base.init (obj);
		this.obj = obj as Player;
	}

	public override void Destroy () {
		p_camera.clearFlags = CameraClearFlags.SolidColor;
		p_camera.transform.SetParent (null);
		base.Destroy ();
	}

	public void reloadChunk () {
		int cx = obj.getChunkX ();
		int cz = obj.getChunkZ ();
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
						Chunk chunk = obj.chunk.map.getChunk (cx + x, cz + z);
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
}
