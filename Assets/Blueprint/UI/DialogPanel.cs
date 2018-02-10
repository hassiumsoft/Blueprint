using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;


public class DialogPanel : BPPanel {
    Sprite informationpng = Resources.Load<Sprite>("Textures/dialog/information");
    Sprite errorpng = Resources.Load<Sprite>("Textures/dialog/error");
    Sprite stoppedpng = Resources.Load<Sprite>("Textures/dialog/stopped");
    public string errorlog = ""
    public Text Message;
    public Image IconImage;
    public string how = "noerror";
	
	// Update is called once per frame
	void Update () {}

    public void information(string errorabout)
    {
        IconImage.sprite = informationpng;
        Message.text = errorabout;
        how = "info";
        base.show(true);
    }

    public void error(string errorabout)
    {
        IconImage.sprite = errorpng;
        Message.text = errorabout;
        how = "error";
        base.show(true);
    }

    public void stopped(string whystopped)
    {
        IconImage.sprite = stoppedpng;
        Message.text = whystopped;
        how = "stopped";
        base.show(true);
        
    }

    public void CloseButton()
    {
        base.show(false);
        if (how == "info")
        {
            if(how == "error")
            {
                StreamWriter writer = new StreamWriter("")
            }
        }
    }
    
}


