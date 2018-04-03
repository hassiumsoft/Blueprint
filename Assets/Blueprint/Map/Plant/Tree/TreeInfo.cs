using UnityEngine;

public class TreeInfo {
	//樹木の情報を管理する

	//TODO 個々の枝等を管理し、成長させるメソッドを追加

	public const float MIN_RADIUS = 0.005f;
	public const float MIN_HEIGHT = 0.01f;

	public TreeType type;
	public float age;

	public TreeInfo (TreeType type, float age) {
		this.type = type;
		this.age = age;
	}

	//双子葉類
	public bool isMagnoliopsida () {
		/*switch (type) {
		case TreeType.Cryptomeria:
		case TreeType.Shirakashi:
			return true;
		default:
			return false;
		}*/
		return true;
	}

	//単子葉類
	public bool isLiliopsida () {
		return !isMagnoliopsida ();
	}

	public float getRadius () {
		return Mathf.Lerp (MIN_RADIUS, getMaxRadius (type), getHeight () / getMaxHeight (type));
	}

	public float getHeight () {
		return Mathf.Max (MIN_HEIGHT, Mathf.Min (age * getGrowSpeed (type), getMaxHeight (type)));
	}

	public float getBranchDownHeight () {
		switch (type) {
		case TreeType.Quercus_myrsinifolia:
			return Mathf.Min (age * getGrowSpeed (type) / 4, getMaxHeight (type) / 4);
		default:
			return 0f;
		}
	}

	public static float getMaxRadius (TreeType type) {
		switch (type) {
		case TreeType.Quercus_myrsinifolia:
			return 0.6f; //TODO 仮
		default:
			return 0f;
		}
	}

	public static float getMaxHeight (TreeType type) {
		switch (type) {
		case TreeType.Quercus_myrsinifolia:
			return 15f;//TODO 20
		default:
			return 0f;
		}
	}

	public static float getGrowSpeed (TreeType type) {
		switch (type) {
		case TreeType.Quercus_myrsinifolia:
			return 0.5f;
		default:
			return 0f;
		}
	}

	public class Branch {

		float branchDown;

		public Branch (float branchDownHeight) {
			this.branchDown = branchDown;
		}
	}
}
