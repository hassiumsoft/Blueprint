using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMesh {

	//TODO UVが一致していない場合にもスムーズ面化出来るようにする
	//TODO フラット面化
	//TODO 使われていない頂点を削除

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

		List<Vector3> a = new List<Vector3> (target.vertices.Length + source.vertices.Length);
		a.AddRange (target.vertices);
		a.AddRange (source.vertices);
		m.vertices = a.ToArray ();

		List<Vector2> b = new List<Vector2> (target.uv.Length + source.uv.Length);
		b.AddRange (target.uv);
		b.AddRange (source.uv);
		m.uv = b.ToArray ();

		b = new List<Vector2> (target.uv2.Length + source.uv2.Length);
		b.AddRange (target.uv2);
		b.AddRange (source.uv2);
		m.uv2 = b.ToArray ();

		b = new List<Vector2> (target.uv3.Length + source.uv3.Length);
		b.AddRange (target.uv3);
		b.AddRange (source.uv3);
		m.uv3 = b.ToArray ();

		b = new List<Vector2> (target.uv4.Length + source.uv4.Length);
		b.AddRange (target.uv4);
		b.AddRange (source.uv4);
		m.uv4 = b.ToArray ();

		List<int> c = new List<int> (target.triangles.Length + source.triangles.Length);
		c.AddRange (target.triangles);
		foreach (int e in source.triangles)
			c.Add (e + target.triangles.Length);
		m.triangles = c.ToArray ();

		return m;
	}

	//メッシュの細分化（フラット面になるので注意）
	public static Mesh Subdivide_Half (Mesh mesh) {
		Mesh m = mesh_copy (mesh);

		List<Vector3> newverts = new List<Vector3> (m.vertices);
		List<Vector2> newuv = new List<Vector2> (m.uv);
		List<int> newtris = new List<int> ();

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

		m.vertices = newverts.ToArray ();
		m.uv = newuv.ToArray ();
		m.triangles = newtris.ToArray ();

		return m;
	}

	//三角ポリゴンの中から、2つの点を結ぶ辺を持つ面の、第三の点のインデックスを返す。見つからない場合は-1を返す。
	/*public static int[] aaa(int[] triangles, int[] p1, int[] p2) {
		int[] r = new int[p1.Length];

		for (int a = 0; a < r.Length; a++) {
			for (int b = 0; b < triangles.Length - 3; b += 3) {
				int p1_ = -1;
				if (triangles [b] == p1 [a]) {
					p1_ = 0;
				} else if (triangles [b + 1] == p1 [a]) {
					p1_ = 1;
				} else if (triangles [b + 2] == p1 [a]) {
					p1_ = 2;
				}

				if (p1_ == -1) {
					r [a] = -1;
				} else {
					if (p1_ != 0 && triangles [b] == p2 [a]) {
						if (p1_ == 2) {
							r [a] = triangles [b + 1];
						} else {
							r [a] = triangles [b + 2];
						}
					} else if (p1_ != 1 && triangles [b + 1] == p2 [a]) {
						if (p1_ == 2) {
							r [a] = triangles [b];
						} else {
							r [a] = triangles [b + 2];
						}
					} else if (p1_ != 2 && triangles [b + 2] == p2 [a]) {
						if (p1_ == 1) {
							r [a] = triangles [b];
						} else {
							r [a] = triangles [b + 1];
						}
					}
				}
			}
		}

		return r;
	}*/

	//頂点及びUVが一致する頂点をまとめてスムーズ面化
	public static Mesh Remove_Doubles (Mesh mesh, int start = 0, int end = -1) {
		Mesh m = mesh_copy (mesh);

		List<Vector3> verts = new List<Vector3> (m.vertices);
		int[] tris = m.triangles;

		if (end == -1)
			end = verts.Count;

		for (int b = start; b < end; b++) {
			for (int c = b + 1; c < end; c++) {
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
	public static void setVert(Vector3[] verts, Vector3 target, Vector3 result) {
		for (int n = 0; n < verts.Length; n++)
			if (verts [n] == target)
				verts [n] = result;
	}

	public static void setVert(Vector3[] verts, Vector3 target, Vector3 result, int start, int end) {
		//"n < verts.Length"や"n < verts.Count"はエラー回避なしでも出来る場合がほとんどであるため、
		//繰り返し処理の負担を避けるためコメントアウトしている。
		for (int n = start; /*n < verts.Length && */n < end; n++)
			if (verts [n] == target)
				verts [n] = result;
	}

	public static IEnumerator setVertAsync(Vector3[] verts, Vector3 target, Vector3 result) {
		for (int n = 0; n < verts.Length; n++) {
			if (Main.yrCondition ())
				yield return new WaitForEndOfFrame();
			if (verts [n] == target)
				verts [n] = result;
		}
	}

	public static IEnumerator setVertAsync(Vector3[] verts, Vector3 target, Vector3 result, int start, int end) {
		for (int n = start; /*n < verts.Length && */n < end; n++) {
			if (Main.yrCondition ())
				yield return new WaitForEndOfFrame();
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
				yield return new WaitForEndOfFrame();
			if (verts [n] == target)
				verts [n] = result;
		}
	}

	public static IEnumerator setVertAsync(List<Vector3> verts, Vector3 target, Vector3 result, int start, int end) {
		for (int n = start; /*n < verts.Count && */n < end; n++) {
			if (Main.yrCondition ())
				yield return new WaitForEndOfFrame();
			if (verts [n] == target)
				verts [n] = result;
		}
	}

	//未使用
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
	public static Mesh getQuadTerrain (float size, bool smooth) {
		Mesh mesh = new Mesh ();

		Vector3 f = Vector3.forward * size;
		Vector3 r = Vector3.right * size;
		Vector3 rf = f + r;
		Vector3 c = rf / 2;

		Vector2 ru = Vector2.right + Vector2.up;
		Vector2 c2 = ru / 2;

		if (smooth) {
			mesh.vertices = new Vector3[] {
				Vector3.zero,
				f,
				r,
				rf,
				c
			};

			mesh.uv = new Vector2[] {
				Vector2.zero,
				Vector2.up,
				Vector2.right,
				ru,
				c2
			};

			mesh.triangles = new int[]{ 0, 1, 4, 1, 3, 4, 3, 2, 4, 2, 0, 4 };
		} else {
			mesh.vertices = new Vector3[] {
				Vector3.zero,
				f,
				c,
				f,
				rf,
				c,
				rf,
				r,
				c,
				r,
				Vector3.zero,
				c
			};

			mesh.uv = new Vector2[] {
				Vector2.zero,
				Vector2.up,
				c2,
				Vector2.up,
				ru,
				c2,
				ru,
				Vector2.right,
				c2,
				Vector2.right,
				Vector2.zero,
				c2
			};

			mesh.triangles = new int[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
		}

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
		Mesh mesh = getQuadTerrain (size, true);
		Vector3[] verts = mesh.vertices;

		//点群データに合わせ頂点を変位させる
		for (int a = 0; a < verts.Length; a++) {
			Vector3 v0 = verts [a];

			bool b = true;
			for (int c = 0; c < points.Count; c++) {
				if (verts [a].x == points [c].x && verts [a].z == points [c].z) {
					b = false;
					verts [a] = points [c];
					points.RemoveAt (c);
					break;
				}
			}

			if (b) {
				v0.y = Random.Range (0f, height);
				verts [a] = v0;
			}
		}
		mesh.vertices = verts;

		//細分化を行い、上記と同じように隣接するチャンクの地形に合わせ頂点を変位させる
		for (int a = 0; a < fineness; a++) {
			int b = mesh.vertices.Length;

			mesh = Remove_Doubles (Subdivide_Half (mesh));

			verts = mesh.vertices;
			while (b < verts.Length) {
				bool c = true;

				if (verts [b].x == 0 || verts [b].x == Chunk.size || verts [b].z == 0 || verts [b].z == Chunk.size) {
					for (int e = 0; e < points.Count; e++) {
						if (verts [b].x == points [e].x && verts [b].z == points [e].z) {
							c = false;
							verts [b] = points [e];
							points.RemoveAt (e);
							break;
						}
					}
				}

				if (c) {
					float d = height / Mathf.Pow (2, a + 1);
					verts [b] = verts [b] + Vector3.up * Random.Range (-d, d);
				}

				b++;
			}
			mesh.vertices = verts;
		}
		mesh = Remove_Doubles (mesh);
		mesh.RecalculateNormals ();
		return mesh;
	}

	public static IEnumerator getBPFractalTerrainAsync (MonoBehaviour behaviour, int fineness, float size, float height) {
		yield return getBPFractalTerrainAsync (behaviour, fineness, size, height, new List<Vector3> ());
	}

	public static IEnumerator getBPFractalTerrainAsync (MonoBehaviour behaviour, int fineness, float size, float height, List<Vector3> points) {
		Mesh mesh = getQuadTerrain (size, true);
		Vector3[] verts = mesh.vertices;

		for (int a = 0; a < verts.Length; a++) {
			Vector3 v0 = verts [a];

			bool b = true;
			for (int c = 0; c < points.Count; c++) {
				if (verts [a].x == points [c].x && verts [a].z == points [c].z) {
					b = false;
					verts [a] = points [c];
					points.RemoveAt (c);
					break;
				}
			}

			if (b) {
				v0.y = Random.Range (0f, height);
				verts [a] = v0;
			}
		}

		mesh.vertices = verts;

		for (int a = 0; a < fineness; a++) {
			if (Main.yrCondition ())
				yield return new WaitForEndOfFrame();
			int b = mesh.vertices.Length;

			mesh = Remove_Doubles (Subdivide_Half (mesh));

			verts = mesh.vertices;
			while (b < verts.Length) {
				bool c = true;

				if (verts [b].x == 0 || verts [b].x == Chunk.size || verts [b].z == 0 || verts [b].z == Chunk.size) {
					for (int e = 0; e < points.Count; e++) {
						if (Main.yrCondition ())
							yield return new WaitForEndOfFrame();
						if (verts [b].x == points [e].x && verts [b].z == points [e].z) {
							c = false;
							verts [b] = points [e];
							points.RemoveAt (e);
							break;
						}
					}
				}

				if (c) {
					float d = height / Mathf.Pow (2, a + 1);
					verts [b] = verts [b] + Vector3.up * Random.Range (-d, d);
				}

				b++;
			}
			mesh.vertices = verts;
		}
		mesh = Remove_Doubles (mesh);
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
			float w1 = (float)a / verts;
			float w2 = (float)(a + 1) / verts;

			vs [a] = new Vector3 (Mathf.Cos (Mathf.PI * 2 * w1) * radius, 0, Mathf.Sin (Mathf.PI * 2 * w1) * radius);
			vs [verts + a] = new Vector3 (0, 0, 0);
			vs [verts * 2 + a] = new Vector3 (Mathf.Cos (Mathf.PI * 2 * w2) * radius, 0, Mathf.Sin (Mathf.PI * 2 * w2) * radius);

			uv [a] = new Vector2 (w1, 0);
			uv [verts + a] = new Vector2 (0, 1);
			uv [verts * 2 + a] = new Vector2 (w2, 0);

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
	//TODO スムーズ面
	public static Mesh cylindrical_surface (float floor_radius, float ceil_radius, float height, int verts) {
		Mesh mesh = new Mesh ();
		Vector3[] vs = new Vector3[verts * 6];
		Vector2[] uv = new Vector2[verts * 6];
		int[] tris = new int[verts * 6];

		for (int a = 0; a < verts; a++) {
			float w1 = (float)a / verts;
			float w2 = (float)(a + 1) / verts;

			float xf1 = Mathf.Cos (Mathf.PI * 2 * w1) * floor_radius;
			float zf1 = Mathf.Sin (Mathf.PI * 2 * w1) * floor_radius;
			float xf2 = Mathf.Cos (Mathf.PI * 2 * w2) * floor_radius;
			float zf2 = Mathf.Sin (Mathf.PI * 2 * w2) * floor_radius;
			float xc1 = Mathf.Cos (Mathf.PI * 2 * w1) * ceil_radius;
			float zc1 = Mathf.Sin (Mathf.PI * 2 * w1) * ceil_radius;
			float xc2 = Mathf.Cos (Mathf.PI * 2 * w2) * ceil_radius;
			float zc2 = Mathf.Sin (Mathf.PI * 2 * w2) * ceil_radius;
			vs [a] = new Vector3 (xf1, 0, zf1);
			vs [verts + a] = new Vector3 (xc1, height, zc1);
			vs [verts * 2 + a] = new Vector3 (xc2, height, zc2);
			vs [verts * 3 + a] = new Vector3 (xf1, 0, zf1);
			vs [verts * 4 + a] = new Vector3 (xc2, height, zc2);
			vs [verts * 5 + a] = new Vector3 (xf2, 0, zf2);

			uv [a] = new Vector2 (w1, 0);
			uv [verts + a] = new Vector2 (w1, 1);
			uv [verts * 2 + a] = new Vector2 (w2, 1);
			uv [verts * 3 + a] = new Vector2 (w1, 0);
			uv [verts * 4 + a] = new Vector2 (w2, 1);
			uv [verts * 5 + a] = new Vector2 (w2, 0);

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
	//TODO スムーズ面
	public static Mesh cylinder (float floor_radius, float ceil_radius, float height, int verts) {
		Mesh floor = circle (floor_radius, verts);
		Mesh ceil = circle (ceil_radius, verts);
		Vector3[] v = floor.vertices;
		mesh_rotate (v, Quaternion.Euler (-180, 0, 0));
		floor.vertices = v;
		v = ceil.vertices;
		mesh_move (v, Vector3.up * height);
		ceil.vertices = v;

		return mesh_combine (mesh_combine (floor, ceil), cylindrical_surface (floor_radius, ceil_radius, height, verts));
	}

	//TODO 切り出し

	//TODO 樹木を生成
	public static Mesh generateTree (TreeInfo info) {
		float radius = info.getRadius ();
		float height = info.getHeight ();
		float bdh = info.getBranchDownHeight ();

		Mesh mesh = new Mesh ();

		//幹を作成
		for (float a = 0; a < height; a += 1f) {
			Mesh mesh_a = cylindrical_surface (Mathf.Lerp (radius, 0f, a / height), Mathf.Lerp (radius, 0f, (a + 1) / height), a + 1f < height ? 1f : height - a, 12);
			if (a != 0) {
				Vector3[] verts = mesh_a.vertices;
				mesh_move (verts, Vector3.up * a);
				mesh_a.vertices = verts;
			}
			mesh = mesh_combine (mesh, mesh_a);
		}

		//枝を作成
		for (float a = bdh; a < height; a += 0.2f) {

		}

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
