using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms;
using FFImageLoading.Forms.Droid;
using GTNTracker.Interfaces;
using GTNTracker.Services;
using Plugin.Permissions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace GTNTracker.Droid
{
    //Theme = "@style/MyTheme"

    [Activity(Label = "GTN Tracker", Icon = "@drawable/ic_gtnRound", Theme = "@style/MyTheme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : FormsAppCompatActivity
    {
        private bool _requestedPermissions;
        private bool _serviceStarted = false;

        public static MainActivity SharedInstance { get; set; }
        public int WaypointMailedId { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;
            SharedInstance = this;
            

            base.SetTheme(Resource.Style.MyTheme);  // put theme back to normal which essentially turns off the splash screen

            Bundle extras = Intent.Extras;

            if (extras != null)
            {
                var regionId = extras.GetString("NotificationRegion");
                if (!string.IsNullOrEmpty(regionId))
                {
                    AppStateService.Instance.IsNotificationLaunch = true;
                }
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;

            FormsAppCompatActivity.ToolbarResource = Resource.Layout.Toolbar;
            FormsAppCompatActivity.TabLayoutResource = Resource.Layout.Tabbar;

            //LocalNotificationsImplementation.NotificationIconId = Resource.Drawable.notifyIcon;
            //Plugin.Notifications.NotificationsImpl.AppIconResourceId = Resource.Drawable.notifyIcon;
            CachedImageRenderer.Init();
            CachedImage.FixedOnMeasureBehavior = true;

            var service = TrailVisitService.Instance;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            MessagingCenter.Subscribe<StartGeofencing>(this, StartGeofencing.MessageString,
                                (sender) => { HandleStart(); });

            MessagingCenter.Subscribe<StopGeofencing, StopGeofencingArgs>(this, StopGeofencing.MessageString,
                    (sender, args) => { HandleStop(args); });
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 12345)
            {
                MessagingCenter.Send<WaypointEmailed, WaypointEMailedArgs>(new WaypointEmailed(), WaypointEmailed.MessageString, new WaypointEMailedArgs(this.WaypointMailedId));
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("=====> Unhandled Exception!!!, {0}", e.ExceptionObject.ToString());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (!_requestedPermissions)
            {
                _requestedPermissions = true;
                PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("-------->>>>>>>>Destroying application, final cleanups!");
            MessagingCenter.Unsubscribe<StartGeofencing, StopGeofencingArgs>(this, StartGeofencing.MessageString);
            MessagingCenter.Unsubscribe<StopGeofencing, StopGeofencingArgs>(this, StopGeofencing.MessageString);
            base.OnDestroy();
            HandleStop(null);

            NotificationManager notificationManager =
                   GetSystemService(Context.NotificationService) as NotificationManager;

            // we have to get rid of the trail notifications since we could be going away, don't want to leave them around????
            notificationManager.Cancel(999888111);
            notificationManager.CancelAll();

            // alert app that the OnDestroy of the activity has occurred.
            MessagingCenter.Send<NotificationDestroyEvent>(new NotificationDestroyEvent(), NotificationDestroyEvent.MessageString);
        }

        private void HandleStart()
        {
            //GeoFenceService.Start();
            // don't need to since service cranks up at creation
            //Start();
            if (!_serviceStarted)
            {
                var intent = new Intent(this, typeof(GeofenceServiceContainer));
                StartService(intent);
                _serviceStarted = true;
            }
            
        }

        private void HandleStop(StopGeofencingArgs args)
        {
            if (_serviceStarted || (args != null && args.ForceStop))
            {
                var intent = new Intent(this, typeof(GeofenceServiceContainer));
                StopService(intent);
                _serviceStarted = false;
            }
        }
    }
}

