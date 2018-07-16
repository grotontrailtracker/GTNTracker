using System;
using GTNTracker.Interfaces;
using GTNTracker.Services;
using Xamarin.Forms;
using Plugin.Notifications;
using System.Collections.Generic;

namespace GTNTracker.iOS
{
    public class GeoContainer
    {
        private bool _isRunning = false;
        private int _idCnt = 8000;
        private List<int> _outstandingNotifications = new List<int>();

        public GeoContainer()
        {

            MessagingCenter.Subscribe<StartGeofencing>(this, StartGeofencing.MessageString,
                    (sender) => { HandleStart(); });

            MessagingCenter.Subscribe<StopGeofencing, StopGeofencingArgs>(this, StopGeofencing.MessageString,
                    (sender, args) => { HandleStop(args); });

            MessagingCenter.Subscribe<GeofenceVisited, GeofenceVisitedArgs>(this, GeofenceVisited.MessageString,
                    (sender, args) => { HandleGeofenceVisitedMessage(args); });
        }

        private void HandleStart()
        {
            Console.WriteLine("-----> Starting the Geofence Service!!!!");
            GeoFenceService.Start();
            var notification = new Notification()
            {
                Title = "Trail Tracker Service",
                Message = "GPS Service is running",
                Vibrate = false,
                Id = 999999,
                When = TimeSpan.FromSeconds(1)
            };
            CrossNotifications.Current.Send(notification);
            _isRunning = true;
        }

        private void HandleStop(StopGeofencingArgs args)
        {
            Console.WriteLine("-----> Stopping the Geofence Service!!!!");
            if (_isRunning)
            {
                GeoFenceService.Stop();
                CrossNotifications.Current.Cancel(999999);

                foreach (var id in _outstandingNotifications)
                {
                    CrossNotifications.Current.Cancel(id);
                }
                _isRunning = false;
            }
        }

        private void HandleGeofenceVisitedMessage(GeofenceVisitedArgs args)
        {
            var region = args.Region;
            var title = string.Format("{0} - Waypoint Reached", string.IsNullOrEmpty(region.TrailName) ? "Trail" : region.TrailName);
            var msg = "Arrived at " + region.Name;
            var notification = new Notification()
            {
                Title = title,
                Message = msg,
                Id = _idCnt++,
                Vibrate = true,
                When = TimeSpan.FromMilliseconds(200)
            };
            CrossNotifications.Current.Send(notification);
            _outstandingNotifications.Add(notification.Id.Value);
            CrossNotifications.Current.Vibrate(300);

            Console.WriteLine($"-----> Waypoint visited message, id: {region.Identifier}");
        }
    }
}
