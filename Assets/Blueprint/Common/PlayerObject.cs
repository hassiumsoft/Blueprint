using System;
using UnityEngine;

public class PlayerObject : MonoBehaviour {
	public static Vector3 CAMERA_POS = new Vector3 (0f, 2f, -2.5f);
	public static Vector3 CAMERA_ANGLE = new Vector3 (15f, 0f, 0f);

	Player player;
	bool initialized = false;
	Camera p_camera;

	void Start () {
		
	}
	
	void Update () {
		if (transform.position.y < -10000f) {
			print (DateTime.Now + " プレイヤー\"" + player.name + "\"が奈落に落ちました");
			if (player == null) {
				Destroy (gameObject);
			} else {
				player.respawn ();
			}
		}
	}

	void OnDestroy () {
		p_camera.transform.SetParent (null);//TODO Can't destroy Transform component of 'Main Camera'. If you want to destroy the game object, please call 'Destroy' on the game object instead. Destroying the transform component is not allowed.
	}

	public void init (Player player) {
		this.player = player;

		(p_camera = FindObjectOfType<Camera> ()).transform.SetParent (this.transform);
		p_camera.transform.localPosition = CAMERA_POS;
		p_camera.transform.localEulerAngles = CAMERA_ANGLE;

		initialized = true;
	}
}
