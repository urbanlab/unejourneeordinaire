using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CRaycastHelper : MonoBehaviour{
	
	public static float _doubleTapDelay = 0.2f;
	public static float _stayDelay = 0.2f;
	public static float _takeAccountDelay = 0.2f;
		
	private static float tps = -1f;
	private static float tps2 = -1f;
	private static float tps3 = -1f;
	
	private static List<GameObject> _raycasted = null;
	private static GameObject _firstRaycasted = null;
	private static GameObject _firstMouseRaycasted = null;
	private static bool _bSingleClicked = false;
	private static bool _bDoubleClicked = true;
	
	private static void CheckMouse()
	{
		if(Input.GetMouseButtonDown(0))
		{
			if(tps == -1)
			{
				tps = Time.time;
			}
			else
			{
				if(tps2 == -1)
				{
					tps2 = Time.time;
				}
				else
				{
					tps = tps2;
					tps2 = Time.time;
				}
			}
		}
		else if(Input.GetMouseButtonUp(0))
		{
			tps2 = Time.time;
		}
	}
	
	private static bool isLastTapsDoubleTap()
	{
		return tps2 != -1 && tps != -1 && tps2 - tps < _doubleTapDelay && isLastTapShortTap();
	}
	
	private static bool isLastTapSingleTap()
	{
		return tps2 != -1 && tps != -1 && tps2 - tps >= _doubleTapDelay && isLastTapShortTap();
	}
	
	private static bool isLastTapShortTap()
	{
		float delay = -1;
		if(tps2 == -1)
		{
			delay = tps3 - tps;
		}
		else
		{
			delay = tps3 - tps2;
		}
		return delay < _stayDelay;
	}
	
	private static bool isLastTapTakenIntoAccount()
	{
		float delay = -1;
		if(tps2 == -1)
		{
			delay = Time.time - tps;
		}
		else
		{
			delay = Time.time - tps2;
		}
		return delay < _takeAccountDelay;
	}
	
	
	private static bool isMouseDoubleClicked()
	{
		return isLastTapsDoubleTap() && isLastTapTakenIntoAccount();
	}
	
	private static bool isMouseSingleClicked()
	{
		return isLastTapSingleTap() && isLastTapTakenIntoAccount();
	}
	
	private static bool isMouseClicked()
	{
		return isMouseSingleClicked() || isMouseDoubleClicked();
	}
	
	public static Camera FindCamera()
	{
		Camera cam = Camera.main;
		if(cam == null)
			cam = GameObject.FindObjectOfType<Camera>();
		return cam;
	}
	
	private static List<GameObject> RayCastAll()
	{
		List<GameObject> res = new List<GameObject>();
		Camera cam = FindCamera();
		if(cam == null)
			return res;
		Ray oRay = new Ray (cam.transform.position, cam.transform.forward);
		RaycastHit[] oHits = Physics.RaycastAll(oRay);
		
		foreach(RaycastHit hit in oHits)
		{
			res.Add (hit.collider.gameObject);
		}
		return res;
	}
	
	private static GameObject MouseRaycast()
	{
		Camera cam = FindCamera();
		if(cam == null)
			return null;
			
		Ray oRay = cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit oHit;
		if( Physics.Raycast ( oRay.origin, oRay.direction, out oHit, Mathf.Infinity ))
		{
			return oHit.collider.gameObject;
		}
		return null;
	}
	
	private static GameObject RayCast()
	{
		Camera cam = FindCamera();
		if(cam == null)
			return null;
		Ray oRay = new Ray (cam.transform.position, cam.transform.forward);
		RaycastHit oHit;
		if( Physics.Raycast ( oRay.origin, oRay.direction, out oHit, Mathf.Infinity ))
		{
			return oHit.collider.gameObject;
		}
		return null;
	}

	public static void Reset()
	{
		_bDoubleClicked = false;
		_bSingleClicked = false;
		_firstRaycasted = null;
		_firstMouseRaycasted = null;
		_raycasted = null;
		_touchPadEvent = false;
	}
	
	void Awake ( )
	{
		DontDestroyOnLoad( this );
	}
	
	void Start()
	{
		// Add a listener to the OVRMessenger for testing
		OVRTouchpad.TouchHandler += LocalTouchEventCallback;
	}
	
	private static bool _touchPadEvent = false;
	private static OVRTouchpad.TouchEvent _args;
	void LocalTouchEventCallback(object sender, System.EventArgs args)
	{
		_args = ((OVRTouchpad.TouchArgs)args).TouchType;
		_touchPadEvent = true;
	}
		
	void Update()
	{
		Reset ();
		_raycasted = RayCastAll();
		_firstRaycasted = RayCast();
		_firstMouseRaycasted = MouseRaycast();
		CheckMouse();
		_bDoubleClicked = isMouseDoubleClicked();
		_bSingleClicked = isMouseSingleClicked();
	}
	
	public static bool isTouchPadEvent()
	{
		return _touchPadEvent;
	}
	
	public static bool isScrollingUp()
	{
		return isTouchPadEvent() && _args == OVRTouchpad.TouchEvent.Up;
	}
	
	public static bool isScrollingDown()
	{
		return isTouchPadEvent() && _args == OVRTouchpad.TouchEvent.Down;
	}
	
	public static bool isScrollingLeft()
	{
		return isTouchPadEvent() && _args == OVRTouchpad.TouchEvent.Left;
	}
	
	public static bool isScrollingRight()
	{
		return isTouchPadEvent() && _args == OVRTouchpad.TouchEvent.Right;
	}
	
	public static bool isThereADoubleClick()
	{
		return _bDoubleClicked;
	}
	
	public static bool isThereASingleClick()
	{
		return _bSingleClicked;
	}
	
	public static bool isRaycasted(GameObject go)
	{
		return _raycasted.Contains(go);
	}
	
	public static bool isFirstRaycasted(GameObject go)
	{
		return _firstRaycasted == go;
	}
	
	public static bool isFirstMouseRaycasted(GameObject go)
	{
		return _firstMouseRaycasted == go;
	}
	
	public static bool isSingleClicked(GameObject go)
	{
		return isFirstRaycasted(go) && isThereASingleClick();
	}
	
	public static GameObject mouseRaycasted()
	{
		return _firstMouseRaycasted;
	}
	
	public static bool isRaycasting<T>()
	{
		if( _raycasted != null && _raycasted.Count == 0)
			return false;
		foreach(GameObject go in _raycasted)
		{
			if(go.GetComponent<T>() != null)
			{
				return true;
			}
		}
		return false;
	}
	
	public static bool isFirstRaycasting<T>()
	{
		return _firstRaycasted != null && _firstRaycasted.GetComponent<T>() != null;
	}
	
	public static GameObject raycasted()
	{
		return _firstRaycasted;
	}
	
	public static List<GameObject> allRaycasted()
	{
		return _raycasted;
	}
}
