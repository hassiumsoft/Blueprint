using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class TreeObject : PlantObject {
	public const string KEY_TYPE = "TYPE";
	public const string KEY_MESH = "MESH";

	//TODO
	//幹
	//枝
	//葉
	//実

	//葉緑体
	//水分
	//デンプン
	//光合成

	//肥料分

	//TODO 二酸化炭素の検出

	public bool generated { get; private set; }
	public TreeType type;
	public Mesh mesh;

	public TreeObject (Map map, Vector3 pos, Quaternion rot) : base (map, pos, rot) {
	}

	protected TreeObject (SerializationInfo info, StreamingContext context) : base (info, context) {
		type = (TreeType)info.GetValue (KEY_TYPE, typeof(TreeType));
		SerializableMesh sMesh = (SerializableMesh)info.GetValue (KEY_MESH, typeof(SerializableMesh));
		if (sMesh != null) {
			mesh = sMesh.toMesh ();
		}
	}

	public override void GetObjectData (SerializationInfo info, StreamingContext context) {
		base.GetObjectData (info, context);
		info.AddValue (KEY_TYPE, type);
		info.AddValue (KEY_MESH, mesh == null ? null : new SerializableMesh (mesh));
	}

	public override void generate () {
		if (!generated) {
			type = TreeType.Shirakashi;
			mesh = BPMesh.generateTree (new TreeInfo (type, age));
			generated = true;
		}

		if (entity == null)
			(entity = new GameObject ("tree-" + getChunkX () + "," + getChunkZ ()).AddComponent<MapEntity> ()).init (this);
		else
			reloadEntity ();
	}

	public override void reloadEntity () {
		if (entity == null)
			return;

		MeshFilter meshfilter = entity.GetComponent<MeshFilter> ();
		MeshRenderer meshrenderer = entity.GetComponent<MeshRenderer> ();
		//MeshCollider meshcollider = entity.GetComponent<MeshCollider> ();
		if (meshfilter == null)
			meshfilter = entity.gameObject.AddComponent<MeshFilter> ();
		if (meshrenderer == null)
			meshrenderer = entity.gameObject.AddComponent<MeshRenderer> ();
		//if (meshcollider == null)
		//	meshcollider = entity.gameObject.AddComponent<MeshCollider> ();

		//meshcollider.convex = true;

		/*meshcollider.sharedMesh = */meshfilter.sharedMesh = mesh;
		meshfilter.sharedMesh.RecalculateBounds ();
		meshfilter.sharedMesh.RecalculateNormals ();

		base.reloadEntity ();
	}
}
