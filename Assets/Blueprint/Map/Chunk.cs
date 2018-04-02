using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Chunk : ISerializable {
	//チャンクの読み込み速度目標値: 2.17013888...9チャンク/秒 (1000km/hで動くものに対応させるため。チャンク生成速度とは異なる）

	public const string KEY_X = "X";
	public const string KEY_Z = "Z";
	public const string KEY_GENERATED = "GENERATED";
	public const string KEY_MESH = "MESH";
	public const string KEY_OBJECTS = "OBJECTS";

	//最大チャンク数: -16777216~16777215 ( -2^(32-1)/128 ~ 2^(32-1)/128-1 )
	//チャンクの頂点数: 3072 (4*4^4*3 = 4*4^fineness*3)
	public const int size = 128; //チャンクサイズ（変更してはいけない）

	//地形の高さ。一時的な変数。
	public static float height = 4f;

	public const int fineness = 4;//フラクタル地形の細かさ（細分化回数）
	public const int max_generation = 1; //最大同時生成チャンク数。数値に理由はない。

	private static List<Chunk> generatingChunks = new List<Chunk> (); //生成中のチャンク数（非同期のみ）

	//生成中のチャンクが最大同時生成チャンク数を超えたため、
	//生成がキャンセルされたチャンク。
	//生成中のチャンクが完了するとキャンセルされたチャンクが動き出す。
	private static List<Chunk> generateCancelledChunks = new List<Chunk> ();

	public ChunkEntity entity;
	[NonSerialized]
	private Map _map;
	public Map map {
		get { return _map; }
		set {
			if (_map == null)
				_map = value;
		}
	}
	public int x { get; private set; }
	public int z { get; private set; }
	public bool generated { get; private set; } //地形データなどが生成されているかどうか。実体ではないので注意。
	//TODO アンロード中に時間が経つと進んだ分の時間が経つ。
	public long lasttime { get; private set; } //最後に読み込まれた時のマップ時間。読み込まれていない状態ではチャンクの処理は行われない。

	//地形データ。後にMapObject化して複数の地形を組み合わせられるようにする。
	public Mesh mesh;
	private bool _generatingSync = false;
	public bool generatingSync { get { return _generatingSync; } private set { _generatingSync = value; } } //同期でチャンク生成中
	private bool _generatingAsync = false;
	public bool generatingAsync { get { return _generatingAsync; } private set { _generatingAsync = value; } } //非同期でチャンク生成中
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
		SerializableMesh sMesh = (SerializableMesh)info.GetValue (KEY_MESH, typeof(SerializableMesh));
		if (sMesh != null) {
			mesh = sMesh.toMesh ();
		}
		objs = (List<MapObject>)info.GetValue (KEY_OBJECTS, typeof(List<MapObject>));
		for (int a = 0; a < objs.Count; a++)
			objs [a].chunk = this;
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
		if (entity == null)
			(entity = new GameObject ("chunk-" + x + "," + z).AddComponent<ChunkEntity> ()).init (this);
		else
			reloadEntity ();
		
		for (int a = 0; a < objs.Count; a++) {
			objs [a].generate ();
		}
	}

	//地形データなどを生成するメソッド。実体は生成しない。
	public bool generateChunk () {
		if (generated || generatingSync)
			return false;

		stopAsyncGenerating ();
		generatingSync = true;
		Debug.Log (DateTime.Now + " チャンク生成開始 X: " + x + " Z: " + z);

		//地形の生成
		List<Vector3> points = new List<Vector3> ();
		for (int x2 = x - 1; x2 <= x + 1; x2++) {
			for (int z2 = z - 1; z2 <= z + 1; z2++) {
				if (x2 != x || z2 != z) {
					Chunk chunk = map.getChunk (x2, z2);
					if (chunk.generatingAsync)
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

		generatingSync = false;
		generated = true;
		Debug.Log (DateTime.Now + " チャンク生成完了 X: " + x + " Z: " + z);

		reloadEntity ();

		return true;
	}

	public IEnumerator generateAsync () {
		yield return Main.main.StartCoroutine (routine = a ());
		generateObj ();
		b ();
	}

	private IEnumerator a () {
		if (generated || generatingAsync)
			yield break;

		if (generatingChunks.Count >= max_generation) {
			generateCancelledChunks.Add (this);
			yield break;
		}

		if (stopGenerating) {
			stopGenerating = false;
			yield break;
		}

		generatingAsync = true;
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
			if (chunk.generatingAsync) {
				//隣接するチャンクが待機中に生成を開始している場合があるため、
				//自チャンクの生成を後に回し最初からやり直す。
				generatingAsync = false;
				generatingChunks.Remove (this);
				generateCancelledChunks.Add (this);
				Debug.Log (DateTime.Now + " チャンク生成中止(13) X: " + x + " Z: " + z);
				yield break;
			}

			if (chunk.generated) {
				List<Vector3> verts1 = new List<Vector3> (chunk.mesh.vertices);
				for (int b = 0; b < verts1.Count;) {
					if (verts1 [b].x == 0 || verts1 [b].z == 0 || verts1 [b].x == size || verts1 [b].z == size) {
						verts1 [b] += (x2 - x) * Vector3.right * size + (z2 - z) * Vector3.forward * size;
						b++;
					} else
						verts1.RemoveAt (b);
				}
				points.AddRange (verts1);

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
				generatingAsync = false;
				stopGenerating = false;
				generatingChunks.Remove (this);
				yield break;
			}

			mesh = (Mesh)_routine.Current;
		}

		//森林の生成
		generateForest ();

		generatingAsync = false;
		generatingChunks.Remove (this);
		generated = true;
		Debug.Log (DateTime.Now + " チャンク生成完了 X: " + x + " Z: " + z);

		reloadEntity ();
	}

	private void stopAsyncGenerating () {
		c ();
		generatingChunks.Remove (this);
	}

	private void generateForest () {
		if (UnityEngine.Random.Range (0, 4) != 0)
			return;
		
		float px = x * size + UnityEngine.Random.Range (0, size);
		float pz = z * size + UnityEngine.Random.Range (0, size);
		map.addObject (new TreeObject (map, new Vector3 (px, map.getTerrainHeight (px, pz), pz), Quaternion.Euler (new Vector3 (0, UnityEngine.Random.Range (0f, 360f)))));
	}

	//時間が経過するメソッド。MapやMapObjectと違い経過時間ではなくlong型で新しい時間を指定する。
	public void TimePasses (long time) {
		foreach (MapObject obj in objs)
			obj.TimePasses (time - lasttime);
		lasttime = time;
	}

	public static void b () {
		//生成がキャンセルされたチャンクを非同期で生成させる
		while (generatingChunks.Count < max_generation && generateCancelledChunks.Count > 0) {
			Main.main.StartCoroutine (generateCancelledChunks [0].generateAsync ());
			generateCancelledChunks.RemoveAt (0);
		}
	}

	public static void stopAllGenerating () {
		foreach (Chunk chunk in generatingChunks) {
			chunk.c ();
		}
		generatingChunks.Clear ();
		generateCancelledChunks.Clear ();
	}

	private void c () {
		if (routine != null) {
			Main.main.StopCoroutine (routine);
			generatingAsync = false;
		}
	}

	public void reloadEntity () {
		if (entity == null)
			return;
		entity.transform.position = new Vector3 (x * size, 0, z * size);

		MeshFilter meshfilter = entity.GetComponent<MeshFilter> ();
		MeshRenderer meshrenderer = entity.GetComponent<MeshRenderer> ();
		MeshCollider meshcollider = entity.GetComponent<MeshCollider> ();
		if (meshfilter == null)
			meshfilter = entity.gameObject.AddComponent<MeshFilter> ();
		if (meshrenderer == null)
			meshrenderer = entity.gameObject.AddComponent<MeshRenderer> ();
		if (meshcollider == null)
			meshcollider = entity.gameObject.AddComponent<MeshCollider> ();

		meshrenderer.material = Main.main.mat; //TODO 一時的。（Main.csも確認）

		//meshcollider.convex = true;
		meshcollider.sharedMesh = meshfilter.sharedMesh = mesh;
	}
}
