using UnityEngine;

public class BPMeshScript : MonoBehaviour
{
	MeshFilter meshfilter;

    void Start()
    {
        
    }

    void Update()
    {

    }

	public void a () {
		meshfilter = GetComponent<MeshFilter>();

		meshfilter.sharedMesh = BPMesh.getBPFractalTerrain (4, 0.5f);

		GetComponent<MeshCollider> ().sharedMesh = meshfilter.sharedMesh;

		//Destroy (GetComponent<BoxCollider> ());
		//gameObject.AddComponent<BoxCollider> ();
	}
}
