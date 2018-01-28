using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;



public class textDisplayer : MonoBehaviour {

	public bool show3dText = false;	//Made true for testing, turn this to false here 
	// Use this for initialization
	public bool showUtteranceTrigger ;	//This value set for testing - in actual code don't initialize here
	public Watson watsonScript;
	public string speechToShow ;
	public string numHesitates ;	//This is a sample number here . Delete this value and do intialization in start 
	TextMesh t;	//Will hold speech user gave


	void Start () {
		watsonScript = GameObject.Find ("Cube").GetComponent<Watson> ();

		t = (TextMesh)gameObject.GetComponent (typeof(TextMesh));
	}
	
	// Update is called once per frame
	void Update () 
	{
		showUtteranceTrigger = watsonScript.showUtterance;
		if (watsonScript.userFinalSpeech != null) {
			speechToShow = watsonScript.userFinalSpeech.ToString ();
		}numHesitates = watsonScript.numTimesHesitate.ToString ();
		/*if (true)                       //show3dText != showUtteranceTrigger) 
		{
			show3dText = showUtteranceTrigger;

			if (true) {
				
				//t.text = speechToShow;
			
			} else 
			{
				
				//t.text = speechToShow;
				//t.text += "\nNot recording";
			}
		}*/
	}





}
