using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
		meshfilter = GetComponent<MeshFilter> ();

		Mesh mesh = BPMesh.getBPFractalTerrain (4, 0.5f);

		string path = "test.bin";

		IFormatter formatter = new BinaryFormatter ();
		Stream stream = new FileStream (path, FileMode.Create, FileAccess.Write, FileShare.None);
		formatter.Serialize (stream, new SerializableMesh (mesh));
		stream.Close ();

		stream = new FileStream (path, FileMode.Open, FileAccess.Read, FileShare.Read);
		mesh = ((SerializableMesh)formatter.Deserialize (stream)).toMesh ();
		stream.Close ();

		meshfilter.sharedMesh = mesh;

		GetComponent<MeshCollider> ().sharedMesh = meshfilter.sharedMesh;
	}
}
