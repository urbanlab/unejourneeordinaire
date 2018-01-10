using UnityEngine;
using UnityEngine.UI;
using EasyWiFi.Core;
using EasyWiFi.ServerControls;
using EasyWiFi.ServerBackchannels;


[RequireComponent(typeof(RectTransform))]
public class CAnglesToPosition : MonoBehaviour
{
	public bool m_bDisplayInRect = false;

    private RectTransform m_rDisplayRect;

    // Offset to have the center at the good place
    private const float Y_OFFSET = 180f;

    private float m_fXAngle = 0f;
    private float m_fYAngle = 0f;
	private float m_fZAngle = 0f;

    private int m_iPlayerNumber;

    public Text m_rClientNumberText;
    public GameObject m_rRectangleObject;

	private GameObject _goCamera;
	Vector3 v3EulerAngles;

    // Use this for initialization
    void Start()
    {
		v3EulerAngles = new Vector3();
		_goCamera = GameObject.Find ("Camera");

//        transform.SetParent(m_rDisplayRect);
//
//        // Set the best dimensions
//        (transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_rDisplayRect.rect.width / 6f);
//        (transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_rDisplayRect.rect.height / 4f);
//
//        // Display the player number
//        if (m_rClientNumberText != null)
//        {
//            m_rClientNumberText.text = m_iPlayerNumber.ToString();
//        }
    }

    public void Update()
    {
		//UpdatePosFromEulerAngles(m_fXAngle, m_fYAngle, m_fZAngle);
        // It isn't in the view screen hierarchy (because it must not be disabled)
        // So we manually change the activation state
//            m_rRectangleObject.SetActive(CScreensManager.Instance.IsViewScreenDisplayed());

		if (_goCamera == null) 
		{
			_goCamera = GameObject.Find ("Camera");
		}
		else 
		{
			v3EulerAngles.Set( m_fXAngle, m_fYAngle, m_fZAngle );
			_goCamera.transform.rotation = Quaternion.Euler( v3EulerAngles );
		}


    }

	///-----------------------------------------------------------------------------------
	/// 								Callbacks activated when received info from client
	///-----------------------------------------------------------------------------------
    public void OnXAngleChanged(float a_fXAngle)
    {
        m_fXAngle = a_fXAngle;
    }

    public void OnYAngleChanged(float a_fYAngle)
    {
        m_fYAngle = a_fYAngle;
    }

	public void OnZAngleChanged(float a_fZAngle)
	{
		m_fZAngle = a_fZAngle;
	}

    /// <summary>
    /// Update the position of the object according to the given 3D angles.
    /// </summary>
	private void UpdatePosFromEulerAngles(float a_fXAngle, float a_fYAngle, float a_fZangle)
    {
		if (!m_bDisplayInRect) 
		{
			transform.localPosition = new Vector3 (a_fXAngle, a_fYAngle, a_fZangle);
			return;
		}

        var v3LocalPos = new Vector3(0, 0, 0);
        
        // The x angle is from 270 to 90, we shift it into [0, 180]
        a_fXAngle = (a_fXAngle + 90f) % 180f;

        // test
        a_fYAngle = (a_fYAngle + Y_OFFSET) % 360f;

        // local pos between [-parent.width/2, parent.width/2] (the anchor is in the center of the image)
        v3LocalPos.x = (a_fYAngle / 360f - 0.5f) * m_rDisplayRect.rect.width;
        v3LocalPos.y = -(a_fXAngle / 180f - 0.5f) * m_rDisplayRect.rect.height;

        transform.localPosition = v3LocalPos;
    }

    /// <summary>
    /// Set the player to follow
    /// </summary>
    public void SetPlayerNumber(int a_iPlayerNumber)
    {
        m_iPlayerNumber = a_iPlayerNumber;
    }

    /// <summary>
    /// Set the display zone covered by the image or video
    /// </summary>
    public void SetDisplayZone(RectTransform a_rDisplay)
    {
        m_rDisplayRect = a_rDisplay;
    }
}


