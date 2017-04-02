using UnityEngine;
using System.Collections;

public class LoadingCircle : MonoBehaviour
{
	private bool _bIsEnabled = true;
	private Vector2 _v2CursorCounterTextureOffset;
	private float _fCursorPointerTime = 0.0F;
	private float _fTimeToSelect;
	private bool _bIsSelected;

	public bool IsEnabled()
	{
		return _bIsEnabled;
	}
	public void Enable( bool a_bValue )
	{
		_bIsEnabled = a_bValue;
	}

	public void IncreaseTimer()
	{
		if (_bIsEnabled) 
		{
			_fCursorPointerTime = Mathf.Clamp (_fCursorPointerTime + Time.deltaTime, 0, _fTimeToSelect);
		}
	}

	public void ResetCursor()
	{
		_bIsSelected = false;
		_fCursorPointerTime = 0.0F;
	}

	void Start()
	{
		_bIsSelected = false;
		_fTimeToSelect = 4.0f;
		_v2CursorCounterTextureOffset = GetComponent<Renderer>().material.mainTextureOffset;

	}
		

	void Update( )
	{
		_v2CursorCounterTextureOffset.x = (-1 / _fTimeToSelect) * _fCursorPointerTime;
		GetComponent<Renderer>().material.mainTextureOffset = _v2CursorCounterTextureOffset;

		if (_fCursorPointerTime >= _fTimeToSelect) 
		{
			_bIsSelected = true;
		}
	}

	IEnumerator Activate()
	{
		_bIsSelected = true;
		yield return null;
		ResetCursor ();
	}
		
	public bool IsSelected()
	{
		return _bIsSelected;
	}
}

