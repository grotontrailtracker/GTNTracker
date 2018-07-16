using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using GTNTracker.Interfaces;
using GTNTracker.Services;
using Xamarin.Forms;

namespace GTNTracker.Droid
{

    public class RegionNotificationMsg
    {
        public RegionNotificationMsg(string id, string title, string message)
        {
            RegionId = id;
            Message = message;
            Title = title;
        }

        public string Title { get; set; }
        public string Message { get; set; }

        public string RegionId { get; set; }
    }

    public class TimerTick
    {
        private System.Timers.Timer myTimer;
        public TimerTick()
        {
            myTimer = new System.Timers.Timer(30000);
            myTimer.Elapsed += new ElapsedEventHandler(OnElapsed);
            myTimer.AutoReset = false;
        }

        public void Start()
        {
            myTimer.Start();
        }

        public void Stop()
        {
            myTimer.Stop();
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            // Do stuff
            //Console.WriteLine("---->I'm still running :)");

            myTimer.Start(); // Restart timer
        }
    }

    [Service]
    public class GeofenceServiceContainer : Service
    {
        private static int _serviceNotificationId = 9990000;

        private int _notificationId = 999888111;
        private int _currServiceCntr;
        private string _gpsRunningNotificationChannel = string.Empty;
        private Notification.Builder _builder;
        private Notification.InboxStyle _inboxStyle;
        private List<RegionNotificationMsg> _currNotifications = new List<RegionNotificationMsg>();
        private TimerTick _myTimer = new TimerTick();

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        private CancellationTokenSource _cts;

