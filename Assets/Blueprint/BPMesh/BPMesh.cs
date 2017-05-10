using System.Collections.Generic;
using UnityEngine;

public class BPMesh
{
	public static Mesh mesh_copy (Mesh mesh) {
		Mesh m = new Mesh ();
		m.vertices = mesh.vertices;
		m.uv = mesh.uv;
		m.triangles = mesh.triangles;
		recalc (m);
		return m;
	}

	public static void recalc (Mesh mesh) {
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
	}

	public static Mesh Subdivide_Half (Mesh mesh)
	{
		Mesh m = mesh_copy (mesh);

		List<Vector3> newverts = new List<Vector3> (m.vertices);
		List<Vector2> newuv = new List<Vector2> (m.uv);
		List<int> newtris = new List<int> ();

		for (int c = 0; c <= m.triangles.Length - 3; c += 3) {
			int vn0 = m.triangles [c];
			int vn1 = m.triangles [c + 1];
			int vn2 = m.triangles [c + 2];

			Vector2 vu0 = m.uv [vn0];
			Vector2 vu1 = m.uv [vn1];
			Vector2 vu2 = m.uv [vn2];

			newverts.Add ((m.vertices [vn0] + m.vertices [vn1]) / 2);
			newverts.Add ((m.vertices [vn1] + m.vertices [vn2]) / 2);
			newverts.Add ((m.vertices [vn2] + m.vertices [vn0]) / 2);

			newuv.Add ((vu0 + vu1) / 2);
			newuv.Add ((vu1 + vu2) / 2);
			newuv.Add ((vu2 + vu0) / 2);

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

		m.vertices = newverts.ToArray ();
		m.uv = newuv.ToArray ();
		m.triangles = newtris.ToArray ();

		recalc (m);

		return m;
	}

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

	public static void setVert(Vector3[] verts, Vector3 target, Vector3 result) {
		for (int a = 0; a < verts.Length; a++) {
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

	public static Mesh getQuadTerrain () {
		Mesh mesh = new Mesh ();

		mesh.vertices = new Vector3[]{ Vector3.zero, Vector3.forward, Vector3.right + Vector3.forward, Vector3.right };
		mesh.uv = new Vector2[]{ Vector2.zero, Vector2.up, Vector2.right + Vector2.up, Vector2.right };
		mesh.triangles = new int[]{ 0, 1, 2, 2, 3, 0 };
		recalc (mesh);

		return mesh;
	}

	public static Mesh getTriangleTerrain () {
		Mesh mesh = new Mesh ();

		mesh.vertices = new Vector3[]{ Vector3.zero, Vector3.forward, Vector3.right };
		mesh.uv = new Vector2[]{ Vector2.zero, Vector2.up, Vector2.right };
		mesh.triangles = new int[]{ 0, 1, 2 };
		recalc (mesh);

		return mesh;
	}

	//中点変位法を使用したフラクタル地形
	public static Mesh getBPFractalTerrain (int fineness, float height) {
		Mesh mesh = getQuadTerrain ();
		//Mesh mesh = getTriangleTerrain ();
		
		Vector3[] verts = mesh.vertices;

		for (int a = 0; a < verts.Length; a++) {
			verts [a].y = Random.Range (0f, height);
		}

		mesh.vertices = verts;

		for (int a = 0; a < fineness; a++) {
			int b = mesh.vertices.Length;
			mesh = BPMesh.Remove_Doubles (BPMesh.Subdivide_Half (mesh));

			verts = mesh.vertices;
			while (b < verts.Length) {
				float c = height / 2 / (a + 1);
				setVert (verts, verts [b], verts [b] + Vector3.up * Random.Range (-c, c));
				b++;
			}
			mesh.vertices = verts;
		}

		return mesh;
	}

	public static Vector3 getIntersectionPoint (Mesh mesh) {

		return Vector3.zero;//TODO
	}

	public static Mesh Combine (Mesh mesh1, Mesh mesh2) {

		return null;
	}

	public static Mesh Quad2Tri (Mesh mesh) {
		Mesh m = mesh_copy (mesh);

		return m;
	}

	public static Mesh[] VoronoiBreak (Mesh mesh) {
		//ボロノイ図状に崩壊
		return null;
	}

	//切り出し
}
