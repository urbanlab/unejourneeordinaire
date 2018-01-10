using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using EasyWiFi.Core;

/// <summary>
/// Handles the different viewports displayed on the tablet screen.
/// Automatically create and destroy rectangles according to clients connections et disconnections
/// </summary>
public class CInfoFromClientManager : MonoBehaviour
{
    /// <summary>
    /// Player number -> Rectangle object
    /// </summary>
    private IDictionary<int, CClientMapping> m_mViewPorts = new Dictionary<int, CClientMapping>();

	[SerializeField]
	private bool bDisplayViewsOfClientsInServerTablet = false;

    [SerializeField]
	private RectTransform m_rDisplayZone;

    [SerializeField]
    private CAnglesToPosition m_rViewPortPrefab;

    [SerializeField]
    private CClientMapping m_rClientMappingPrefab;

	[SerializeField]
	private CStringFromClient m_rStringFromClient;

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
    void Awake()
    {
		Debug.Log ("Registering delegate into EasyWifiController");
		EasyWiFiController.On_ConnectionsChanged += OnClientConnectionChanged;

		if ( bDisplayViewsOfClientsInServerTablet )
		{
			Debug.Assert(m_rDisplayZone != null && m_rViewPortPrefab != null);
		}
    }

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
    private void OnClientConnectionChanged(bool a_bConnect, int a_iPlayer)
    {
        if (a_bConnect)
        {
			Debug.Log (" CClientViewportsManager On client connection");
            OnClientConnection(a_iPlayer);
        }
        else
        {
			Debug.Log (" CClientViewportsManager On client DISCONNECTION");
            OnClientDisconnection(a_iPlayer);
        }
    }

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
    private void OnClientConnection(int a_iPlayerNumber)
    {
		
      //  if (!m_mViewPorts.ContainsKey(a_iPlayerNumber))
      //  {
            StartCoroutine( ClientMappingCoroutine(a_iPlayerNumber) );
			Debug.Log ("********************** CLIENT CONNECTION *****************************");
       // }
    }

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
    private IEnumerator ClientMappingCoroutine(int a_iPlayerNumber)
    {
		
        yield return new WaitForEndOfFrame();

       	CAnglesToPosition oViewPort = Instantiate(m_rViewPortPrefab);
		oViewPort.SetDisplayZone(m_rDisplayZone);
		oViewPort.SetPlayerNumber(a_iPlayerNumber + 1);

        CClientMapping oClientMapping = Instantiate(m_rClientMappingPrefab);
        m_mViewPorts.Add(a_iPlayerNumber, oClientMapping);

		CStringFromClient oStringFromClient = Instantiate( m_rStringFromClient );

		oClientMapping.Init(a_iPlayerNumber, oViewPort, oStringFromClient);
    }

	///-----------------------------------------------------------------------------------
	/// 																			
	///-----------------------------------------------------------------------------------
    private void OnClientDisconnection(int a_iPlayerNumber)
    {
        CClientMapping oClientMapping;
        if (m_mViewPorts.TryGetValue(a_iPlayerNumber, out oClientMapping))
        {
            Destroy(oClientMapping.gameObject);
            m_mViewPorts.Remove(a_iPlayerNumber);
        }
    }

}