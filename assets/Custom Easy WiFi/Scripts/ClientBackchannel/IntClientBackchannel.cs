using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using EasyWiFi.Core;
using System;

namespace EasyWiFi.ClientBackchannels
{
    [Serializable]
    public sealed class IntEvent : UnityEvent<int> { }

    [AddComponentMenu("EasyWiFiController/Client/Backchannels/Int Backchannel")]
    public class IntClientBackchannel : MonoBehaviour, IClientBackchannel
    {

        public string controlName = "Int1";

        public IntEvent m_fOnValueChange;
        //public string notifyMethod = "yourMethod";

        //runtime variables
        IntBackchannelType intBackchannel = new IntBackchannelType();
        string backchannelKey;

        void Awake()
        {
            backchannelKey = EasyWiFiController.registerControl(EasyWiFiConstants.BACKCHANNELTYPE_INT, controlName);
            intBackchannel = (IntBackchannelType)EasyWiFiController.controllerDataDictionary[backchannelKey];

            if (m_fOnValueChange == null)
            {
                m_fOnValueChange = new IntEvent();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //if we have a populated server key then we know where to look in the data structure
            if (intBackchannel.serverKey != null)
            {
                mapDataStructureToMethod();
            }
        }


        public void mapDataStructureToMethod()
        {
            //SendMessage(notifyMethod, intBackchannel, SendMessageOptions.DontRequireReceiver);
            m_fOnValueChange.Invoke(intBackchannel.INT_VALUE);
        }
    }

}
