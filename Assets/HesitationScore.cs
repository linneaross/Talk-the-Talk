using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HesitationScore : MonoBehaviour {


	// Use this for initialization
	//This value set for testing - in actual code don't initialize here
	public Watson watsonScript;

	public string numHesitates ;	//This is a sample number here . Delete this value and do intialization in start 
	TextMesh t;	//Will hold speech user gave

	void Start () {
		watsonScript = GameObject.Find ("Cube").GetComponent<Watson> ();

		t = (TextMesh)gameObject.GetComponent (typeof(TextMesh));
	}

	// Update is called once per frame
	void Update () 
	{
		
		numHesitates = watsonScript.numTimesHesitate.ToString ();
		t.text = numHesitates;
	}
}
