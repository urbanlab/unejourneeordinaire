using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStringFromClient : MonoBehaviour 
{

	private string _strString;
	// Use this for initialization
	void Start () 
	{
		_strString = "NoDataReceived";
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (_strString != "NoDataReceived") 
		{
			CMainManager.Get ().ReceiveDataFromClient (_strString );
		}
	}


	public void OnStringChanged(string a_strString)
	{
		Debug.Log ("On string changed " + a_strString);
		_strString = a_strString;
	}
}
