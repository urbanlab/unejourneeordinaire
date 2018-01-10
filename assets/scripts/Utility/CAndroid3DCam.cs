using UnityEngine;
using System.Collections;

public class CAndroid3DCam : MonoBehaviour {

	#if !UNITY_EDITOR
	GameObject _goCamera = null;
	GameObject _goTrackingSpace = null;
	#endif
	Camera _camRight = null;
	Camera _camLeft = null;

	
	// Use this for initialization
	void Start () {
		#if !UNITY_EDITOR
		_goCamera = GameObject.Find("Camera").gameObject;
		_goTrackingSpace = GameObject.Find("Camera/TrackingSpace").gameObject;
		#endif

		_camRight = GameObject.Find ("Camera/TrackingSpace/Right").GetComponent<Camera>();
		_camLeft = GameObject.Find ("Camera/TrackingSpace/Left").GetComponent<Camera>();
		#if !UNITY_EDITOR
		Input.gyro.enabled = true;
		
		_goCamera.transform.Rotate (90, 0, -50);
		#endif
		
	}
	
	void OnMouseDown()
	{
		if(_camRight.gameObject.activeSelf)
		{
			_camRight.gameObject.SetActive(false);
			_camLeft.rect = new Rect(0f, 0f, 1f, 1f);
		}
		else
		{
			_camRight.gameObject.SetActive(true);
			_camLeft.rect = new Rect(0f, 0f, 0.5f, 1f);
		}
	}

#if !UNITY_EDITOR
	Vector3 delta = Vector3.zero;
#endif
	// Update is called once per frame
	void Update () 
	{
	
	#if !UNITY_EDITOR
		foreach(Touch touch in Input.touches)
		{
			delta.x += touch.deltaPosition.y;
			delta.y += touch.deltaPosition.x;
		}

		// Invert the z and w of the gyro attitude
		Quaternion vect = new Quaternion(Input.gyro.attitude.x, Input.gyro.attitude.y, -Input.gyro.attitude.z, -Input.gyro.attitude.w);

		_goTrackingSpace.transform.localRotation = vect;
		_goTrackingSpace.transform.Rotate(-delta.x, 0, 0);
		_goTrackingSpace.transform.Rotate (0, delta.y, 0);
		#endif
	}
}
