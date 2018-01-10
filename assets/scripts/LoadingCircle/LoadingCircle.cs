using UnityEngine;
using System.Collections;

public class LoadingCircle : MonoBehaviour
{
	private MAppManager _gameManager;

	private bool _bIsInitialized = false;
	private bool _bIsEnabled = true;

	private MeshRenderer _mrCursorPointer;
	private Vector2 _v2CursorCounterTextureOffset;
	private float _fCursorPointerTime = 0.0F;

    private float _fTimeToSelect;
    private bool _bIsSelected;

    //private Texture _textureNormal;
    //private Texture _textureHover;

    public bool IsEnabled()
    {
        return _bIsEnabled;
    }
    public void Enable(bool a_bValue)
    {
        _bIsEnabled = a_bValue;
    }

    public void IncreaseTimer()
    {
        if (_bIsEnabled)
        {
            _fCursorPointerTime = Mathf.Clamp(_fCursorPointerTime + Time.deltaTime, 0, _fTimeToSelect);
        }
    }

    public void HoverCursor()
    {
        //GameObject.Find("crosshair").GetComponent<Renderer>().material.mainTexture = _textureHover;
    }


    public void ResetCursor()
    {
        _bIsSelected = false;
        _fCursorPointerTime = 0.0F;
        //GameObject.Find("crosshair").GetComponent<Renderer>().material.mainTexture = _textureNormal;
    }

	public int IsIncreasing( )
	{
		if ( _fCursorPointerTime > 0.0f) 
		{
			return 1;
		}
		return 0;
	}

    void Start()
    {
        _bIsSelected = false;
        _fTimeToSelect = 2.0f;
        _v2CursorCounterTextureOffset = GetComponent<Renderer>().material.mainTextureOffset;
        //_textureHover = Resources.Load<Texture>("Textures/pointer_hover");
        //_textureNormal = Resources.Load<Texture>("Textures/pointer_normal");
        OVRTouchpad.TouchHandler += HandleTouchHandler;
    }

    void HandleTouchHandler(object sender, System.EventArgs args)
    {
        var touchArgs = (OVRTouchpad.TouchArgs)args;
        OVRTouchpad.TouchEvent touchEvent = touchArgs.TouchType;

        switch (touchEvent)
        {
            case OVRTouchpad.TouchEvent.SingleTap:
                StartCoroutine(Activate());
                break;
        }
    }

    void Update()
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
        ResetCursor();
    }

    public bool IsSelected()
    {
        return _bIsSelected;
    }
}
