using UnityEngine;
using System;

public static class WinRTAdHelper
{
        //Create new events
        public static event EventHandler ShowAd;
        public static event EventHandler HideAd;

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
}
