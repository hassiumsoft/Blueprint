﻿using UnityEngine;

public class CameraMover : MonoBehaviour {
	public static Vector3 CAMERA_POS = new Vector3 (0f, 2f, -2.5f);
	public static Vector3 CAMERA_ANGLE = new Vector3 (15f, 0f, 0f);
	public static float maxDistance = 5f; //カメラの追跡が遅れたときに対象から離れない距離
	public static float min_t = 1f / 5f; //カメラの追跡力(0=動かない、1=瞬時に追跡)
	public static float free_time = 0.8f; //手動回転後に自動回転するまでの時間

	Transform target;
	Vector3 pos;
	Quaternion rot;
	float f = free_time;
	Vector3 lastMousePos = Vector3.zero;
	Vector3 rotStartEuler = Vector3.zero;
	bool a = true;
	float lv = 0f;

	//自分で視点を変えることが出来る。
	//カメラは後からついてくる挙動になっており、カメラが一定距離以上離れないようになっているため、乗り物向けなカメラになっている。
	void LateUpdate () {
		if (target == null) {
			if (Main.masterPlayer != null && Main.masterPlayer.playerEntity != null) {
				target = Main.masterPlayer.playerEntity.transform;
			}
		} else {
			bool c = Main.playingmap != null && !Main.pause; //操作可能か
			float h = Input.GetAxis ("Horizontal");
			float v = Input.GetAxis ("Vertical");

			if (c && Input.GetMouseButton (0)) {
				if (Input.GetMouseButtonDown (0)) {
					Cursor.lockState = CursorLockMode.Confined;
					lastMousePos = Input.mousePosition;
					rotStartEuler = rot.eulerAngles;
					a = false;
					f = 0;
				}
				Vector3 m = (Input.mousePosition - lastMousePos) * Main.dragRotSpeed; //カメラの移動量
				//x,y軸を「xをz、yをx、zをy、zを0にする」工程で交換しながら、カメラがひっくり返らないよう新しいx軸の範囲を固定
				m.z = m.x;
				m.y = -m.y;
				float rx = Mathf.Repeat (rotStartEuler.x - m.y + 180f, 360f) + m.y - 180f + CAMERA_ANGLE.x;
				m.x = Mathf.Clamp (rx + m.y, -60f, 60f) - rx;
				m.y = m.z;
				m.z = 0f;
				rot = Quaternion.Euler (rotStartEuler + m);
			} else {
				if (h == 0.0f && (lv < 0.0f) != (v < 0.0f)) {
					a = false;
					f = 0;
				} else {
					a = true;
				}
			}

			if (f >= free_time) {
				if (v < 0.0f) {
					rot = Quaternion.Euler (new Vector3 (target.rotation.eulerAngles.x, Mathf.Repeat (target.rotation.eulerAngles.y, 360f) - 180f, target.rotation.eulerAngles.z));
				} else {
					rot = target.rotation;
				}
			} else if (a) {
				f += Time.deltaTime;
			}

			pos = target.position + rot * CAMERA_POS;

			float x = pos.x - transform.position.x;
			float y = pos.y - transform.position.y;
			float z = pos.z - transform.position.z;
			float t = Mathf.Max (min_t, 1f - maxDistance / Mathf.Sqrt (x * x + y * y + z * z));
			transform.position = new Vector3 (Mathf.Lerp (transform.position.x, pos.x, t), Mathf.Lerp (transform.position.y, pos.y, t), Mathf.Lerp (transform.position.z, pos.z, t));
			transform.eulerAngles = new Vector3 (Mathf.Repeat (transform.eulerAngles.x + CAMERA_ANGLE.x + (Mathf.Repeat (rot.eulerAngles.x - transform.eulerAngles.x - CAMERA_ANGLE.x + 180f, 360f) - 180f) / 2 + 180f, 360f) - 180f,
				Mathf.Repeat (transform.eulerAngles.y + CAMERA_ANGLE.y + (Mathf.Repeat (rot.eulerAngles.y - transform.eulerAngles.y - CAMERA_ANGLE.y + 180f, 360f) - 180f) / 2 + 180f, 360f) - 180f,
				Mathf.Repeat (transform.eulerAngles.z + CAMERA_ANGLE.z + (Mathf.Repeat (rot.eulerAngles.z - transform.eulerAngles.z - CAMERA_ANGLE.z + 180f, 360f) - 180f) / 2 + 180f, 360f) - 180f);

			lv = v;
		}

		if (Input.GetMouseButtonUp (0)) {
			Cursor.lockState = CursorLockMode.None;
		}
	}
}
