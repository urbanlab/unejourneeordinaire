using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyWiFi.Core;
using EasyWiFi.ClientControls;

public class CStringToServer : MonoBehaviour
{

	public StringDataClientController _strDataClientToSend; 

	private static CStringToServer s_oInstance = null;

	public static CStringToServer Get()
	{
		return s_oInstance;
	}

	// Use this for initialization
	void Start () 
	{
		s_oInstance = this;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void SendStringToServer( string a_strStringToServer )
	{
		_strDataClientToSend.setValue ( a_strStringToServer );
	}

}
