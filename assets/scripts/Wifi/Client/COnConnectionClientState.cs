using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COnConnectionClientState : MonoBehaviour 
{



	public void OnClientConnected( )
	{
		Debug.Log("Client is connected");
	}

	public void OnClientDisconnected( )
	{
		Debug.Log("Client is disconnected");
	}

}
