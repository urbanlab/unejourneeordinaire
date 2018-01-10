using UnityEngine;
using UnityEngine.Events;
using EasyWiFi.Core;
using System.Collections;


public class CConnectionStateReporter : MonoBehaviour {

    public UnityEvent m_fOnConnection;
    public UnityEvent m_fOnDisconnection;

    private bool m_bLastConnectionState = false;

    // Use this for initialization
    void Start () 
	{
        if (m_fOnConnection == null)
        {
            m_fOnConnection = new UnityEvent();
        }

        if (m_fOnDisconnection == null)
        {
            m_fOnDisconnection = new UnityEvent();
        }
    }

    // Update is called once per frame
    void Update () 
	{
        bool bNewConnectionState =
            (EasyWiFiController.clientState == EasyWiFiConstants.CURRENT_CLIENT_STATE.SendingControllerData);
	
        if (bNewConnectionState != m_bLastConnectionState)
        {
            if (bNewConnectionState)
            {
				Debug.Log ("CConnectionStateReporter::On Connection state ");
                m_fOnConnection.Invoke();
            }
            else
            {
				Debug.Log ("CConnectionStateReporter::On Disconnection state ");
                m_fOnDisconnection.Invoke();
            }
            m_bLastConnectionState = bNewConnectionState;
        }
    }
}


