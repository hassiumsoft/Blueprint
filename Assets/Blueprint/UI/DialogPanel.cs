using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogPanel : MonoBehaviour {
    Sprite informationpng = Resources.Load<Sprite>("Textures/dialog/information");
    Sprite errorpng = Resources.Load<Sprite>("Textures/dialog/error");
    Sprite stopped = Resources.Load<Sprite>("Textures/dialog/stopped");
    //TODO IconImageをIconとして読み込み
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public static void information()
    {

    }
}
