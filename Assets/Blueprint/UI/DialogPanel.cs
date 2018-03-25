using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;

public class DialogPanel : BPPanel {
	public Sprite informationpng;
	public Sprite errorpng;
	public Sprite stoppedpng;
    public Text Message;
    public Image IconImage;
    public string how = "noerror";
    public string aboutoferror;
    public string txtfile;
	
	// Update is called once per frame
	void Update () {}

    public void information(string errorabout)
    {
        IconImage.sprite = informationpng;
        Message.text = errorabout;
        how = "info";
        base.show(true);
        aboutoferror = errorabout;
       // Pause(5);
      //  base.show(false);
    }

    public void error(string errorabout)
    {
        IconImage.sprite = errorpng;
        Message.text = errorabout;
        how = "error";
        base.show(true);
        aboutoferror = errorabout;
      //  Pause(5);
       // base.show(false);
    }

    public void stopped(string whystopped)
    {
        IconImage.sprite = stoppedpng;
        Message.text = whystopped;
        how = "stopped";
        base.show(true);
        aboutoferror = whystopped;
      //  Pause(5);
       // base.show(false);


    }

    public void Close()
    {

        /*
        if (how == "error")
        {
            //writelog(aboutoferror, "error", txtfile);
            base.show(false);
        }else
        if(how == "stopped")
        {
            //writelog(aboutoferror, "stopped", txtfile);
            base.show(false);
        }else
        if (how == "info")
        {
            base.show(false);

        }*/
       // Pause(5);
        base.show(false);
    }
    IEnumerator Pause(int sec)
    {
        yield return new WaitForSeconds(sec);
        yield break;
    }

    public void writelog(string errorabout, string howdoes, string textfile)
    {
        string now = System.DateTime.Now.ToString();
        StreamWriter writer = new StreamWriter(textfile, true);
        writer.WriteLine("time-stamp:" + now + "Blueprint has" + howdoes + "\nabout:" + errorabout + "----------[EOL]----------\n");
        writer.Flush();
        writer.Close();


    }

}


