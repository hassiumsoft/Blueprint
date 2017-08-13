using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test903 : MonoBehaviour {
	
	void Start () {
		MeshFilter f = gameObject.AddComponent<MeshFilter> ();
		f.sharedMesh = BPMesh.cylinder (0.5f, 1f, 6);
		f.sharedMesh.RecalculateBounds ();
		f.sharedMesh.RecalculateNormals ();

		gameObject.AddComponent<MeshRenderer> ();
	}

	void Update () {
		
	}
}
