/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using System.Text;

public class speechInput : MonoBehaviour {

	[SerializeField]
	private string[] listKeywords = {"start"} ;

	private KeywordRecognizer wordRecognizer; 

	//Text Area which shows the recognized strings
	public Text DictationDisplay;

	private DictationRecognizer dictationRecognizer;

	//store the text currently spoken
	private StringBuilder textSoFar;


	//keeping deviceName as emptystring means use default microsophone
	private static string deviceName = string.Empty;
	private int samplingRate;
	private const int messageLength = 10;

	//have a boolean to reset the UI once we're done recording
	private bool hasRecordingStarted;


	void Awake()
	{
		dictationRecognizer = new DictationRecognizer ();
		//As the user pauses-typically when some sentence ends, it is displayed on the screen
		dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
		//When dictation stops - whether from stop() being called or from some seeor, the method is called
		dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
		//When an error occurs, this is fired
		dictationRecognizer.DictationError += DictationRecognizer_DictationError;
		//Get the max frequency of the default microphone, use int unused to ignore the min frequency
		//sampling rate will hold the maxFreq
		int unusedMinFreq;
		Microphone.GetDeviceCaps (deviceName, out unusedMinFreq, out samplingRate);
	
		//Save text captured so far
		textSoFar = new StringBuilder();

		//use this boolean so as to reset UI when recording is finished
		hasRecordingStarted = false;


	}

	void DictationRecognizer_DictationError (string error, int hresult)
	{
		//Called when some error occurs
		//string 'error' is reason for error
		//the hresult is the int representation of the error

		DictationDisplay.text = error + "\nHRESULT: " + hresult;



	}



	void DictationRecognizer_DictationHypothesis (string text)
	{
		DictationDisplay.text = textSoFar.ToString () + " " + text + "...";

	}

	void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
	{
		//if timeout occurs - user was silent for too long
		//Default timeout-with dictation is 20secs
		//Default timeout with initial silence is 5 secs
		if (cause == DictationCompletionCause.TimeoutExceeded)
		{
			Microphone.End (deviceName);
			DictationDisplay.text = "Your Dictation timed out";
			//Send message calls method called 'ResetAfterTimeout'
			SendMessage ("ResetAfterTimeout");
		
		}
	
	}

	// Use this for initialization
	void Start () 
	{
		wordRecognizer = new KeywordRecognizer (listKeywords);
		wordRecognizer.OnPhraseRecognized += WordRecognizer_OnPhraseRecognized;
		wordRecognizer.Start ();
	}

	void WordRecognizer_OnPhraseRecognized (PhraseRecognizedEventArgs args)
	{
		StartRecording ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (hasRecordingStarted && !Microphone.IsRecording (deviceName) && dictationRecognizer.Status == SpeechSystemStatus.Running) 
		{
			//Now try to clean UI - so we'll reset the flag
			hasRecordingStarted = false;
			StopRecording ();
			//SendMessage ("RecordStop");	//Record Stop method is called
		}

	}

	//Turns on dictation recognizer and recording begins from default microphone
	//Method returns the audio clip recorded
	public AudioClip StartRecording()
	{
		//Shutdown the PhraseRcognitionSystem. This controls the keywordRecognizers
		PhraseRecognitionSystem.Shutdown();

		//Start dictationRecognizer
		dictationRecognizer.Start();

		DictationDisplay.text = "Dictation is starting. It may take time to display the text the first time. But continue speaking.";
		hasRecordingStarted = true;
		//message length is 10 secs. So record from the microphone for 10 secs
		return Microphone.Start (deviceName, false, messageLength, samplingRate);
	}

	//Ending the recording session
	public void StopRecording()
	{
		if (dictationRecognizer.Status == SpeechSystemStatus.Running) 
		{
			dictationRecognizer.Stop ();	//Stop the recording
		
		}
		Microphone.End (deviceName);	//Stop recording
	}

	/*private IEnumerator RestartSpeechSystem(KeywordManager keywordToStart)
	{
		while (dictationRecognizer != null && dictationRecognizer.Status == SpeechSystemStatus.Running) {
			yield return null;
		}
		keywordToStart.StartKeywordRecognizer ();
	}
* //////////// - THIS was end of original content
}
*/