using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GTNTracker.EventArguments;
using GTNTracker.Interfaces;
using GTNTracker.Models;
using GTNTracker.Types;
using Plugin.Geolocator;
using Xamarin.Forms;

namespace GTNTracker.Services
{
    public class GeoFenceService
    {
        private static GeoFenceService _instance;

        private IGeofenceManager _geoMgr;

        public static void Initialize()
        {
            if (_instance == null)
            {
                _instance = new GeoFenceService();
            }
        }

        public static void Start()
        {
            if (_instance == null)
            {
                Debug.WriteLine("----> Creating new instance of GeoMgr");
                _instance = new GeoFenceService();
            }
            else
            {
                Debug.WriteLine("----> Using old instance of GeoMgr");
            }

            Debug.WriteLine("====> Starting Geo Manager");
            _instance._geoMgr.Start();
        }

        public static void Stop()
        {
            if (_instance != null)
            {
                Debug.WriteLine("====> Stopping Geo Manager");
                _instance._geoMgr.Stop();
            }
        }

        public static bool IsRunning()
        {
            if (_instance != null)
            {
                return _instance._geoMgr.IsServiceRunning;
            }

            return false;
        }

        public static string GetCurrentMonitoredTrail()
        {
            string trailId = string.Empty;
            if (_instance != null)
            {
                trailId = _instance.CurrentMonitoredTrail();
            }
            return trailId;
        }

        private GeoFenceService()
        {
            _geoMgr = new CoreGeofenceMgr
            {
                DesiredAccuracy = Distance.FromMeters(50)
            };

            _geoMgr.RegionStatusChanged += HandleRegionStatusChanged;
            _geoMgr.PositionChanged += HandlePositionChanged;

            MessagingCenter.Subscribe<RegisterMonitoringRegions, RegisterMonitoringRegionsArgs>(this, RegisterMonitoringRegions.MessageString,
                                (sender, args) => { HandleRegisterMonitoringRegionsMsg(args); });

            MessagingCenter.Subscribe<RequestMonitoredRegions>(this, RequestMonitoredRegions.MessageString,
                                (sender) => { HandleRequestMonitoredRegions(); });

            MessagingCenter.Subscribe<StopMonitoringRegions, StopMonitoringRegionsArgs>(this, StopMonitoringRegions.MessageString,
                                (sender, args) => { HandleStopMonitoringRegions(args); });

            MessagingCenter.Subscribe<RequestCurrentPosition>(this, RequestCurrentPosition.MessageString,
                                (sender) => { HandleRequestCurrentPositionMsg(); });

            MessagingCenter.Subscribe<RequestCurrentPositionString>(this, RequestCurrentPositionString.MessageString,
                                (sender) => { HandleRequestCurrentPositionStringMsg(); });
        }

        private string CurrentMonitoredTrail()
        {
            string trailId = string.Empty;
            var firstRegion = _geoMgr.MonitoredRegions.FirstOrDefault();
            if (firstRegion != null)
            {
                trailId = firstRegion.TrailIdentifier;
            }

            return trailId;
        }

        private void HandlePositionChanged(object sender, PositionChangedEventArgs e)
        {
            MessagingCenter.Send(new GeoPositionChanged(), GeoPositionChanged.MessageString,
                                new GeoPositionChangedArgs(e.Position, e.Accuracy));
        }

        private void HandleRegisterMonitoringRegionsMsg(RegisterMonitoringRegionsArgs args)
        {
            SetMonitorRegions(args.Regions);
        }

        private void SetMonitorRegions(IEnumerable<GeofenceRegion> regions)
        {
            Debug.WriteLine("--->Setting Monitor Regions");
            foreach (var region in regions)
            {
                _geoMgr.StartMonitoring(region);
            }
        }

        private void HandleStopMonitoringRegions(StopMonitoringRegionsArgs args)
        {
            Debug.WriteLine("--->Stopping Monitor Regions");
            var regions = args.Regions;
            foreach (var region in regions)
            {
                StopMonitoringRegion(region);
            }
        }

        private void StopMonitoringRegion(GeofenceRegion region)
        {
            _geoMgr.StopMonitoring(region);
        }

        private void HandleRequestMonitoredRegions()
        {
            var regionList = _geoMgr.MonitoredRegions.GroupBy(reg => reg.Identifier).Select(grp => grp.First()).ToList();
            MessagingCenter.Send(new GeofenceMonitoredRegions(), GeofenceMonitoredRegions.MessageString,
                                new GeofenceMonitoredRegionsArgs(regionList));
        }

        private async void HandleRequestCurrentPositionStringMsg()
        {
            var rtnString = string.Empty;
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50.0;
            var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));
            if (position == null)
            {
                rtnString = "Location not available";
            }
            else
            {
                rtnString = string.Format("Time: {0} \nLat: {1} \nLong: {2} \nAccuracy: {3} \n ",
                position.Timestamp.ToLocalTime(), position.Latitude, position.Longitude, position.Accuracy);
            }

            MessagingCenter.Send(new GeofenceCurrentPositionString(), GeofenceCurrentPositionString.MessageString,
                                rtnString);           
        }
 
        private async void HandleRequestCurrentPositionMsg()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50.0;
            var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));

            var result = new Position(position.Latitude, position.Longitude);

            MessagingCenter.Send(new GeofenceCurrentPosition(), GeofenceCurrentPosition.MessageString,
                                new GeofenceCurrentPositionArgs(result, position.Accuracy));
        }

        private void StopGeoMgr()
        {
            Debug.WriteLine("===> Stopping GeofenceService");
            _geoMgr.StopAllMonitoring();
        }

        private void HandleRegionStatusChanged(object sender, GeofenceStatusChangedEventArgs e)
        {
            var state = e.Status;
            var region = e.Region;
            var position = e.Position;
            Debug.WriteLine("status changed: " + state.ToString() + ", " + region.Identifier);

            if (state == GeofenceStatus.Entered)
            {
                var alreadyVisited = TrailVisitService.Instance.GetVisits(region.TrailIdentifier).FirstOrDefault(v => v.RegionIdentifier == region.Identifier);
                if (alreadyVisited == null)
                {
                    // save away the hard work as well.
                    var visit = new TrailRegionVisit()
                    {
                        TrailIdentifer = region.TrailIdentifier,
                        RegionIdentifier = region.Identifier,
                        Completed = DateTime.Now
                    };

                    TrailVisitService.Instance.AddTrailVisit(visit);

                    // issue request to service to handle processing notification
                    MessagingCenter.Send<GeofenceVisited, GeofenceVisitedArgs>(new GeofenceVisited(), GeofenceVisited.MessageString,
                                new GeofenceVisitedArgs(region));
                }
            }

            MessagingCenter.Send<GeofenceUpdated, GeofenceUpdatedArgs>(new GeofenceUpdated(), GeofenceUpdated.MessageString,
                                new GeofenceUpdatedArgs(state, region));
        }
    }
}