using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;


[Serializable]
public class CStringEvent : UnityEvent<string> { }

[Serializable]
public class CBoolEvent : UnityEvent<bool> { }

[Serializable]
public class CIntEvent : UnityEvent<int> { }

public class CCommandsReceiver : MonoBehaviour {

    private static CCommandsReceiver s_oInstance;

    public CStringEvent m_fOnOpenPhoto;
    public CStringEvent m_fOnOpenVideo;
    public UnityEvent m_fOnVideoPause;
    public UnityEvent m_fOnVideoPlay;
    public CIntEvent m_fOnSetVideoTime;
	public CBoolEvent m_fOnTrial;

    public void Awake()
    {
        s_oInstance = this;
    }

    public void OnDestroy()
    {
        s_oInstance = null;
    }

    /// <summary>
    /// Link this method to the callback of string backchannel corresponding to commands.
    /// It automatically calls the good method handler depending on the command type
    /// </summary>
    public void OnCommandChanged(string a_sCommand)
    {
        if (a_sCommand == null)
        {
            return;
        }

        string sPrefix = String.Empty;
        string sData = String.Empty;

        if (a_sCommand.Contains(CCommandsConstants.SEPARATOR))
        {
            int iSeparatorIndex = a_sCommand.IndexOf(CCommandsConstants.SEPARATOR);

            sPrefix = a_sCommand.Substring(0, iSeparatorIndex);

            if (iSeparatorIndex + CCommandsConstants.SEPARATOR.Length < a_sCommand.Length)
            {
                sData = a_sCommand.Substring(iSeparatorIndex + CCommandsConstants.SEPARATOR.Length);
            }
        }
        else
        {
            sPrefix = a_sCommand;
        }

        switch (sPrefix)
        {
			case CCommandsConstants.TRIAL_VERSION:
				if (m_fOnTrial != null) 
				{
					m_fOnTrial.Invoke (bool.Parse (sData));
				}
				break;
			
            case CCommandsConstants.OPEN_PHOTO_PREFIX:
                if (m_fOnOpenPhoto != null)
                {
                    m_fOnOpenPhoto.Invoke(sData);
                }
                break;
            case CCommandsConstants.OPEN_VIDEO_PREFIX:
                if (m_fOnOpenVideo != null)
                {
                    m_fOnOpenVideo.Invoke(sData);
                }
                break;
            case CCommandsConstants.PAUSE:
                if (m_fOnVideoPause != null)
                {
                    m_fOnVideoPause.Invoke();
                }
                break;
            case CCommandsConstants.PLAY:
                if (m_fOnVideoPlay != null)
                {
                    m_fOnVideoPlay.Invoke();
                }
                break;
            case CCommandsConstants.SET_TIME:
                if (m_fOnSetVideoTime != null)
                {
                    int iTime;
                    if (int.TryParse(sData, out iTime))
                    {
                        m_fOnSetVideoTime.Invoke(iTime);
                    }
                    else
                    {
                        Debug.LogError("Invalid time: " + sData);
                    }
                }
                break;
            default:
                Debug.LogError("Invalid command prefix: " + sPrefix);
                break;
        }
    }
}


