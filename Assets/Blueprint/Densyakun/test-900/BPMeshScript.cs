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

		//BPMesh mdmesh = new BPMesh(meshfilter.sharedMesh);
		//mdmesh.a();
		//meshfilter.sharedMesh = mdmesh.toMesh();

		meshfilter.sharedMesh = BPMesh.Subdivide_Half (meshfilter.sharedMesh);

		Destroy(GetComponent<BoxCollider>());
		gameObject.AddComponent<BoxCollider>();
	}
}
