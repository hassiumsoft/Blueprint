using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;


public class DialogPanel : BPPanel {
    Sprite informationpng = Resources.Load<Sprite>("Textures/dialog/information");
    Sprite errorpng = Resources.Load<Sprite>("Textures/dialog/error");
    Sprite stoppedpng = Resources.Load<Sprite>("Textures/dialog/stopped");
    public Text Message;
    public Image IconImage;
    public string how = "noerror";
    public string aboutoferror;
	
	// Update is called once per frame
	void Update () {}

    public void information(string errorabout)
    {
        IconImage.sprite = informationpng;
        Message.text = errorabout;
        how = "info";
        base.show(true);
        aboutoferror = errorabout;
    }

    public void error(string errorabout)
    {
        IconImage.sprite = errorpng;
        Message.text = errorabout;
        how = "error";
        base.show(true);
        aboutoferror = errorabout;
    }

    public void stopped(string whystopped)
    {
        IconImage.sprite = stoppedpng;
        Message.text = whystopped;
        how = "stopped";
        base.show(true);
        aboutoferror = whystopped;

    }

    public void CloseButton()
    {
        if (how == "info")
        {
            if(how == "error")
            {
                writelog(aboutoferror, "error");
                base.show(false);
            }
            if(how == "stopped")
            {
                writelog(aboutoferror, "stopped");
                base.show(false);
            }
            base.show(false);
        }
    }

    public void writelog(string errorabout, string howdoes)
    {
        string now = System.DateTime.Now.ToString();
        StreamWriter writer = new StreamWriter("errorlogs/errorlog");
        writer.WriteLine("time-stamp:" + now + "Blueprint has" + howdoes + "\nabout:" + errorabout + "----------[EOL]----------\n");
        writer.Flush();
        writer.Close();


    }

}


