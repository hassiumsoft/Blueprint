using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Chunk : ISerializable {
	public const string KEY_X = "X";
	public const string KEY_Z = "Z";
	public const string KEY_MESH = "MESH";
	public const string KEY_OBJECTS = "OBJECTS";

	//最大チャンク数: -16777216~16777215 ( -2^(32-1)/128 ~ 2^(32-1)/128-1 )
	//チャンクの頂点数: 3072 (4*4^4*3 = 4*4^fineness*3)
	public const int size = 128; //チャンクサイズ（変更してはいけない）

	//TODO スペックに応じて荒い地形から細かい地形まで自動的に調整されるようにする。設定でも変更可能にする

	//高低差の基準値（基準値よりズレが生じる場合がある。変更可能）
	//TODO 0.2fにしたらColliderの作成エラーが発生 [Physics.PhysX] ConvexHullBuilder::CreateTrianglesFromPolygons: convex hull has a polygon with less than 3 vertices!
	public const float height = 8;

	//フラクタル地形の細かさ（細分化回数）
	//3では20秒程度かかった。 4では1分程度かかった。 5では7分~28分程度かかった。 6では数十分~数時間以上の時間がかかった。
	//7ではメッシュの超点数が制限(65000)を超えてしまうので不可能。
	//描画を優先しない場合はfinenessが3で4/1秒。 4では2~3秒程度。
	//スペックによる差があるため3倍かかると見込んだほうが良い。
	//理想は6~7 (size/2^fineness=1になる数値)
	//TODO 細分化回数の違うメッシュをつなぎ合わせるようにする
	public const int fineness = 4;

	//TODO チャンクの読み込み速度目標値: 2.17013888...9チャンク/秒 (1000km/hで動くものに対応させるため。チャンク生成速度とは異なる）

	//TODO 一時的。（Main.csも確認）
	public Material mat;

	public GameObject obj;

	public Map map;
	public int x { get; }
	public int z { get; }

	//地形データ。後にMapObject化して複数の地形を組み合わせられるようにする。
	public Mesh mesh;
	//オブジェクトデータ
	public List<MapObject> objs;

	//public SerializableMaterial sMat;

	public Chunk (Map map, int x, int z) {
		this.map = map;
		this.x = x;
		this.z = z;
		objs = new List<MapObject> ();

		//TODO 一時的
		mat = Main.main.mat;
	}

	protected Chunk (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		x = info.GetInt32 (KEY_X);
		z = info.GetInt32 (KEY_Z);
		SerializableMesh sMesh = ((SerializableMesh)info.GetValue (KEY_MESH, typeof(SerializableMesh)));
		if (sMesh != null) {
			mesh = sMesh.toMesh ();
		}
		objs = (List<MapObject>)info.GetValue (KEY_OBJECTS, typeof(List<MapObject>));
		for (int a = 0; a < objs.Count; a++) {
			objs [a].chunk = this;
		}

		//TODO 一時的
		mat = Main.main.mat;
	}

	public virtual void GetObjectData (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		info.AddValue (KEY_X, x);
		info.AddValue (KEY_Z, z);
		info.AddValue (KEY_MESH, mesh == null ? null : new SerializableMesh (mesh));
		info.AddValue (KEY_OBJECTS, objs);
	}

	public IEnumerator generate (MonoBehaviour behaviour) {
		return generate(behaviour, false);
	}

	public IEnumerator generate (MonoBehaviour behaviour, bool pause) {
		if (obj == null) {
			obj = new GameObject ();
			obj.AddComponent<MeshFilter> ();
			obj.AddComponent<MeshRenderer> ();
			obj.AddComponent<MeshCollider> ();
			//obj.AddComponent<MeshCollider> ().convex = true;
			/*BoxCollider box = obj.AddComponent<BoxCollider> ();
			box.center = new Vector3 (size / 2, -0.5f, size / 2);
			box.size = new Vector3 (size, 1, size);*/

			obj.transform.position = new Vector3 (x * size, 0, z * size);
		}

		obj.GetComponent<MeshRenderer> ().material = mat;
		yield return null;

		if (mesh == null) {
			Debug.Log ("チャンク生成開始 X: " + x + " Z: " + z + " Date: " + DateTime.Now);
			List<Vector3> points = new List<Vector3> ();
			for (int x2 = x - 1; x2 <= x + 1; x2++) {
				for (int z2 = z - 1; z2 <= z + 1; z2++) {
					if (x2 != x || z2 != z) {
						int a = map.getChunk (x2, z2);
						if (a != -1) {
							Chunk chunk = map.chunks [a];
							if (chunk.mesh != null) {
								List<Vector3> verts1 = new List<Vector3> (chunk.mesh.vertices);
								for (int b = 0; b < verts1.Count;) {
									//ゲームプレイに影響を与えない程度にマップ生成を優先する
									if (1 <= Time.deltaTime * Application.targetFrameRate) {
										yield return null;
									}
									if (verts1 [b].x == 0 || verts1 [b].z == 0 || verts1 [b].x == size || verts1 [b].z == size) {
										b++;
									} else {
										verts1.RemoveAt (b);
									}
								}
								Vector3[] verts2 = verts1.ToArray ();
								for (int c = 0; c < verts2.Length; c++) {
									if (1 <= Time.deltaTime * Application.targetFrameRate) {
										yield return null;
									}
									verts2 [c] += (x2 - x) * Vector3.right * size + (z2 - z) * Vector3.forward * size;
								}
								points.AddRange (verts2);
							}
						}
					}
				}
			}

			if (pause) {
				mesh = BPMesh.getBPFractalTerrain (fineness, size, height, points);
			} else {
				IEnumerator routine = BPMesh.getBPFractalTerrainAsync (behaviour, fineness, size, height, points);
				yield return behaviour.StartCoroutine (routine);
				if (routine.Current is Mesh) {
					mesh = (Mesh)routine.Current;
					yield return null;
				}
			}
			Debug.Log ("チャンク生成完了 X: " + x + " Z: " + z + " Date: " + DateTime.Now);
		}

		MeshFilter meshfilter = obj.GetComponent<MeshFilter> ();
		meshfilter.sharedMesh = mesh;

		obj.GetComponent<MeshCollider> ().sharedMesh = meshfilter.sharedMesh;



		for (int a = 0; a < objs.Count; a++) {
			objs [a].generate (behaviour);
		}
	}
}
