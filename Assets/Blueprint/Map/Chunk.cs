using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Chunk : ISerializable {
	public const string KEY_X = "X";
	public const string KEY_Z = "Z";
	public const string KEY_GENERATED = "GENERATED";
	public const string KEY_MESH = "MESH";
	public const string KEY_OBJECTS = "OBJECTS";

	//最大チャンク数: -16777216~16777215 ( -2^(32-1)/128 ~ 2^(32-1)/128-1 )
	//チャンクの頂点数: 3072 (4*4^4*3 = 4*4^fineness*3)
	public const int size = 128; //チャンクサイズ（変更してはいけない）

	//TODO スペックに応じて荒い地形から細かい地形まで自動的に調整されるようにする。設定でも変更可能にする

	//TODO 必須チャンク範囲を作り、必須チャンク範囲を優先して生成する。
	//優先されないチャンクは読み込み停止などの管理を自動的に行うようにする。
	//もしくは、同時生成チャンク数を管理する。

	//TODO （優先）不要になったチャンクのアンロード

	//高低差の基準値（基準値よりズレが生じる場合がある。変更可能）
	//TODO 0.2fにしたらColliderの作成エラーが発生 [Physics.PhysX] ConvexHullBuilder::CreateTrianglesFromPolygons: convex hull has a polygon with less than 3 vertices!
	public const float height = 4f;

	//フラクタル地形の細かさ（細分化回数）
	//3では20秒程度かかった。 4では1分程度かかった。 5では7分~28分程度かかった。 6では数十分~数時間以上の時間がかかった。
	//7ではメッシュの超点数が制限(65000)を超えてしまうので不可能。
	//描画を優先しない場合はfinenessが3で4/1秒。 4では2~3秒程度。
	//スペックによる差があるため3倍かかると見込んだほうが良い。
	//理想の細かさは6~7 (size/2^fineness=1になる数値)
	//TODO 細分化回数の違うメッシュをつなぎ合わせるようにする
	public const int fineness = 4;

	//TODO チャンクの読み込み速度目標値: 2.17013888...9チャンク/秒 (1000km/hで動くものに対応させるため。チャンク生成速度とは異なる）

	//TODO 溜めようとすると12まで溜まるがその分生成が遅くなってしまう
	public const int max_generation = 1; //最大同時生成チャンク数

	private static List<Chunk> generatingChunks = new List<Chunk> (); //生成中のチャンク数（非同期のみ）

	//生成中のチャンクが最大同時生成チャンク数を超えたため、
	//生成がキャンセルされたチャンク。
	//生成中のチャンクが完了するとキャンセルされたチャンクが動き出す。
	private static List<Chunk> generateCancelledChunks = new List<Chunk> ();

	public ChunkEntity obj;

	public Map map;
	public int x { get; }
	public int z { get; }
	public bool generated { get; private set; } //地形データなどが生成されているかどうか。実体ではないので注意。
	//TODO アンロード中に時間が経つと進んだ分の時間が経つ。
	public long lasttime { get; private set; } //最後に読み込まれた時のマップ時間。読み込まれていない状態ではチャンクの処理は行われない。

	//地形データ。後にMapObject化して複数の地形を組み合わせられるようにする。
	public Mesh mesh;
	private bool _generating = false;
	public bool generating { get { return _generating; } private set { _generating = value; } } //チャンク生成中
	private bool stopGenerating = false; //現在実行中の生成を停止するか（非同期のみ）
	private IEnumerator routine;

	//オブジェクトデータ
	public List<MapObject> objs;

	//public SerializableMaterial sMat;

	public Chunk (Map map, int x, int z) {
		this.map = map;
		this.x = x;
		this.z = z;
		objs = new List<MapObject> ();

		//TODO
		lasttime = map.time;
	}

	protected Chunk (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		x = info.GetInt32 (KEY_X);
		z = info.GetInt32 (KEY_Z);
		generated = info.GetBoolean (KEY_GENERATED);
		SerializableMesh sMesh = ((SerializableMesh)info.GetValue (KEY_MESH, typeof(SerializableMesh)));
		if (sMesh != null) {
			mesh = sMesh.toMesh ();
		}
		objs = (List<MapObject>)info.GetValue (KEY_OBJECTS, typeof(List<MapObject>));
		/*for (int a = 0; a < objs.Count; a++) {
			//TODO MapObject.chunkはprivate setにする必要がある
			objs [a].chunk = this;
		}*/

	}

	public virtual void GetObjectData (SerializationInfo info, StreamingContext context) {
		if (info == null)
			throw new ArgumentNullException ("info");
		info.AddValue (KEY_X, x);
		info.AddValue (KEY_Z, z);
		info.AddValue (KEY_GENERATED, generated);
		info.AddValue (KEY_MESH, mesh == null ? null : new SerializableMesh (mesh));
		info.AddValue (KEY_OBJECTS, objs);
	}

	//Playerなどにもある実体を生成するメソッド。同時に地形データなども生成する
	public bool generate () {
		bool r = generateChunk ();
		generateObj ();
		return r;
	}

	//実体を生成するメソッド。地形データなどは生成せず、すでに存在する場合に使用される。
	public void generateObj () {
		if (obj == null)
			(obj = new GameObject ("chunk-" + x + "," + z).AddComponent<ChunkEntity> ()).init (this);
		else
			obj.reload ();
		
		for (int a = 0; a < objs.Count; a++) {
			objs [a].generate ();
		}
	}

	//地形データなどを生成するメソッド。実体は生成しない。
	public bool generateChunk () {
		if (generated || generating)
			return false;

		stopAsyncGenerating ();
		generating = true;
		Debug.Log (DateTime.Now + " チャンク生成開始 X: " + x + " Z: " + z);

		//地形の生成
		List<Vector3> points = new List<Vector3> ();
		for (int x2 = x - 1; x2 <= x + 1; x2++) {
			for (int z2 = z - 1; z2 <= z + 1; z2++) {
				if (x2 != x || z2 != z) {
					Chunk chunk = map.getChunk (x2, z2);
					if (chunk.generating)
						chunk.stopAsyncGenerating ();

					if (chunk.generated && chunk.mesh != null) {
						List<Vector3> verts1 = new List<Vector3> (chunk.mesh.vertices);
						for (int b = 0; b < verts1.Count;) {
							if (verts1 [b].x == 0 || verts1 [b].z == 0 || verts1 [b].x == size || verts1 [b].z == size)
								b++;
							else
								verts1.RemoveAt (b);
						}
						Vector3[] verts2 = verts1.ToArray ();
						for (int c = 0; c < verts2.Length; c++)
							verts2 [c] += (x2 - x) * Vector3.right * size + (z2 - z) * Vector3.forward * size;
						points.AddRange (verts2);
					}
				}
			}
		}

		mesh = BPMesh.getBPFractalTerrain (fineness, size, height, points);

		//森林の生成
		generateForest ();

		generating = false;
		generated = true;
		Debug.Log (DateTime.Now + " チャンク生成完了 X: " + x + " Z: " + z);

		if (obj != null)
			obj.reload ();

		return true;
	}

	public IEnumerator generateAsync () {
		yield return Main.main.StartCoroutine (routine = a ());
		generateObj ();
		b ();
	}

	private IEnumerator a () {
		if (generated || generating)
			yield break;

		if (generatingChunks.Count >= max_generation) {
			generateCancelledChunks.Add (this);
			yield break;
		}

		if (stopGenerating) {
			stopGenerating = false;
			yield break;
		}

		generating = true;
		generatingChunks.Add (this);
		Debug.Log (DateTime.Now + " チャンク生成開始(Async) X: " + x + " Z: " + z);

		//地形の生成
		List<Vector3> points = new List<Vector3> ();
		bool e_u = false;
		bool e_d = false;
		bool e_l = false;
		bool e_r = false;
		bool e_ul = false;
		bool e_ur = false;
		bool e_dr = false;
		bool e_dl = false;
		while (!(e_u && e_d && e_l && e_r && e_ul && e_ur && e_dr && e_dl)) {
			int x2 = x;
			int z2 = z;
			if (!e_u)
				z2 = z + 1;
			else if (!e_d)
				z2 = z - 1;
			else if (!e_l)
				x2 = x - 1;
			else if (!e_r)
				x2 = x + 1;
			else if (!e_ul) {
				x2 = x - 1;
				z2 = z + 1;
			} else if (!e_ur) {
				x2 = x + 1;
				z2 = z + 1;
			} else if (!e_dr) {
				x2 = x + 1;
				z2 = z - 1;
			} else if (!e_dl) {
				x2 = x - 1;
				z2 = z - 1;
			}

			Chunk chunk = map.getChunk (x2, z2);
			if (chunk.generating) {
				//他の未生成だったチャンクが待機中に生成を開始している場合があるため、
				//自チャンクの生成を後に回し最初からやり直す。
				generating = false;
				generatingChunks.Remove (this);
				generateCancelledChunks.Add (this);
				Debug.Log (DateTime.Now + " チャンク生成中止(13) X: " + x + " Z: " + z);
				yield break;
			}

			if (chunk.generated) {
				List<Vector3> verts1 = new List<Vector3> (chunk.mesh.vertices);
				for (int b = 0; b < verts1.Count;) {
					//ゲームプレイに影響を与えない程度にマップ生成を優先する
					//if (Main.yrCondition ())
					//	yield return null;
					if (verts1 [b].x == 0 || verts1 [b].z == 0 || verts1 [b].x == size || verts1 [b].z == size)
						b++;
					else
						verts1.RemoveAt (b);
				}
				Vector3[] verts2 = verts1.ToArray ();
				for (int c = 0; c < verts2.Length; c++) {
					//if (Main.yrCondition ())
					//	yield return null;
					verts2 [c] += (x2 - x) * Vector3.right * size + (z2 - z) * Vector3.forward * size;
				}
				points.AddRange (verts2);

				if (!e_u) {
					e_ul = true;
					e_ur = true;
				} else if (!e_d) {
					e_dl = true;
					e_dr = true;
				} else if (!e_l) {
					e_ul = true;
					e_dl = true;
				} else if (!e_r) {
					e_ur = true;
					e_dr = true;
				}
			}

			if (!e_u)
				e_u = true;
			else if (!e_d)
				e_d = true;
			else if (!e_l)
				e_l = true;
			else if (!e_r)
				e_r = true;
			else if (!e_ul)
				e_ul = true;
			else if (!e_ur)
				e_ur = true;
			else if (!e_dr)
				e_dr = true;
			else if (!e_dl)
				e_dl = true;
		}

		IEnumerator _routine = BPMesh.getBPFractalTerrainAsync (Main.main, fineness, size, height, points);
		yield return Main.main.StartCoroutine (_routine);
		if (_routine.Current is Mesh) {
			if (stopGenerating) {
				Debug.Log (DateTime.Now + " チャンク生成中止(14) X: " + x + " Z: " + z);
				generating = false;
				stopGenerating = false;
				generatingChunks.Remove (this);
				yield break;
			}

			mesh = (Mesh)_routine.Current;
		}

		//森林の生成
		generateForest ();

		generating = false;
		generatingChunks.Remove (this);
		generated = true;
		Debug.Log (DateTime.Now + " チャンク生成完了 X: " + x + " Z: " + z);

		if (obj != null)
			obj.reload ();
	}

	private void stopAsyncGenerating () {
		if (routine != null) {
			Main.main.StopCoroutine (routine);
			generating = false; //TODO 同期による生成中でもfalseにしてしまう可能性がある
			generatingChunks.Remove (this);
		}
	}

	private void generateForest () {
		float px = x * size + UnityEngine.Random.Range (0, size);
		float pz = z * size + UnityEngine.Random.Range (0, size);
		map.addObject (new TreeObject (map, new Vector3 (px, map.getTerrainHeight (px, pz), pz)));
	}

	//時間が経過するメソッド。MapやMapObjectと違い経過時間ではなくlong型で新しい時間を指定する。
	public void TimePasses (long time) {
		foreach (MapObject obj in objs)
			obj.TimePasses (time - lasttime);
		lasttime = time;
	}

	public static void b () {
		//生成がキャンセルされたチャンクを非同期で生成させる
		int n = 0;
		for (; generatingChunks.Count < max_generation && n < generateCancelledChunks.Count; n++) {
			Main.main.StartCoroutine (generateCancelledChunks [n].generateAsync ());
		}
		generateCancelledChunks.RemoveRange (0, n);
	}

	public static void c () {
		//生成がキャンセルされたチャンクを同期で生成させる
		int n = 0;
		for (; generatingChunks.Count < max_generation && n < generateCancelledChunks.Count; n++) {
			generateCancelledChunks [n].generate ();
		}
		generateCancelledChunks.RemoveRange (0, n);
	}

	public static void stopAllGenerating () {
		foreach (Chunk chunk in generatingChunks) {
			chunk.stopAsyncGenerating ();
		}
	}
}
