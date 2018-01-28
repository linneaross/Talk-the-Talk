using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using SimpleJSON;

public class setimentAnalyse : MonoBehaviour {

	private IEnumerator coroutine;
	// Use this for initialization
	private string documents = "This is a positive statement !\n";
	public bool showUtteranceTrigger ;	//This value set for testing - in actual code don't initialize here
	public Watson watsonScript;
	public string speechToShow ;
	TextMesh t;
	//UnityWe
	void Start ()
	{
		Debug.Log ("Start called");
			watsonScript = GameObject.Find ("Cube").GetComponent<Watson> ();

			t = (TextMesh)gameObject.GetComponent (typeof(TextMesh));

	}

	IEnumerator getRequest(string uri)
	{
		Debug.Log ("Get Request enetered");
		UnityWebRequest uwr = UnityWebRequest.Get (uri);
		yield return uwr.SendWebRequest ();
		if (uwr.isNetworkError) {
			Debug.Log ("Error when sending \n");
		} else 
		{
			Debug.Log ("Received" + uwr.downloadHandler.text);
			//JSONObject json = new JSONObject(uwr.downloadHandler.text);

			//json.GetField ("");
			//t.text += uwr.downloadHandler.text;
		}

	}


	// Update is called once per frame
	void Update () {
		showUtteranceTrigger = watsonScript.showUtterance;
		if (showUtteranceTrigger)
		{
			//StartCoroutine(getRequest("https://language.googleapis.com/$discovery/rest?version=v1beta2/documents:analyzeSentiment "));

		}

	}
}
