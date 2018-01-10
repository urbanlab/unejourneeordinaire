using UnityEngine;
using System.Collections;

public class CRotator : MonoBehaviour 
{
	[SerializeField] private Vector3 _v3RotationSpeed;

	private Transform _transform;

	///-----------------------------------------------------------------------------------
	/// 
	///-----------------------------------------------------------------------------------
	void Start ()
	{
		_transform = transform;
	}

	///-----------------------------------------------------------------------------------
	/// 
	///-----------------------------------------------------------------------------------
	void Update ()
	{
		if (_transform != null) 
		{			
			_transform.Rotate (_v3RotationSpeed * Time.deltaTime);
		}
	}
}
