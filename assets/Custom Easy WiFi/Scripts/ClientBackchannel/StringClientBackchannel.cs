using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using EasyWiFi.Core;
using System;

[Serializable]
public class StringChangedHandler: UnityEvent<string> { }

namespace EasyWiFi.ClientBackchannels
{
    [AddComponentMenu("EasyWiFiController/Client/Backchannels/String Backchannel")]
    public class StringClientBackchannel : MonoBehaviour, IClientBackchannel
    {

        public string controlName = "String1";
        //public string notifyMethod = "_onControlChange";
        public StringChangedHandler m_fAction;

        [Tooltip("Determines when your Notify Method gets called")]
        public EasyWiFiConstants.CALL_TYPE callType = EasyWiFiConstants.CALL_TYPE.Every_Frame;

        //runtime variables
        StringBackchannelType stringBackchannel = new StringBackchannelType();
        string backchannelKey;
        string lastValue = "";

        void Awake()
        {
            backchannelKey = EasyWiFiController.registerControl(EasyWiFiConstants.BACKCHANNELTYPE_STRING, controlName);
            stringBackchannel = (StringBackchannelType)EasyWiFiController.controllerDataDictionary[backchannelKey];
        }

        // Update is called once per frame
        void Update()
        {
            //if we have a populated server key then we know where to look in the data structure
            if (stringBackchannel.serverKey != null)
            {
                mapDataStructureToMethod();
            }
        }

        public void mapDataStructureToMethod()
        {
            if (callType == EasyWiFiConstants.CALL_TYPE.Every_Frame)
            {
                //SendMessage(notifyMethod, stringBackchannel, SendMessageOptions.DontRequireReceiver);
                m_fAction.Invoke(stringBackchannel.STRING_VALUE);
            }
            else
            {
                if (lastValue != stringBackchannel.STRING_VALUE)
                {
                    //SendMessage(notifyMethod, stringBackchannel, SendMessageOptions.DontRequireReceiver);
                    m_fAction.Invoke(stringBackchannel.STRING_VALUE);
                }
                lastValue = stringBackchannel.STRING_VALUE;
            }
        }
    }

}
