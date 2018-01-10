﻿/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.2 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-3.2

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using UnityEngine;
using System;

/// <summary>
/// Interface class to a touchpad.
/// </summary>
public static class TouchPad
{
	/// <summary>
	/// Touch Type.
	/// </summary>
	public enum TouchEvent
	{
		SingleTap,
		Left,
		Right,
		Up,
		Down,
	};
	
	public class TouchArgs : EventArgs
	{
		public TouchEvent TouchType;
	}
	
	/// <summary>
	/// Occurs when touched.
	/// </summary>
	public static event EventHandler TouchHandler;
	
	/// <summary>
	/// Native Touch State.
	/// </summary>
	enum TouchState
	{
		Init,
		Down,
		Stationary,
		Move,
		Up
	};
	
	static TouchState touchState = TouchState.Init;
	static Vector2 moveAmount;
	static float minMovMagnitude = 100.0f; // Tune this to gage between click and swipe
	
	// mouse
	static Vector3 moveAmountMouse;
	static float minMovMagnitudeMouse = 25.0f;
	
	// Disable the unused variable warning
	#pragma warning disable 0414
	// Ensures that the TouchpadHelper will be created automatically upon start of the scene.
	static private TouchpadHelper touchpadHelper =
		(new GameObject("TouchpadHelper")).AddComponent<TouchpadHelper>();
	#pragma warning restore 0414
	
	/// <summary>
	/// Add the Touchpad game object into the scene.
	/// </summary>
	static public void Create()
	{
		// Does nothing but call constructor to add game object into scene
	}
	
	static public void Update()
	{
		// MOUSE INPUT
		if (Input.GetMouseButtonDown(0))
		{
			//Debug.Log ("OnOVRTouchpadMouseDown");
			moveAmountMouse = Input.mousePosition;
			touchState = TouchState.Down;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			//Debug.Log ("OnOVRTouchpadMouseUP");
			moveAmountMouse -= Input.mousePosition;
			HandleInputMouse(ref moveAmountMouse);
			touchState = TouchState.Init;
		}
	}
	
	static public void OnDisable()
	{
	}
	
	/// <summary>
	/// Determines if input was a click or swipe and sends message to all prescribers.
	/// </summary>
	static void HandleInput(TouchState state, ref Vector2 move)
	{
		if ((move.magnitude < minMovMagnitude) || (touchState == TouchState.Stationary))
		{
			//Debug.Log( "CLICK" );
		}
		else if (touchState == TouchState.Move)
		{
			move.Normalize();
			
			// Left
			if(Mathf.Abs(move.x) > Mathf.Abs (move.y))
			{
				if(move.x > 0.0f)
				{
					//Debug.Log( "SWIPE: LEFT" );
				}
				else
				{
					//Debug.Log( "SWIPE: RIGHT" );
				}
			}
			// Right
			else
			{
				if(move.y > 0.0f)
				{
					//Debug.Log( "SWIPE: DOWN" );
				}
				else
				{
					//Debug.Log( "SWIPE: UP" );
				}
			}
		}
	}
	
	static void HandleInputMouse(ref Vector3 move)
	{
		
		if (move.magnitude < minMovMagnitudeMouse)
		{
			if (TouchHandler != null)
			{
				//				Debug.Log ("OnOVRTouchpadSingleTap");
				TouchHandler(null, new TouchArgs() { TouchType = TouchEvent.SingleTap });
			}
		}
		else
		{
			move.Normalize();
			
			// Left/Right
			if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
			{
				if (move.x > 0.0f)
				{
					if (TouchHandler != null)
					{
						TouchHandler(null, new TouchArgs () { TouchType = TouchEvent.Left });
					}
				}
				else
				{
					if (TouchHandler != null)
					{
						TouchHandler(null, new TouchArgs () { TouchType = TouchEvent.Right });
					}
				}
			}
			// Up/Down
			else
			{
				if (move.y > 0.0f)
				{
					if (TouchHandler != null)
					{
						TouchHandler(null, new TouchArgs () { TouchType = TouchEvent.Down });
					}
				}
				else
				{
					if(TouchHandler != null)
					{
						TouchHandler(null, new TouchArgs () { TouchType = TouchEvent.Up });
					}
				}
			}
		}
	}
}

/// <summary>
/// This singleton class gets created and stays resident in the application. It is used to 
/// trap the touchpad values, which get broadcast to any listener on the "Touchpad" channel.
/// </summary>
public sealed class TouchpadHelper : MonoBehaviour
{
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
	
	void Start()
	{
		// Add a listener to the OVRMessenger for testing
		TouchPad.TouchHandler += LocalTouchEventCallback;
	}
	
	void Update()
	{
		TouchPad.Update();
	}
	
	public void OnDisable()
	{
		TouchPad.OnDisable();
	}
	
	void LocalTouchEventCallback(object sender, EventArgs args)
	{
		var touchArgs = (TouchPad.TouchArgs)args;
		TouchPad.TouchEvent touchEvent = touchArgs.TouchType;
		
		switch(touchEvent)
		{
		case TouchPad.TouchEvent.SingleTap:
			//Debug.Log("SINGLE CLICK\n");
			break;
			
		case TouchPad.TouchEvent.Left:
			//Debug.Log("LEFT SWIPE\n");
			break;
			
		case TouchPad.TouchEvent.Right:
			//Debug.Log("RIGHT SWIPE\n");
			break;
			
		case TouchPad.TouchEvent.Up:
			//Debug.Log("UP SWIPE\n");
			break;
			
		case TouchPad.TouchEvent.Down:
			//Debug.Log("DOWN SWIPE\n");
			break;
		}
	}
}
