using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMesh {

	//メッシュの複製
	public static Mesh mesh_copy (Mesh mesh) {
		Mesh m = new Mesh ();

		m.vertices = mesh.vertices;
		m.uv = mesh.uv;
		m.uv2 = mesh.uv2;
		m.uv3 = mesh.uv3;
		m.uv4 = mesh.uv4;
		m.triangles = mesh.triangles;

		m.bindposes = mesh.bindposes;
		m.boneWeights = mesh.boneWeights;
		m.bounds = mesh.bounds;
		m.colors = mesh.colors;
		m.colors32 = mesh.colors32;
		m.normals = mesh.normals;
		m.subMeshCount = mesh.subMeshCount;
		m.tangents = mesh.tangents;

		return m;
	}

	//メッシュの結合（ソリッドは判定しない）
	public static Mesh mesh_combine (Mesh target, Mesh source) {
		Mesh m = new Mesh ();

		List<Vector3> a = new List<Vector3> (target.vertices);
		a.AddRange (source.vertices);
		m.vertices = a.ToArray ();

		List<Vector2> b = new List<Vector2> (target.uv);
		b.AddRange (source.uv);
		m.uv = b.ToArray ();

		b = new List<Vector2> (target.uv2);
		b.AddRange (source.uv2);
		m.uv2 = b.ToArray ();

		b = new List<Vector2> (target.uv3);
		b.AddRange (source.uv3);
		m.uv3 = b.ToArray ();

		b = new List<Vector2> (target.uv4);
		b.AddRange (source.uv4);
		m.uv4 = b.ToArray ();

		List<int> c = new List<int> (target.triangles);
		int[] d = source.triangles;
		foreach (int e in d)
			c.Add (e + target.triangles.Length);
		m.triangles = c.ToArray ();

		return m;
	}

	//メッシュの細分化
	//TODO フラット面とスムーズ面を判別して細分化
	public static Mesh Subdivide_Half (Mesh mesh, bool smooth) {
		Mesh m = mesh_copy (mesh);

		List<Vector3> newverts = new List<Vector3> (m.vertices);
		List<Vector2> newuv = new List<Vector2> (m.uv);
		List<int> newtris = new List<int> ();

		if (smooth) {
			for (int c = 0; c <= m.triangles.Length - 3; c += 3) {
				int vn0 = m.triangles [c];
				int vn1 = m.triangles [c + 1];
				int vn2 = m.triangles [c + 2];

				newverts.Add ((m.vertices [vn0] + m.vertices [vn1]) / 2);
				newverts.Add ((m.vertices [vn1] + m.vertices [vn2]) / 2);
				newverts.Add ((m.vertices [vn2] + m.vertices [vn0]) / 2);

				newuv.Add ((m.uv [vn0] + m.uv [vn1]) / 2);
				newuv.Add ((m.uv [vn1] + m.uv [vn2]) / 2);
				newuv.Add ((m.uv [vn2] + m.uv [vn0]) / 2);

				int _vn = newverts.Count - 3;

				newtris.Add (vn0);
				newtris.Add (_vn);
				newtris.Add (_vn + 2);

				newtris.Add (vn1);
				newtris.Add (_vn + 1);
				newtris.Add (_vn);

				newtris.Add (vn2);
				newtris.Add (_vn + 2);
				newtris.Add (_vn + 1);

				newtris.Add (_vn);
				newtris.Add (_vn + 1);
				newtris.Add (_vn + 2);
			}
		} else {
			for (int c = 0; c <= m.triangles.Length - 3; c += 3) {
				int vn0 = m.triangles [c];
				int vn1 = m.triangles [c + 1];
				int vn2 = m.triangles [c + 2];

				Vector3 v0 = (m.vertices [vn0] + m.vertices [vn1]) / 2;
				Vector3 v1 = (m.vertices [vn1] + m.vertices [vn2]) / 2;
				Vector3 v2 = (m.vertices [vn2] + m.vertices [vn0]) / 2;

				newverts.Add (v0);
				newverts.Add (v1);
				newverts.Add (v2);
				newverts.Add (v0);
				newverts.Add (v1);
				newverts.Add (v2);
				newverts.Add (v0);
				newverts.Add (v1);
				newverts.Add (v2);

				Vector2 vu0 = (m.uv [vn0] + m.uv [vn1]) / 2;
				Vector2 vu1 = (m.uv [vn1] + m.uv [vn2]) / 2;
				Vector2 vu2 = (m.uv [vn2] + m.uv [vn0]) / 2;

				newuv.Add (vu0);
				newuv.Add (vu1);
				newuv.Add (vu2);
				newuv.Add (vu0);
				newuv.Add (vu1);
				newuv.Add (vu2);
				newuv.Add (vu0);
				newuv.Add (vu1);
				newuv.Add (vu2);

				int _vn = newverts.Count - 9;

				newtris.Add (vn0);
				newtris.Add (_vn);
				newtris.Add (_vn + 2);
				newtris.Add (vn1);
				newtris.Add (_vn + 1);
				newtris.Add (_vn + 3);
				newtris.Add (vn2);
				newtris.Add (_vn + 5);
				newtris.Add (_vn + 4);
				newtris.Add (_vn + 6);
				newtris.Add (_vn + 7);
				newtris.Add (_vn + 8);
			}
		}

		m.vertices = newverts.ToArray ();
		m.uv = newuv.ToArray ();
		m.triangles = newtris.ToArray ();

		return m;
	}

	//頂点及びUVが一致する頂点をまとめてスムーズ化
	public static Mesh Remove_Doubles (Mesh mesh) {
		Mesh m = mesh_copy (mesh);

		List<Vector3> verts = new List<Vector3> (m.vertices);
		int[] tris = m.triangles;

		for (int b = 0; b < verts.Count; b++) {
			for (int c = b + 1; c < verts.Count; c++) {
				if (verts [b] == verts [c] && m.uv [b] == m.uv [c]) {
					for (int d = 0; d < tris.Length; d++) {
						if (tris [d] == c) {
							tris [d] = b;
						}
					}
				}
			}
		}

		m.triangles = tris;
		m.vertices = verts.ToArray ();

		return m;
	}

	//TODO フラット面化

	/*public static Vector3 getNormal (Mesh mesh, int v) {
		Vector3? a = null;
		for (int b = 0; b < mesh.triangles.Length;) {
			if (mesh.triangles [b] == v) {
				int c = b % 3;
				b -= c;

				Vector3 e = Vector3.Cross (mesh.vertices [mesh.triangles [c + 1 < 3 ? b + c + 1 : b + c + 1 - 3]], 
					            mesh.vertices [mesh.triangles [c + 2 < 3 ? b + c + 2 : b + c + 2 - 3]]);
				if (a == null) {
					a = e;
				} else {
					a = (a + e) / 2;
				}

				b += 3;
			} else {
				b++;
			}
		}

		return a == null ? Vector3.zero : (Vector3)a;
	}

	public static Vector3 getNormal (Mesh mesh, Vector3 v) {
		Vector3? a = null;
		for (int b = 0; b < mesh.triangles.Length;) {
			if (mesh.vertices [mesh.triangles [b]] == v) {
				int c = b % 3;
				b -= c;

				Vector3 e = Vector3.Cross (mesh.vertices [mesh.triangles [c + 1 < 3 ? b + c + 1 : b + c + 1 - 3]], 
					            mesh.vertices [mesh.triangles [c + 2 < 3 ? b + c + 2 : b + c + 2 - 3]]);
				if (a == null) {
					a = e;
				} else {
					a = (a + e) / 2;
				}

				b += 3;
			} else {
				b++;
			}
		}

		return a == null ? Vector3.zero : (Vector3)a;
	}*/

	//同一位置にある頂点を目的地に移動
	//
	//TODO start, endの指定が存在するメソッドの繰り返し処理の条件にある"n < verts.Length"や"n < verts.Count"は、
	//エラー回避なしでも出来る場合がほとんどであるため、繰り返し処理の負担を避けるためコメントアウトしている。
	public static void setVert(Vector3[] verts, Vector3 target, Vector3 result) {
		for (int n = 0; n < verts.Length; n++)
			if (verts [n] == target)
				verts [n] = result;
	}

	public static void setVert(Vector3[] verts, Vector3 target, Vector3 result, int start, int end) {
		for (int n = start; /*n < verts.Length && */n < end; n++)
			if (verts [n] == target)
				verts [n] = result;
	}

	public static IEnumerator setVertAsync(Vector3[] verts, Vector3 target, Vector3 result) {
		for (int n = 0; n < verts.Length; n++) {
			if (Main.yrCondition ())
				yield return null;
			if (verts [n] == target)
				verts [n] = result;
		}
	}

	public static IEnumerator setVertAsync(Vector3[] verts, Vector3 target, Vector3 result, int start, int end) {
		for (int n = start; /*n < verts.Length && */n < end; n++) {
			if (Main.yrCondition ())
				yield return null;
			if (verts [n] == target)
				verts [n] = result;
		}
	}

	public static void setVert(List<Vector3> verts, Vector3 target, Vector3 result) {
		for (int n = 0; n < verts.Count; n++)
			if (verts [n] == target)
				verts [n] = result;
	}

	public static void setVert(List<Vector3> verts, Vector3 target, Vector3 result, int start, int end) {
		for (int n = start; /*n < verts.Count && */n < end; n++)
			if (verts [n] == target)
				verts [n] = result;
	}

	public static IEnumerator setVertAsync(List<Vector3> verts, Vector3 target, Vector3 result) {
		for (int n = 0; n < verts.Count; n++) {
			if (Main.yrCondition ())
				yield return null;
			if (verts [n] == target)
				verts [n] = result;
		}
	}

	public static IEnumerator setVertAsync(List<Vector3> verts, Vector3 target, Vector3 result, int start, int end) {
		for (int n = start; /*n < verts.Count && */n < end; n++) {
			if (Main.yrCondition ())
				yield return null;
			if (verts [n] == target)
				verts [n] = result;
		}
	}

	public static Mesh getTriangleFlat () {
		Mesh mesh = new Mesh ();

		mesh.vertices = new Vector3[]{ Vector3.zero, Vector3.forward, Vector3.right };
		mesh.uv = new Vector2[]{ Vector2.zero, Vector2.up, Vector2.right };
		mesh.triangles = new int[]{ 0, 1, 2 };

		return mesh;
	}

	//XZ面の四角形を作成。2つの三角面で構成されている
	public static Mesh getQuadFlat () {
		Mesh mesh = new Mesh ();

		mesh.vertices = new Vector3[] {
			Vector3.zero,
			Vector3.forward,
			Vector3.right + Vector3.forward,
			Vector3.right + Vector3.forward,
			Vector3.right,
			Vector3.zero
		};
		mesh.uv = new Vector2[] {
			Vector2.zero,
			Vector2.up,
			Vector2.right + Vector2.up,
			Vector2.right + Vector2.up,
			Vector2.right,
			Vector2.zero
		};
		mesh.triangles = new int[]{ 0, 1, 2, 3, 4, 5 };

		return mesh;
	}

	//XZ面の地形用の四角形を作成。四隅と中心に点を置き、それぞれを結んだ4つの三角面で構成されている
	public static Mesh getQuadTerrain () {
		Mesh mesh = new Mesh ();

		mesh.vertices = new Vector3[] {
			Vector3.zero,
			Vector3.forward,
			(Vector3.right + Vector3.forward) / 2,
			Vector3.forward,
			Vector3.right + Vector3.forward,
			(Vector3.right + Vector3.forward) / 2,
			Vector3.right + Vector3.forward,
			Vector3.right,
			(Vector3.right + Vector3.forward) / 2,
			Vector3.right,
			Vector3.zero,
			(Vector3.right + Vector3.forward) / 2
		};
		mesh.uv = new Vector2[] {
			Vector2.zero,
			Vector2.up,
			(Vector2.right + Vector2.up) / 2,
			Vector2.up,
			Vector2.right + Vector2.up,
			(Vector2.right + Vector2.up) / 2,
			Vector2.right + Vector2.up,
			Vector2.right,
			(Vector2.right + Vector2.up) / 2,
			Vector2.right,
			Vector2.zero,
			(Vector2.right + Vector2.up) / 2
		};
		mesh.triangles = new int[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

		return mesh;
	}

	//中点変位法を使用したフラクタル地形
	public static Mesh getBPFractalTerrain (int fineness, float size, float height) {
		return getBPFractalTerrain (fineness, size, height, new List<Vector3> ());
	}

	//pointsに点群データを入れておくことで、X,Zが一致する点のYを点群データに合わせることが出来る。
	//チャンクに対応した地形を生成する際に使用する。
	public static Mesh getBPFractalTerrain (int fineness, float size, float height, List<Vector3> points) {
		//地形用の四角形メッシュを作成
		Mesh mesh = getQuadTerrain ();

		//チャンクサイズに拡大
		Vector3[] verts = mesh.vertices;
		for (int a = 0; a < verts.Length; a++)
			verts [a] = verts [a] *= size;

		//隣接するチャンクの地形に合わせ頂点を変位させる
		for (int a = 0; a < verts.Length; a++) {
			Vector3 v0 = verts [a];
			v0.y = Random.Range (0f, height);
			setVert (verts, verts [a], v0);
			for (int b = 0; b < points.Count; b++) {
				if (verts [a].x == points [b].x && verts [a].z == points [b].z) {
					setVert (verts, verts [a], points [b]);
					points.RemoveAt (b);
					break;
				}
			}
		}
		mesh.vertices = verts;

		//細分化を行い、上記と同じように隣接するチャンクの地形に合わせ頂点を変位させる
		for (int a = 0; a < fineness; a++) {
			int b = mesh.vertices.Length;

			mesh = BPMesh.Subdivide_Half (mesh, false);

			verts = mesh.vertices;
			while (b < verts.Length) {
				bool c = true;

				for (int e = 0; e < points.Count; e++) {
					if (verts [b].x == points [e].x && verts [b].z == points [e].z) {
						c = false;
						setVert (verts, verts [b], points [e]);
						points.RemoveAt (e);
						break;
					}
				}

				if (c) {
					float d = height / Mathf.Pow (2, a + 1);
					setVert (verts, verts [b], verts [b] + Vector3.up * Random.Range (-d, d));
				}

				b++;
			}
			mesh.vertices = verts;
		}
		mesh.RecalculateNormals ();
		return mesh;
	}

	public static IEnumerator getBPFractalTerrainAsync (MonoBehaviour behaviour, int fineness, float size, float height) {
		yield return getBPFractalTerrainAsync (behaviour, fineness, size, height, new List<Vector3> ());
	}

	public static IEnumerator getBPFractalTerrainAsync (MonoBehaviour behaviour, int fineness, float size, float height, List<Vector3> points) {
		Mesh mesh = getQuadTerrain ();

		Vector3[] verts = mesh.vertices;

		for (int a = 0; a < verts.Length; a++)
			verts [a] = verts [a] *= size;

		for (int a = 0; a < verts.Length; a++) {
			Vector3 v0 = verts [a];
			v0.y = Random.Range (0f, height);
			setVert (verts, verts [a], v0);
			for (int b = 0; b < points.Count; b++) {
				//if (Main.yrCondition ())
				//	yield return null;
				if (verts [a].x == points [b].x && verts [a].z == points [b].z) {
					setVert (verts, verts [a], points [b]);
					points.RemoveAt (b);
					break;
				}
			}
		}

		mesh.vertices = verts;

		for (int a = 0; a < fineness; a++) {
			int b = mesh.vertices.Length;

			mesh = BPMesh.Subdivide_Half (mesh, false);

			verts = mesh.vertices;
			while (b < verts.Length) {
				bool c = true;

				for (int e = 0; e < points.Count; e++) {
					//if (Main.yrCondition ())
					//	yield return null;
					if (verts [b].x == points [e].x && verts [b].z == points [e].z) {
						c = false;
						setVert (verts, verts [b], points [e]);
						points.RemoveAt (e);
						break;
					}
				}

				if (c) {
					float d = height / Mathf.Pow (2, a + 1);
					//TODO vertsが多いため絞り込むアルゴリズムを考える必要がある。
					//verts[b]
					setVert (verts, verts [b], verts [b] + Vector3.up * Random.Range (-d, d));
					//yield return behaviour.StartCoroutine (setVertAsync (verts, verts [b], verts [b] + Vector3.up * Random.Range (-d, d)));
				}

				b++;
			}
			mesh.vertices = verts;
		}
		mesh.RecalculateNormals ();
		yield return mesh;
	}

	public static void mesh_move (Vector3[] verts, Vector3 vec) {
		for (int a = 0; a < verts.Length; a++)
			verts [a] = new Vector3 (verts [a].x + vec.x, verts [a].y + vec.y, verts [a].z + vec.z);
	}

	public static void mesh_rotate (Vector3[] verts, Quaternion rot) {
		for (int a = 0; a < verts.Length; a++)
			verts [a] = rot * verts [a];
	}
	
	/*public static Vector3 getIntersectionPoint (Mesh mesh) {

		return Vector3.zero;
	}

	public static Mesh Quad2Tri (Mesh mesh) {
		Mesh m = mesh_copy (mesh);

		return m;
	}*/

	//円を生成
	public static Mesh circle (float radius, int verts) {
		Mesh mesh = new Mesh ();
		Vector3[] vs = new Vector3[verts * 3];
		Vector2[] uv = new Vector2[verts * 3];
		int[] tris = new int[verts * 3];

		for (int a = 0; a < verts; a++) {
			vs [a] = new Vector3 (Mathf.Cos (Mathf.PI * 2 / verts * a) * radius, 0, Mathf.Sin (Mathf.PI * 2 / verts * a) * radius);
			vs [verts + a] = new Vector3 (0, 0, 0);
			vs [verts * 2 + a] = new Vector3 (Mathf.Cos (Mathf.PI * 2 / verts * (a + 1)) * radius, 0, Mathf.Sin (Mathf.PI * 2 / verts * (a + 1)) * radius);

			uv [a] = new Vector2 ();
			uv [verts + a] = new Vector2 ();//TODO
			uv [verts * 2 + a] = new Vector2 ();

			tris [a * 3] = a;
			tris [a * 3 + 1] = verts + a;
			tris [a * 3 + 2] = verts * 2 + a;

			mesh.vertices = vs;
			mesh.uv = uv;
			mesh.triangles = tris;
		}
		return mesh;
	}

	//円筒を生成
	public static Mesh cylindrical_surface (float radius, float height, int verts) {
		Mesh mesh = new Mesh ();
		Vector3[] vs = new Vector3[verts * 6];
		Vector2[] uv = new Vector2[verts * 6];
		int[] tris = new int[verts * 6];

		for (int a = 0; a < verts; a++) {
			float x1 = Mathf.Cos (Mathf.PI * 2 / verts * a) * radius;
			float z1 = Mathf.Sin (Mathf.PI * 2 / verts * a) * radius;
			float x2 = Mathf.Cos (Mathf.PI * 2 / verts * (a + 1)) * radius;
			float z2 = Mathf.Sin (Mathf.PI * 2 / verts * (a + 1)) * radius;
			vs [a] = new Vector3 (x1, 0, z1);
			vs [verts + a] = new Vector3 (x1, height, z1);
			vs [verts * 2 + a] = new Vector3 (x2, height, z2);
			vs [verts * 3 + a] = new Vector3 (x1, 0, z1);
			vs [verts * 4 + a] = new Vector3 (x2, height, z2);
			vs [verts * 5 + a] = new Vector3 (x2, 0, z2);

			uv [a] = new Vector2 ();
			uv [verts + a] = new Vector2 ();//TODO
			uv [verts * 2 + a] = new Vector2 ();
			uv [verts * 3 + a] = new Vector2 ();
			uv [verts * 4 + a] = new Vector2 ();
			uv [verts * 5 + a] = new Vector2 ();

			tris [a * 6] = a;
			tris [a * 6 + 1] = verts + a;
			tris [a * 6 + 2] = verts * 2 + a;

			tris [a * 6 + 3] = verts * 3 + a;
			tris [a * 6 + 4] = verts * 4 + a;
			tris [a * 6 + 5] = verts * 5 + a;

			mesh.vertices = vs;
			mesh.uv = uv;
			mesh.triangles = tris;
		}
		return mesh;
	}

	//円柱（角柱）を作成
	//TODO スムーズ面に対応させる
	public static Mesh cylinder (float radius, float height, int verts) {
		Mesh floor = circle (radius, verts);
		Mesh ceil = circle (radius, verts);
		Vector3[] v = floor.vertices;
		mesh_rotate (v, Quaternion.Euler (-180, 0, 0));
		floor.vertices = v;
		v = ceil.vertices;
		mesh_move (v, Vector3.up * height);
		ceil.vertices = v;

		return mesh_combine (mesh_combine (floor, ceil), cylindrical_surface (radius, height, verts));
	}

	//円錐を作成
	//TODO スムーズ化
	public static Mesh getCone (float radius, float height, int verts) {
		Mesh mesh = new Mesh ();

		Vector3[] vs = new Vector3[verts * 6];
		Vector2[] uv = new Vector2[verts * 6];
		int[] tris = new int[verts * 6];

		for (int a = 0; a < verts; a++) {
			float x1 = Mathf.Cos (Mathf.PI * 2 / verts * a) * radius;
			float z1 = Mathf.Sin (Mathf.PI * 2 / verts * a) * radius;
			float x2 = Mathf.Cos (Mathf.PI * 2 / verts * (a + 1)) * radius;
			float z2 = Mathf.Sin (Mathf.PI * 2 / verts * (a + 1)) * radius;
			vs [a] = new Vector3 (0, 0, 0);
			vs [verts + a] = new Vector3 (x1, 0, z1);
			vs [verts * 2 + a] = new Vector3 (x2, 0, z2);
			vs [verts * 3 + a] = new Vector3 (x1, 0, z1);
			vs [verts * 4 + a] = new Vector3 (0, height, 0);
			vs [verts * 5 + a] = new Vector3 (x2, 0, z2);

			uv [a] = new Vector2 ();
			uv [verts + a] = new Vector2 ();//TODO
			uv [verts * 2 + a] = new Vector2 ();
			uv [verts * 3 + a] = new Vector2 ();
			uv [verts * 4 + a] = new Vector2 ();
			uv [verts * 5 + a] = new Vector2 ();

			tris [a * 6] = a;
			tris [a * 6 + 1] = verts + a;
			tris [a * 6 + 2] = verts * 2 + a;

			tris [a * 6 + 3] = verts * 3 + a;
			tris [a * 6 + 4] = verts * 4 + a;
			tris [a * 6 + 5] = verts * 5 + a;
		}

		mesh.vertices = vs;
		mesh.uv = uv;
		mesh.triangles = tris;

		return mesh;
	}

	//TODO 切り出し

	//TODO 樹木を生成
	public static Mesh generateTree (float radius) {
		Mesh mesh = new Mesh ();

		return mesh;
	}

	/*public static Mesh[] VoronoiBreak (Mesh mesh) {
		//TODO ボロノイ図状に崩壊
		return null;
	}*/

	//TODO ソリッドモデルの結合
	/*public static Mesh SolidCombine (Mesh target, Mesh source) {

	}*/
}
