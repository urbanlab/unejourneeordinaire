using UnityEngine;
using System.Collections;
using EasyWiFi.ClientControls;


public class CRelativeOrientationClientController : MonoBehaviour
{

	public FloatDataClientController m_oXAngleController;
    public FloatDataClientController m_oYAngleController;
	public FloatDataClientController m_oZAngleController;


	private GameObject _goCamera;
    public Transform m_oReference;

	private static CRelativeOrientationClientController s_oInstance = null;

	public static CRelativeOrientationClientController Get()
	{
		return s_oInstance;
	}

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
    void Start()
    {
		Debug.Assert(m_oXAngleController != null && m_oYAngleController != null && m_oXAngleController != null
             && m_oReference != null);

		s_oInstance = this;
		_goCamera = GameObject.Find ("Camera");
    }

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
	public GameObject GetCamera()
	{
		return _goCamera;
	}

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
    void Update()
    {
		if ( _goCamera  !=  null) 
		{
			var v3Diff = _goCamera.transform.rotation.eulerAngles - m_oReference.rotation.eulerAngles;

			float fAngleX = (v3Diff.x + 360f) % 360f;
			float fAngleY = (v3Diff.y + 360f) % 360f;
			float fAngleZ = (v3Diff.z + 360f) % 360f;

			// Send values from client to server (this script must be attached in the client side) 
			m_oXAngleController.setValue ( fAngleX );
			m_oYAngleController.setValue ( fAngleY );
			m_oZAngleController.setValue ( fAngleZ );
		}
		else
		{
			if ( GameObject.Find("Camera") != null )
			{
				_goCamera = GameObject.Find ("Camera").transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
			}
		}


    }

}


