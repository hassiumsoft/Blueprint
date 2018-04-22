using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuPanel : BPPanel {
    public GameObject Pole;
    public GameObject Kabe;
    public Slider width;
    public Slider height;
    public Text WValue;
    public Text HValue;
    //public Button ExitButton;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        //TODO float→Stringをやるメソッドをかませる
        WValue.text = width.value;
        //TODO float→Stringをやるメソッドをかませる
        HValue.text = height.value; 
	}
    public void Poletate()
    {
        //int 
        //int b = 0;
        //for(b = 1; a == b ;) { 
        GameObject newpole = Object.Instantiate(Pole) as GameObject;
        
   // }

    }
    public void ExitButton()
    {
        base.show(false);
    }
    

}
