using System;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class WinRTDeviceHelper
    {
        public delegate void VibrationEventHandler(object sender, VibrationEventHandlerArgs args);
        
        //Create new events
        public static event EventHandler ShowAd;
        public static event EventHandler HideAd;
        public static event VibrationEventHandler VibratePhone;

        public static void FireShowAd()
        {
            Debug.Log ("Showing Ad......");
            //If event is subscribed than fire it
            if(ShowAd!=null)
                ShowAd(null,null);
        } 

        public static void FireHideAd()
        {
            Debug.Log ("Hiding Ad......");
            //If event is subscribed than fire it
            if(HideAd!=null)
                HideAd(null,null);
        }

        public static void FireVibratePhone(TimeSpan time)
        {
            Debug.Log("Vibrating......");
            //If event is subscribed than fire it
            if (VibratePhone != null)
                VibratePhone(null, new VibrationEventHandlerArgs { VibrationTime = time });
        }
    }

    public class VibrationEventHandlerArgs : EventArgs
    {
        public TimeSpan VibrationTime { get; set; }
    }
}
