using UnityEngine;
using System.Collections;
using EasyWiFi.ServerBackchannels;


public class CCommandsSender : MonoBehaviour
{
    public StringServerBackchannel m_oCommandBackchannel;

    private static CCommandsSender s_oInstance;

    public void Awake()
    {
        s_oInstance = this;
    }

    public void OnDestroy()
    {
        s_oInstance = null;
    }
		
    public static void OpenPhoto(string a_sRelPath)
    {
        s_oInstance.m_oCommandBackchannel.setValue(
            CCommandsConstants.OPEN_PHOTO_PREFIX +
            CCommandsConstants.SEPARATOR +
            a_sRelPath);
    }

    public static void OpenVideo(string a_sRelPath)
    {
        s_oInstance.m_oCommandBackchannel.setValue(
            CCommandsConstants.OPEN_VIDEO_PREFIX +
            CCommandsConstants.SEPARATOR +
            a_sRelPath);
    }

    /// <summary>
    /// Set the video lecture position (in seconds)
    /// </summary>
    /// <param name="a_sTime">Time in seconds</param>
    public static void SetVideoTime(int a_sTime)
    {
        s_oInstance.m_oCommandBackchannel.setValue(
            CCommandsConstants.SET_TIME +
            CCommandsConstants.SEPARATOR +
            a_sTime.ToString());
    }

    public static void PauseVideo()
    {
        s_oInstance.m_oCommandBackchannel.setValue(
            CCommandsConstants.PAUSE);
    }

    public static void PlayVideo()
    {
        s_oInstance.m_oCommandBackchannel.setValue(
            CCommandsConstants.PLAY);
    }
}


