using UnityEngine;

public class Test903 : MonoBehaviour {

	public Material mat;

	MeshFilter f;
	TreeType type = TreeType.Shirakashi;
	float age = 0;

	void Start () {
		f = gameObject.AddComponent<MeshFilter> ();
		gameObject.AddComponent<MeshRenderer> ().material = mat;
	}

	void Update () {
		age = Time.time;
		if (age < TreeInfo.getMaxHeight (type) / TreeInfo.getGrowSpeed (type)) {
			f.sharedMesh = BPMesh.generateTree (new TreeInfo (type, age));
			f.sharedMesh.RecalculateBounds ();
			f.sharedMesh.RecalculateNormals ();
		}
	}

	void OnGUI () {
		GUI.color = Color.black;
		GUI.Label (new Rect (4, 4, 120, 24), Mathf.FloorToInt (age * 365.25f) + "日目");
	}
}
