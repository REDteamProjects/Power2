using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Windows.Phone.Devices.Notification;
using Assets.Scripts.Helpers;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using UnityApp = UnityPlayer.UnityApp;
using UnityBridge = WinRTBridge.WinRTBridge;
using Microsoft.Devices;

namespace Power2
{
	public partial class MainPage : PhoneApplicationPage
	{
		private bool _unityStartedLoading;

		// Constructor
		public MainPage()
		{
			var bridge = new UnityBridge();
			UnityApp.SetBridge(bridge);
			InitializeComponent();
			bridge.Control = DrawingSurfaceBackground;
            WinRTDeviceHelper.HideAd += WinRTDeviceHelperOnHideAd;
            WinRTDeviceHelper.ShowAd += WinRTDeviceHelperOnShowAd;
            WinRTDeviceHelper.VibratePhone += WinRTDeviceHelper_VibratePhone;
            MyAd.ErrorOccurred += MyAd_ErrorOccurred;
		}

        void MyAd_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            Debug.WriteLine(e.Error);
        }

        void WinRTDeviceHelper_VibratePhone(object sender, VibrationEventHandlerArgs args)
        {
            Dispatcher.BeginInvoke( () =>
            {
                var vibrationDevice = VibrateController.Default;//VibrationDevice.GetDefault();
                if (args.VibrationTime.Milliseconds == 0)
                    vibrationDevice.Stop();//Cancel();
                else
                    vibrationDevice.Start(args.VibrationTime);//.Vibrate(args.VibrationTime);
            });
        }

        void WinRTDeviceHelperOnShowAd(object sender, EventArgs eventArgs)
	    {
            Dispatcher.BeginInvoke( () =>
            {
                MyAd.Visibility = Visibility.Visible;
            });
	    }

        void WinRTDeviceHelperOnHideAd(object sender, EventArgs eventArgs)
	    {
            Dispatcher.BeginInvoke(() =>
            {
                MyAd.Visibility = Visibility.Collapsed;
            });
	    }

	    private void DrawingSurfaceBackground_Loaded(object sender, RoutedEventArgs e)
		{
			if (!_unityStartedLoading)
			{
				_unityStartedLoading = true;

				//UnityApp.SetLoadedCallback(() => { Dispatcher.BeginInvoke(Unity_Loaded); });

				var content = Application.Current.Host.Content;
				var nativeWidth = (int)Math.Floor(content.ActualWidth * content.ScaleFactor / 100.0 + 0.5);
				var nativeHeight = (int)Math.Floor(content.ActualHeight * content.ScaleFactor / 100.0 + 0.5);
				
				var physicalWidth = nativeWidth;
				var physicalHeight = nativeHeight;
				object physicalResolution;

				if (DeviceExtendedProperties.TryGetValue("PhysicalScreenResolution", out physicalResolution))
				{
					var resolution = (System.Windows.Size)physicalResolution;
					var nativeScale = content.ActualHeight / content.ActualWidth;
					var physicalScale = resolution.Height / resolution.Width;
					// don't use physical resolution for devices that don't have hardware buttons (e.g. Lumia 630)
					if (Math.Abs(nativeScale - physicalScale) < 0.01)
					{
						physicalWidth = (int)resolution.Width;
						physicalHeight = (int)resolution.Height;
					}
				}

				UnityApp.SetNativeResolution(nativeWidth, nativeHeight);
				UnityApp.SetRenderResolution(physicalWidth, physicalHeight);
				UnityApp.SetOrientation((int)Orientation);

				DrawingSurfaceBackground.SetBackgroundContentProvider(UnityApp.GetBackgroundContentProvider());
				DrawingSurfaceBackground.SetBackgroundManipulationHandler(UnityApp.GetManipulationHandler());
			}
		}

		/*private void Unity_Loaded()
		{
		}*/

		private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
		{
			e.Cancel = UnityApp.BackButtonPressed();
		}

		private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
		{
			UnityApp.SetOrientation((int)e.Orientation);
		}
	}
}
