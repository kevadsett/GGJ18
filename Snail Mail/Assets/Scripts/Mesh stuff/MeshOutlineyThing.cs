using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode ()]
[RequireComponent (typeof (Renderer))]
[RequireComponent (typeof (PolygonCollider2D))]
[RequireComponent (typeof (MeshFilter))]
public class MeshOutlineyThing : MonoBehaviour {
	public bool dirty;
	public float depth;

	PolygonCollider2D pc2d;
	MeshFilter mf;

	void Awake () {
		pc2d = GetComponent<PolygonCollider2D> ();
		mf = GetComponent<MeshFilter> ();

		mf.sharedMesh = Generate ();
	}

	void Update () {
		if (Application.isEditor == false) {
			enabled = false;
			return;
		}

		if (dirty) {
			mf.sharedMesh = Generate ();
			dirty = false;
		}
	}

	Mesh Generate () {
		Mesh mesh = new Mesh ();

		var points = pc2d.points;

		var triangulator = new Triangulator (points);
		var triangulated = triangulator.Triangulate ();

		var verts = new Vector3 [points.Length * 2];
		var uvs = new Vector2 [points.Length * 2];

		// MAKE ALL THE POINTS BUT LIKE DOUBLE SO WE CAN MAKE SIDENESS

		for (int i = 0; i < points.Length; i++) {
			var v2d = points [i];

			verts [i] = new Vector3 (v2d.x, v2d.y, 0f);
			verts [points.Length + i] = new Vector3 (v2d.x, v2d.y, depth);

			uvs [i] = new Vector2 (v2d.x * 0.5f + 0.5f, v2d.y * 0.5f + 0.5f);
			uvs [points.Length + i] = uvs [i];
		}

		var tris = new int [(triangulated.Length * 2) + (points.Length * 6)];

		// MAKE THE TWO FACES OF THE CUTOUT THING
		for (int i = 0; i < triangulated.Length; i++) {
			int o = (triangulated.Length * 2) - 1 - i;

			tris [i] = triangulated [i];
			tris [o] = points.Length + triangulated [i];
		}

		// BRING THE TWO FACE TOGETHER WITH SIDENESS
		for (int i = 0; i < points.Length - 1; i++) {
			int v0 = i;
			int v1 = i + 1;
			int v2 = points.Length + i;
			int v3 = points.Length + i + 1;

			int start = (triangulated.Length * 2) + (i * 6);

			tris [start + 0] = v0; tris [start + 1] = v1; tris [start + 2] = v2;
			tris [start + 3] = v2; tris [start + 4] = v1; tris [start + 5] = v3;
		}

		// JOIN THE END WITH THE START FOR MAXIMUM SIDENESS

		int ve0 = points.Length - 1;
		int ve1 = 0;
		int ve2 = points.Length * 2 - 1;
		int ve3 = points.Length;

		int starte = tris.Length - 6;

		tris [starte + 0] = ve0; tris [starte + 1] = ve1; tris [starte + 2] = ve2;
		tris [starte + 3] = ve2; tris [starte + 4] = ve1; tris [starte + 5] = ve3;

		// OK WE ARE DONE NOW

		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.uv = uvs;
		mesh.RecalculateBounds ();
		return mesh;
	}
}