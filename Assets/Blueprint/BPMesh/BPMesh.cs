using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMesh {

	//メッシュの複製
	public static Mesh mesh_copy (Mesh mesh) {
		Mesh m = new Mesh ();
		m.vertices = mesh.vertices;
		m.uv = mesh.uv;
		m.triangles = mesh.triangles;

		return m;
	}

	//TODO フラット面とスムーズ面を判別して細分化

	//メッシュの細分化
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

	//頂点及びUVが一致する頂点をまとめる。スムーズ面になってしまうためおすすめはしない。
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
		for (int a = 0; a < verts.Length; a++) {
			if (verts [a] == target) {
				verts [a] = result;
			}
		}
	}

	public static IEnumerator setVertAsync(Vector3[] verts, Vector3 target, Vector3 result) {
		for (int a = 0; a < verts.Length; a++) {
			if (1 <= Time.deltaTime * Application.targetFrameRate) {
				yield return null;
			}
			if (verts [a] == target) {
				verts [a] = result;
			}
		}
	}

	public static void setVert(List<Vector3> verts, Vector3 target, Vector3 result) {
		for (int a = 0; a < verts.Count; a++) {
			if (verts [a] == target) {
				verts [a] = result;
			}
		}
	}

	public static IEnumerator setVertAsync(List<Vector3> verts, Vector3 target, Vector3 result) {
		for (int a = 0; a < verts.Count; a++) {
			if (1 <= Time.deltaTime * Application.targetFrameRate) {
				yield return null;
			}
			if (verts [a] == target) {
				verts [a] = result;
			}
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
	public static IEnumerator getBPFractalTerrain (MonoBehaviour behaviour, int fineness, float size, float height) {
		yield return getBPFractalTerrain (behaviour, fineness, size, height, new Vector3[0]);
	}

	//pointsに点群データを入れておくことで、X,Zが一致する点のYを点群データに合わせることが出来る。
	//チャンクに対応した地形を生成する際に使用する。
	public static IEnumerator getBPFractalTerrain (MonoBehaviour behaviour, int fineness, float size, float height, Vector3[] points) {
		Mesh mesh = getQuadTerrain ();

		Vector3[] verts = mesh.vertices;

		scale (verts, size);

		for (int a = 0; a < verts.Length; a++) {
			Vector3 v0 = verts [a];
			v0.y = Random.Range (0f, height);
			if (behaviour == null)
				setVert (verts, verts [a], v0);
			else
				yield return behaviour.StartCoroutine (setVertAsync (verts, verts [a], v0));
			for (int b = 0; b < points.Length; b++) {
				if (1 <= Time.deltaTime * Application.targetFrameRate) {
					yield return null;
				}
				if (verts [a].x == points [b].x && verts [a].z == points [b].z) {
					if (behaviour == null)
						setVert (verts, verts [a], points [b]);
					else
						yield return behaviour.StartCoroutine (setVertAsync (verts, verts [a], points [b]));
					break;
				}
			}
		}

		mesh.vertices = verts;

		for (int a = 0; a < fineness; a++) {
			yield return null;
			int b = mesh.vertices.Length;

			mesh = BPMesh.Subdivide_Half (mesh, false);

			verts = mesh.vertices;
			while (b < verts.Length) {
				if (1 <= Time.deltaTime * Application.targetFrameRate) {
					yield return null;
				}

				bool c = true;

				for (int e = 0; e < points.Length; e++) {
					if (1 <= Time.deltaTime * Application.targetFrameRate) {
						yield return null;
					}
					if (verts [b].x == points [e].x && verts [b].z == points [e].z) {
						c = false;
						if (behaviour == null)
							setVert (verts, verts [b], points [e]);
						else
							yield return behaviour.StartCoroutine (setVertAsync (verts, verts [b], points [e]));
						break;
					}
				}

				if (c) {
					float d = height / Mathf.Pow (2, a + 1);
					if (behaviour == null)
						setVert (verts, verts [b], verts [b] + Vector3.up * Random.Range (-d, d));
					else
						yield return behaviour.StartCoroutine (setVertAsync (verts, verts [b], verts [b] + Vector3.up * Random.Range (-d, d)));
				}

				b++;
			}
			mesh.vertices = verts;
		}
		mesh.RecalculateNormals ();
		yield return mesh;
	}

	public static void scale (Vector3[] verts, float scale) {
		for (int a = 0; a < verts.Length; a++) {
			verts [a] *= scale;
		}
	}

	public static void scale (Vector3[] verts, Vector3 scale) {
		for (int a = 0; a < verts.Length; a++) {
			verts [a] = new Vector3 (verts [a].x * scale.x, verts [a].y * scale.y, verts [a].z * scale.z);
		}
	}

	//TODO フラット面化
	//TODO スムーズ面化

	/*public static Vector3 getIntersectionPoint (Mesh mesh) {

		return Vector3.zero;
	}

	public static Mesh Combine (Mesh mesh1, Mesh mesh2) {

		return null;
	}

	public static Mesh Quad2Tri (Mesh mesh) {
		Mesh m = mesh_copy (mesh);

		return m;
	}*/

	//円柱を作成
	public static Mesh getCylinder (float radius, float height, int verts) {
		Mesh mesh = new Mesh ();

		Vector3[] vs = new Vector3[verts * 12];
		Vector2[] uv = new Vector2[verts * 12];
		int[] tris = new int[verts * 12];

		for (int a = 0; a < verts; a++) {
			float x1 = Mathf.Cos (Mathf.PI * 2 / verts * a) * radius;
			float z1 = Mathf.Sin (Mathf.PI * 2 / verts * a) * radius;
			float x2 = Mathf.Cos (Mathf.PI * 2 / verts * (a + 1)) * radius;
			float z2 = Mathf.Sin (Mathf.PI * 2 / verts * (a + 1)) * radius;
			vs [a] = new Vector3 (0, 0, 0);
			vs [verts + a] = new Vector3 (x1, 0, z1);
			vs [verts * 2 + a] = new Vector3 (x2, 0, z2);
			vs [verts * 3 + a] = new Vector3 (x1, 0, z1);
			vs [verts * 4 + a] = new Vector3 (x1, height, z1);
			vs [verts * 5 + a] = new Vector3 (x2, height, z2);
			vs [verts * 6 + a] = new Vector3 (x1, 0, z1);
			vs [verts * 7 + a] = new Vector3 (x2, height, z2);
			vs [verts * 8 + a] = new Vector3 (x2, 0, z2);
			vs [verts * 9 + a] = new Vector3 (x1, height, z1);
			vs [verts * 10 + a] = new Vector3 (0, height, 0);
			vs [verts * 11 + a] = new Vector3 (x2, height, z2);

			uv [a] = new Vector2 (0, 0);
			uv [verts + a] = new Vector2 ();//TODO
			uv [verts * 2 + a] = new Vector2 ();
			uv [verts * 3 + a] = new Vector2 ();
			uv [verts * 4 + a] = new Vector2 ();
			uv [verts * 5 + a] = new Vector2 ();
			uv [verts * 6 + a] = new Vector2 ();
			uv [verts * 7 + a] = new Vector2 ();
			uv [verts * 8 + a] = new Vector2 ();
			uv [verts * 9 + a] = new Vector2 ();
			uv [verts * 10 + a] = new Vector2 (0, 0);
			uv [verts * 11 + a] = new Vector2 ();

			tris [a * 12] = a;
			tris [a * 12 + 1] = verts + a;
			tris [a * 12 + 2] = verts * 2 + a;

			tris [a * 12 + 3] = verts * 3 + a;
			tris [a * 12 + 4] = verts * 4 + a;
			tris [a * 12 + 5] = verts * 5 + a;

			tris [a * 12 + 6] = verts * 6 + a;
			tris [a * 12 + 7] = verts * 7 + a;
			tris [a * 12 + 8] = verts * 8 + a;

			tris [a * 12 + 9] = verts * 9 + a;
			tris [a * 12 + 10] = verts * 10 + a;
			tris [a * 12 + 11] = verts * 11 + a;
		}

		mesh.vertices = vs;
		mesh.uv = uv;
		mesh.triangles = tris;

		return mesh;
	}

	//TODO 円錐を作成
	/*public static Mesh getCone () {

	}*/

	//TODO 切り出し

	//TODO 樹木を生成

	public static Mesh[] VoronoiBreak (Mesh mesh) {
		//ボロノイ図状に崩壊
		return null;
	}

}
