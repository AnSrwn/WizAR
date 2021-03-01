using UnityEngine;

public class SpeechRecognizerPlugin_Editor : SpeechRecognizerPlugin
{
    private bool isContinuous = false;
    private int silenceLength = 500;

    public SpeechRecognizerPlugin_Editor(string gameObjectName) : base(gameObjectName) { }

    public override void SetContinuousListening(bool isContinuous)
    {
        this.isContinuous = isContinuous;
    }

    public override void SetSilenceLength(int silenceLength)
    {
        this.silenceLength = silenceLength;
    }

    public override void StartListening()
    {
        SpeechRecognizer speechRecognizer = GameObject.FindObjectOfType<SpeechRecognizer>();
        if(this.isContinuous)
            speechRecognizer.OnResult("left left right right forward attack");
        else
            speechRecognizer.OnResult("start listening test~start listening test~start listening test");
    }

    protected override void SetUp()
    {
        Debug.LogWarning("<b>WARNING</b>: You are running this plugin on Editor mode. Real recognition only works running on mobile device.");
    }
}