        public override void OnCreate()
        {
            base.OnCreate();
            Console.WriteLine("Creating the GeofenceServiceContainer");
            MessagingCenter.Subscribe<GeofenceVisited, GeofenceVisitedArgs>(this, GeofenceVisited.MessageString,
                                (sender, args) => { HandleGeofenceVisitedMessage(args); });
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            _cts = new CancellationTokenSource();

            Task.Run(() => {
                try
                {
                    _currServiceCntr = _serviceNotificationId;
                    _serviceNotificationId++;
                    Notification builder;

                    var tmpbuilder = new Notification.Builder(this)
                        .SetContentTitle("Trail Tracker Service")
                        .SetContentText("GPS Service is running")
                        .SetSmallIcon(Resource.Drawable.ic_directions_walk_white)
                        .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.gtn))
                        .SetOngoing(true);   // this means the user cannot dismiss until the app is killed.

                    if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.O)
                    {
                        builder = tmpbuilder.Build();
                    }
                    else
                    {
                        _gpsRunningNotificationChannel = PackageName;
                        var notificationManager = NotificationManager.FromContext(BaseContext);
                        NotificationChannel channel;
                        channel = notificationManager.GetNotificationChannel(_gpsRunningNotificationChannel);
                        if (channel == null)
                        {
                            channel = new NotificationChannel(_gpsRunningNotificationChannel, "GPS Service", NotificationImportance.Default);
                            channel.LockscreenVisibility = NotificationVisibility.Public;
                            notificationManager.CreateNotificationChannel(channel);
                        }
                        channel?.Dispose();

                        builder = tmpbuilder
                        .SetChannelId(_gpsRunningNotificationChannel)
                        .Build();
                    }

                    Console.WriteLine("---->Starting Geofence container instance: {0}", _currServiceCntr);
                   
                    StartForeground(_currServiceCntr, builder);

                    GeoFenceService.Start();

                    _myTimer.Start();

                }
                catch (System.OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                }
                finally
                {
                    if (_cts.IsCancellationRequested)
                    {
                        //var message = new CancelledMessage();
                        //Device.BeginInvokeOnMainThread(
                        //    () => MessagingCenter.Send(message, "CancelledMessage")
                        //);
                    }
                }

            }, _cts.Token);

            return StartCommandResult.Sticky;
        }

        private void HandleGeofenceVisitedMessage(GeofenceVisitedArgs args)
        {
            var region = args.Region;
            var title = string.Format("{0} - Waypoint Reached", string.IsNullOrEmpty(region.TrailName) ? "Trail" : region.TrailName);
            var msg = "Arrived at " + region.Name;
            var inboxMsg = string.Format("Trail: {0} - Waypoint: {1}", string.IsNullOrEmpty(region.TrailName) ? "<none>" : region.TrailName, region.Name);

            var notificationMsg = new RegionNotificationMsg(region.Identifier, title, inboxMsg);
            _currNotifications.Add(notificationMsg);

            if (_builder == null)
            {
                var launchIntent = Android.App.Application.Context.PackageManager.GetLaunchIntentForPackage(Android.App.Application.Context.PackageName);
                launchIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                var currNotification = _currNotifications.Last();
                launchIntent.PutExtra("NotificationRegion", currNotification.RegionId);

                var pendingIntent = Android.App.TaskStackBuilder
                        .Create(Android.App.Application.Context)
                        .AddNextIntent(launchIntent)
                        .GetPendingIntent(_notificationId, PendingIntentFlags.OneShot);

                _inboxStyle = new Notification.InboxStyle();
                _inboxStyle.AddLine(inboxMsg);

                _builder = new Notification.Builder(this);
                _builder.SetContentTitle("Waypoints Visited");
                _builder.SetOngoing(false);
                _builder.SetSmallIcon(Resource.Drawable.notifyIcon);
                _builder.SetAutoCancel(true);
                _builder.SetContentIntent(pendingIntent);
                _builder.SetStyle(_inboxStyle);

                if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.O)
                {
                    //_builder = new Notification.Builder(this);
                    //_builder.SetContentTitle("Waypoints Visited");
                    //_builder.SetOngoing(false);
                    _builder.SetDefaults(NotificationDefaults.Vibrate);
                    //_builder.SetSmallIcon(Resource.Drawable.notifyIcon);
                    //_builder.SetAutoCancel(true);
                    //_builder.SetContentIntent(pendingIntent);

                    //_builder.SetStyle(_inboxStyle);
                }
                else
                {
                    var myNotificationChannel = PackageName + ".WaypointNotification";
                    var notificationMgr = NotificationManager.FromContext(BaseContext);
                    NotificationChannel channel;
                    channel = notificationMgr.GetNotificationChannel(myNotificationChannel);
                    if (channel == null)
                    {
                        channel = new NotificationChannel(myNotificationChannel, "Waypoint Reached", NotificationImportance.High);
                        channel.LockscreenVisibility = NotificationVisibility.Public;
                        channel.EnableVibration(true);
                        notificationMgr.CreateNotificationChannel(channel);
                    }
                    channel?.Dispose();

                    //_builder = new Notification.Builder(this);
                    _builder.SetChannelId(myNotificationChannel);
                    //_builder.SetContentTitle("Waypoints Visited");

                    //_builder.SetOngoing(false);
                    //_builder.SetSmallIcon(Resource.Drawable.notifyIcon);
                    //_builder.SetAutoCancel(true);
                    //_builder.SetContentIntent(pendingIntent);

                    //_builder.SetStyle(_inboxStyle);
                }
            }
            else
            {
                if (_currNotifications.Count() < 4)
                {
                    _inboxStyle.AddLine(inboxMsg);
                }
                else
                {
                    // might want to switch here to do the most recent three vs the earliest ones.
                    _inboxStyle.SetSummaryText(string.Format("+{0} more", _currNotifications.Count() - 3));
                }
            }

            var notification = _builder.Build();

            NotificationManager notificationManager =
                    GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.Notify(_notificationId, notification);
        }

        public override void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Token.ThrowIfCancellationRequested();

                _cts.Cancel();
            }
            Console.WriteLine("---->Stopping Geofence container instance: {0}", _currServiceCntr);

            StopForeground(true);
            _myTimer.Stop();
            GeoFenceService.Stop();
            base.OnDestroy();
        }
    }
}