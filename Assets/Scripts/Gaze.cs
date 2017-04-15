using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gaze : MonoBehaviour {

	public LoadingCircle _loadingCircle;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Ray oRay;
		RaycastHit oHit;
		oRay = new Ray(transform.position, transform.forward);
		if (Physics.Raycast (oRay.origin, oRay.direction, out oHit, Mathf.Infinity)) {
			if (oHit.collider.gameObject.GetComponent< Choice > ()) {

				Choice choice = oHit.collider.gameObject.GetComponent <Choice> ();
				// make glow the interface.

				_loadingCircle.IncreaseTimer (); 

				if (_loadingCircle.IsSelected ()) {
					// select choice

					_loadingCircle.ResetCursor ();
				}
			}
		} else {
			_loadingCircle.ResetCursor ();
		}
	}
}
