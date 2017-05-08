using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour {
	public Text text;
	public int a = 0;

	void Start () {
		
	}

	void Update () {
		
	}

	public void Test(int n) {
		a += n;
		text.text = "" + a;
	}
}
