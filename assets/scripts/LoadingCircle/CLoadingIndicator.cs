using UnityEngine;
using System.Collections;

public class CLoadingIndicator : MonoBehaviour 
{
	private Transform _tRotatingBody;
	private bool _bIsEnabled = false;

	public bool IsEnabled( )
	{
		return _bIsEnabled;
	}

	public void Toggle( bool a_bValue )
	{
		_bIsEnabled = a_bValue;
		GetComponent<Renderer> ().enabled = a_bValue;
		_tRotatingBody.gameObject.GetComponent<MeshRenderer> ().enabled = a_bValue;
	}

	void Start( )
	{
		_tRotatingBody = transform.Find ("loading_indicator");
		Toggle (false);
	}

	void Update () 
	{
		if( _bIsEnabled )
		{
			_tRotatingBody.Rotate (Vector3.up, 0.5F);
		}
	}
}
