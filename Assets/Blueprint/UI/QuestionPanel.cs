using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class QuestionPanel : BPPanel {
    public Text Question;

    public void question(string question, string whatdo)
    {
        Question.text = question;
        base.show(true);
    }

    public void cancel()
    {
        base.show(false);
    }
}
