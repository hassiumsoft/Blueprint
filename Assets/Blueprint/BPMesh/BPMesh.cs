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
		List<int> newtris = new List<int> ();

		for (int c = 0; c <= m.triangles.Length - 3; c += 3) {
			int vn0 = m.triangles [c];
			int vn1 = m.triangles [c + 1];
			int vn2 = m.triangles [c + 2];

			newverts.Add ((m.vertices [vn0] + m.vertices [vn1]) / 2);
			newverts.Add ((m.vertices [vn1] + m.vertices [vn2]) / 2);
			newverts.Add ((m.vertices [vn2] + m.vertices [vn0]) / 2);

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
		m.triangles = newtris.ToArray ();

		recalc (m);

		return m;
	}

	public static Mesh clear_doubles (Mesh mesh) {
		Mesh m = mesh_copy (mesh);

		return m;
	}

	public static Mesh Combine (Mesh mesh1, Mesh mesh2) {

		return null;
	}

	public static Mesh Quad2Tri (Mesh mesh) {
		Mesh m = mesh_copy (mesh);

		return m;
	}
}
