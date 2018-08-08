using System;
using System.Collections.Generic;
using System.Linq;
using FFImageLoading.Forms.Touch;
using Foundation;
using UIKit;
using UserNotifications;

namespace GTNTracker.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private GeoContainer _myGeofenceContainer = null;
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            CachedImageRenderer.Init();

            UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                (approved, error) => 
                {
                    Console.WriteLine($"Authorization for Notifications has been approved! state: {approved}");
                });

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            //UIBarButtonItem.Appearance.SetBackButtonTitlePositionAdjustment(new UIOffset(-130, 0), UIBarMetrics.Default);
            _myGeofenceContainer = new GeoContainer();

            UNUserNotificationCenter.Current.Delegate = new NotificationDelegate();
            UINavigationBar.Appearance.TintColor = UIColor.White;
            UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(0x42, 0x42, 0x42);
            var titleAttributes = UINavigationBar.Appearance.GetTitleTextAttributes();
            titleAttributes.TextColor = UIColor.White;
            UINavigationBar.Appearance.SetTitleTextAttributes(titleAttributes);

            UIProgressView.Appearance.TintColor = UIColor.FromRGB(0x1B, 0x5E, 0x20);

            return base.FinishedLaunching(app, options);
        }
    }
}
