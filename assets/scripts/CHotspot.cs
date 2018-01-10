using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHotspot : MonoBehaviour 
{
	private MAppManager _appManager;

	[SerializeField]
	private AudioSource _audioSource;



	void Start () 
	{
		_appManager = GameObject.Find( "_app_manager" ).GetComponent< MAppManager >( );	
	}
	

	void Update () 
	{
		
	}

}
