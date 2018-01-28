using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.DataTypes;
using System;
using System.Text;

using System.Collections.Generic;
using System.IO;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Watson2 : MonoBehaviour
{
	private int m_RecordingRoutine = 0;


	private string m_MicrophoneID = null;
	private AudioClip m_Recording = null;
	private int m_RecordingBufferSize = 2;
	private int m_RecordingHZ = 22050;
	public bool showUtterance = false;
	public StringBuilder userFinalSpeech;

	private SpeechToText m_SpeechToText;
	public GUIStyle myGuiStyle;
	private int numWords = 0;
	public int numTimesHesitate = 0;
	private int minFreq = 0, maxFreq = 0;

	//GameObject.Find ("Cube").GetComponent<Watson> ();

	TextMesh temp;

	void Start()
	{

		StartCoroutine (RunCoroutine ());


	}

	private IEnumerator RunCoroutine()
	{

		yield return new WaitForSecondsRealtime(2);
		LogSystem.InstallDefaultReactors();
		Credentials credentials = new Credentials ("051cdc53-daff-41d3-9258-14562c19e6bc", "Gd5NzFy7KCqW", "https://stream.watsonplatform.net/speech-to-text/api"); 
		m_SpeechToText = new SpeechToText (credentials);
		Log.Debug("ExampleStreaming", "Start();");
		myGuiStyle.normal.background = new Texture2D (2, 2, TextureFormat.ARGB32, false);
		myGuiStyle.alignment = TextAnchor.UpperCenter;
		myGuiStyle.normal.textColor = Color.white;
		myGuiStyle.wordWrap = true;
		myGuiStyle.padding = new RectOffset(5,5,5,5);
		userFinalSpeech = new StringBuilder ();
		temp =  (TextMesh) (GameObject.Find("UserText").GetComponent (typeof(TextMesh)));
		//temp.text += "\nNow will call watson service";
		Active = true;
		Runnable.Run (RunCoroutine ());
		StartRecording();
		//temp.text += "Attempt made";
		yield return null;

	}
	public bool Active
	{
		get { return m_SpeechToText.IsListening; }
		set {
			if ( value && !m_SpeechToText.IsListening )
			{
				//temp.text += " Setting up configs for speechToText in Active ";
				m_SpeechToText.DetectSilence = true;
				m_SpeechToText.EnableWordConfidence = false;
				m_SpeechToText.EnableTimestamps = false;
				m_SpeechToText.SilenceThreshold = 0.03f;
				m_SpeechToText.MaxAlternatives = 1;
				//m_SpeechToText.EnableInterimResults = true;

				//m_SpeechToText.EnableContinousRecognition = true;
				m_SpeechToText.EnableInterimResults = true;
				m_SpeechToText.OnError = OnError;
				m_SpeechToText.StartListening( OnRecognize );
			}
			else if ( !value && m_SpeechToText.IsListening )
			{
				//temp.text += " Stop Listening called now ";
				m_SpeechToText.StopListening();
			}
		}
	}

	private void StartRecording()
	{
		//temp.text += " Called start recording ";
		if (m_RecordingRoutine == 0)
		{
			//temp.text += " Recording Routine len is 0 - calling recording handler ";
			UnityObjectUtil.StartDestroyQueue();
			m_RecordingRoutine = Runnable.Run(RecordingHandler());
		}
		showUtterance = true;
		//temp.text += "outside if";
		Debug.Log ("Set showUtterance to false in start recording");
	}

	private void StopRecording()
	{
		showUtterance = true;
		//temp.text += " Called Stop recording";
		Debug.Log ("Set showUtterance to true in stop recording");
		if (m_RecordingRoutine != 0)
		{
			Microphone.End(m_MicrophoneID);
			Runnable.Stop(m_RecordingRoutine);
			m_RecordingRoutine = 0;
		}
	}

	private void OnError( string error )
	{
		Active = false;
		//temp.text += " Some error happened - on Error method";
		Log.Debug("ExampleStreaming", "Error! {0}", error);
	}

	private IEnumerator RecordingHandler()
	{

		//temp.text += " Recording handler called ";


		Microphone.GetDeviceCaps(null, out minFreq , out maxFreq);

		//According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...
		if(minFreq == 0 && maxFreq == 0)
		{
			//...meaning 44100 Hz can be used as the recording sampling rate
			maxFreq = 44100;
		}
		m_Recording = Microphone.Start(null, true, m_RecordingBufferSize, maxFreq);

		//yield return null;      // let m_RecordingRoutine get set..

		if (m_Recording == null)
		{
			//temp.text += " m_Recording is null ";
			StopRecording();
			yield break;
		}else 
		{
			//temp.text += " mike is not null";
		}

		bool bFirstBlock = true;
		int midPoint = m_Recording.samples / 2;
		float[] samples = null;

		while (m_RecordingRoutine != 0 && m_Recording != null)
		{
			int writePos = Microphone.GetPosition(m_MicrophoneID);
			//temp.text += " writePos is " + writePos.ToString (); 
			if (writePos > m_Recording.samples || !Microphone.IsRecording (m_MicrophoneID)) {
				Log.Error ("MicrophoneWidget", "Microphone disconnected.");
				//temp.text += " Problem with the mike ";
				StopRecording ();
				yield break;
			} else 
			{
				//temp.text += " Mike is connected  ";
			}

			if ((bFirstBlock && writePos >= midPoint)
				|| (!bFirstBlock && writePos < midPoint))
			{
				// front block is recorded, make a RecordClip and pass it onto our callback.
				samples = new float[midPoint];
				m_Recording.GetData(samples, bFirstBlock ? 0 : midPoint);
				//temp.text += " Passing to callback ";
				AudioData record = new AudioData();
				record.MaxLevel = Mathf.Max(samples);
				record.Clip = AudioClip.Create("Recording", midPoint, m_Recording.channels, m_RecordingHZ, false);
				record.Clip.SetData(samples, 0);

				m_SpeechToText.OnListen(record);

				bFirstBlock = !bFirstBlock;
			}
			else
			{
				// calculate the number of samples remaining until we ready for a block of audio, 
				// and wait that amount of time it will take to record.
				int remaining = bFirstBlock ? (midPoint - writePos) : (m_Recording.samples - writePos);
				float timeRemaining = (float)remaining / (float)m_RecordingHZ;
				//temp.text += " Waiting for audio sample to finish ";
				yield return new WaitForSeconds(timeRemaining);
			}

		}

		yield break;
	}

	private void OnRecognize(SpeechRecognitionEvent result)
	{
		//temp.text += "\n OnRecognize called ";
		if (result == null) {
			//temp.text += " Result is null ";
		} else if (result.results.Length <= 0)
		{	//temp.text += " Result len is <= 0 ";
		}
		if (result != null && result.results.Length > 0)
		{
			//temp.text += " Called on recognize " ;
			foreach (var res in result.results)
			{
				Debug.Log (res);
				//temp.text += " There are results of recording";
				foreach (var alt in res.alternatives)
				{
					string text = alt.transcript;
					Log.Debug("ExampleStreaming", string.Format("{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));
					if(text.Contains ("stop recording"))
					{
						StopRecording ();
					}
					if (text.Contains ("%HESITATION")) 
					{
						numTimesHesitate++;
					}
					if (res.final) {
						//showUtterance = true;
						userFinalSpeech.Append (text);
						temp.text += " \n "+text + "\n ";

					} else 
					{
						//showUtterance = false;
					}
				}
			}
		}
	}


	void OnGUI()
	{
		if(true)
		{
			GUI.contentColor = Color.white;
			GUI.backgroundColor = Color.black;
			GUI.color = Color.white;
			GUI.Label (new Rect (0, 0, 100,100), numWords + "", myGuiStyle);
			//GUI.Label (new Rect (0, 0, Screen.width, Screen.height), userFinalSpeech.ToString(), myGuiStyle);
		}
	}
}