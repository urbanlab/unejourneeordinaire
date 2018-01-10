using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTimer
{
	private float _fStartTime;

	void Start () 
	{
		_fStartTime = Time.time;
	}

	public float GetElapsedTime( )
	{
		return (Time.time - _fStartTime);
	}

	public void ReStart( )
	{
		_fStartTime = Time.time;
	} 
}
