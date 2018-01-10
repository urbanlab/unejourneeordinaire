using UnityEngine;
using System.Collections;
using EasyWiFi.ServerBackchannels;
using EasyWiFi.ServerControls;
using EasyWiFi.Core;

/// <summary>
/// Maps info with a client
/// </summary>
public class CClientMapping : MonoBehaviour
{
    [SerializeField]
    private IntServerBackchannel m_rClientNumberBackchannel;

    [SerializeField]
    private CustomFloatDataServerController m_rAngleXController;

    [SerializeField]
    private CustomFloatDataServerController m_rAngleYController;

	[SerializeField]
	private CustomFloatDataServerController m_rAngleZController;

	[SerializeField]
	private CustomStringDataServerController m_rStringData;

    private CAnglesToPosition m_rViewPort;

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
    void Start()
    {
        Debug.Assert(m_rAngleXController != null 
            && m_rAngleYController != null
			&& m_rAngleZController != null
			&& m_rStringData != null
            && m_rClientNumberBackchannel != null);
    }

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
    public void OnDestroy()
    {
        // In Unity, the gameobject == operator is overloaded to return true when compared to null
        // after Destroy() has already been called
        if (m_rViewPort.gameObject != null)
        {
            Destroy(m_rViewPort.gameObject);
        }
    }

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
//	public void OnMetadataChanged(string a_sMetadata)
//	{
//		Debug.Log (a_sMetadata + " was received");
//		if (a_sMetadata == "NextGoal")
//		{
//			CMainManager.Get ().NextGoal ();
//		}
//	}

	///-----------------------------------------------------------------------------------
	/// Initializes all values that will be receiving values from client													
	///-----------------------------------------------------------------------------------
	public void Init(int a_iPlayerNumber, CAnglesToPosition a_rViewPort, CStringFromClient a_rStringFromClient )
    {


        EasyWiFiConstants.PLAYER_NUMBER ePlayerId = (EasyWiFiConstants.PLAYER_NUMBER)a_iPlayerNumber;
        m_rViewPort = a_rViewPort;

		//Add listener to string receiver
		m_rStringData.m_strOnStringChanged.AddListener( a_rStringFromClient.OnStringChanged );
		m_rStringData.player = ePlayerId;

		//Add listener to floats receivers
        m_rAngleXController.m_fOnFloatChanged.AddListener(a_rViewPort.OnXAngleChanged);
        m_rAngleXController.player = ePlayerId;

        m_rAngleYController.m_fOnFloatChanged.AddListener(a_rViewPort.OnYAngleChanged);
        m_rAngleYController.player = ePlayerId;

		m_rAngleZController.m_fOnFloatChanged.AddListener (a_rViewPort.OnZAngleChanged);
		m_rAngleZController.player = ePlayerId;

        m_rClientNumberBackchannel.player = ePlayerId;
        a_rViewPort.SetPlayerNumber(a_iPlayerNumber);
        m_rClientNumberBackchannel.setValue(a_iPlayerNumber);
    }

}


