using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//独自のスプラッシュ画面(現在開発中)
//スクリプトの動作の検証も行う。
//従来のスプラッシュ画面ではできない機能を開発する。
public class SplashManager : MonoBehaviour {
	public Camera c;

	//OnEnableよりも前、スクリプトの付属するオブジェクトが読み込まれたときに実行される。
	//スクリプトが更新された場合は実行されない。
	void Awake() {
		print ("Awake");
		if (c != null) {
			c.backgroundColor = Color.red + Color.green;
		}
	}

	//スクリプトが更新された場合に最初に実行される。
	void OnEnable() {
		print ("OnEnable");
		if (c != null) {
			c.backgroundColor = Color.green;
		}
	}

	//最初のUpdateが実行される前に実行される。
	//一般的に使われるメソッドだが、スクリプトが更新された場合には実行されないため、
	//開発にはStartよりもOnEnableを使用したほうが良い。
	void Start () {
		print ("Start");
		if (c != null) {
			c.backgroundColor = Color.green + Color.blue;
		}
	}

	/*void Update () {
		print ("Update");
		if (c != null) {
			c.backgroundColor = new Color (Random.Range (0f, 255f), Random.Range (0f, 255f), Random.Range (0f, 255f));
		}
	}*/
}
