using UnityEngine;
using UnityEngine.UI;
using EasyWiFi.ClientBackchannels;
using EasyWiFi.Core;


[RequireComponent(typeof(Text))]
public class CDisplayClientNumber : MonoBehaviour {

    private Text m_rTextComponent;
    private float m_fAlpha = 0f;

    private int m_iPlayerNumber = EasyWiFiConstants.PLAYERNUMBER_DISCONNECTED;

    [Range(0.1f, 5f)]
    public float m_fFadingLength = 0.5f;

    [Tooltip("Reference to a EasyWifi client controller to get the player number")]
    public IntClientBackchannel m_rClientNumberBackchannel;

    // Use this for initialization
    void Start () {
        m_rTextComponent = GetComponent<Text>();

        m_rClientNumberBackchannel.m_fOnValueChange.AddListener(OnClientNumberChanged);
    }

    // Update is called once per frame
    void Update () {
        if (m_fAlpha > 0f)
        {
            m_fAlpha -= Time.deltaTime / m_fFadingLength;
            if (m_fAlpha <= 0f)
            {
                m_fAlpha = 0f;
            }
        }

        // On Gear, the touchpad is considered like the left mouse button
        if (Input.GetMouseButton(0))
        {
            m_fAlpha = 1f;
        }

        var oColor = m_rTextComponent.color;
        oColor.a = m_fAlpha;
        m_rTextComponent.color = oColor;

        if (m_iPlayerNumber != EasyWiFiConstants.PLAYERNUMBER_DISCONNECTED &&
            m_iPlayerNumber != EasyWiFiConstants.PLAYERNUMBER_ANY &&
            m_iPlayerNumber >= 0)
        {
            m_rTextComponent.text = m_iPlayerNumber.ToString();
        }
        else
        {
            m_rTextComponent.text = "?";
        }

    }

    private void OnClientNumberChanged(int a_iClientNumber)
    {
        m_iPlayerNumber = a_iClientNumber;
    }
}


