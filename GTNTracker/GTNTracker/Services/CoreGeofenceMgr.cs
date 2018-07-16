using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GTNTracker.EventArguments;
using GTNTracker.Interfaces;
using GTNTracker.Types;

namespace GTNTracker.Services
{
    public class CoreGeofenceMgr : IGeofenceManager
    {
        readonly Plugin.Geolocator.Abstractions.IGeolocator geolocator;
        //readonly GeofenceSettings settings;
        readonly IDictionary<string, GeofenceState> states;
        Position current;
        DateTime? lastFix;
        private bool _isServiceRunning;
        private List<GeofenceRegion> _monitoredRegions = new List<GeofenceRegion>();

        public CoreGeofenceMgr(int accuracy = 50)
        {
            geolocator = Plugin.Geolocator.CrossGeolocator.Current;
            //this.settings = settings ?? GeofenceSettings.GetInstance();
            DesiredAccuracy = Distance.FromMeters(accuracy);
            MinimumAccuracy = accuracy;
            GeolocateTimeInterval = 5000;   // update every 5 seconds
            //this.geolocator.AllowsBackgroundUpdates = true;

            this.states = new Dictionary<string, GeofenceState>();
            //if (this._MonitoredRegions.Count > 0)
            //{
            //    this.TryStartGeolocator();
            //}
//            var result = StartService();
//            _isServiceRunning = result.Result;
//            Debug.WriteLine("GeoLocation Service Started");
        }


        public async Task<GeofenceStatus> RequestState(GeofenceRegion region, CancellationToken? cancelToken = null)
        {
            // TODO: what if my position is old?
            if (this.states.ContainsKey(region.Identifier))
                return this.states[region.Identifier].Status;

            if (this.current != null)
                return region.IsPositionInside(this.current) ? GeofenceStatus.Entered : GeofenceStatus.Exited;

            var tcs = new TaskCompletionSource<GeofenceStatus>();
            cancelToken?.Register(() => tcs.TrySetCanceled());

            var handler = new EventHandler<GeofenceStatusChangedEventArgs>((sender, args) =>
            {
                if (args.Region.Identifier.Equals(region.Identifier))
                    tcs.TrySetResult(args.Status);
            });

            try
            {
                // this is not ideal since it fires the public event, though they were likely monitoring it anyhow
                this.RegionStatusChanged += handler;
                this.StartMonitoring(region);
                return await tcs.Task;
            }
            finally
            {
                this.StopMonitoring(region);
                this.RegionStatusChanged -= handler;
            }
        }

        public event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;
        public event EventHandler<PositionChangedEventArgs> PositionChanged;
        public event EventHandler<PositionInaccurateEventArgs> InaccuratePosition;
        public double MinimumAccuracy { get; set; }
        public int GeolocateTimeInterval { get; set; }

        public bool IsServiceRunning { get { return _isServiceRunning; } }

        public Distance DesiredAccuracy
        {
            get
            {
                return Distance.FromMeters(this.geolocator.DesiredAccuracy);
            }
            set
            {
                geolocator.DesiredAccuracy = value.TotalMeters;
            }
        }

        /// <summary>
        /// Pass out only a copy of the monitored regions.
        /// </summary>
        public IEnumerable<GeofenceRegion> MonitoredRegions => _monitoredRegions.ToList();

        public bool Start()
        {
            if (_isServiceRunning)
            {
                Debug.WriteLine("+++++ GeoMgr is already running");
                return _isServiceRunning;
            }
            var result = StartService();
            _isServiceRunning = result.Result;
            Debug.WriteLine("+++++ GeoMgr Service Started");

            return _isServiceRunning;
        }

        public bool Stop()
        {
            try
            {
                StopGeolocator();
                _isServiceRunning = false;
                Debug.WriteLine("+++++ GeoMgr service is stopped");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> StartService()
        {
            var x = await this.TryStartGeolocator();
            return x;
        }

        public GeofenceStatus StartMonitoring(GeofenceRegion monitorRegion)
        {
            var region = monitorRegion;
            var state = new GeofenceState(region);
            var status = GeofenceStatus.Unknown;

            if (states.ContainsKey(region.Identifier))
            {
                region = _monitoredRegions.FirstOrDefault(reg => reg.Identifier == region.Identifier);
            }
            else
            {
                this.states.Add(region.Identifier, state);
                this._monitoredRegions.Add(region);
            }

            if (this.current != null)
            {
                // TODO: what if my position is old?
                var inside = region.IsPositionInside(this.current);
                //state.Status = inside ? GeofenceStatus.Entered : GeofenceStatus.Exited;
                //status = state.Status;
                UpdateFences(current.Latitude, current.Longitude);
                status = state.Status;
            }

            //this.TryStartGeolocator();

            return status;
        }


        public void StopMonitoring(GeofenceRegion region)
        {
            this.states.Remove(region.Identifier);
            var removeRegion = _monitoredRegions.FirstOrDefault(r => r.Identifier == region.Identifier);
            if (removeRegion != null)
            {
                var status = _monitoredRegions.RemoveAll(t => t.Identifier == region.Identifier);
                // big bug here, won't remove the element!!!!
                //var status = _monitoredRegions.Remove(removeRegion);
            }
        }


        public void StopAllMonitoring()
        {
            //_monitoredRegions.Clear();
            _monitoredRegions = new List<GeofenceRegion>();
            this.states.Clear();
            //await this.geolocator.StopListeningAsync();
            //_isServiceRunning = false;
        }

        private async Task<Plugin.Geolocator.Abstractions.Position> GetInitialPosition()
        {
            TimeSpan waitTime = TimeSpan.FromMilliseconds(10000);
            var position = await geolocator.GetPositionAsync(waitTime);
            return position;
        }


        protected async Task<bool> TryStartGeolocator()
        {
            if (geolocator.IsListening)
                return false;   // already running, don't need to try to start listener

            geolocator.PositionChanged += OnPositionChanged;

            var success = await Plugin.Geolocator.CrossGeolocator.Current.StartListeningAsync(
                                                                    TimeSpan.FromMilliseconds(GeolocateTimeInterval), 
                                                                    5,
                                                                    true, 
                                                                    new Plugin.Geolocator.Abstractions.ListenerSettings
                                                                    {
                                                                        ActivityType = Plugin.Geolocator.Abstractions.ActivityType.Fitness,
                                                                        AllowBackgroundUpdates = true,
                                                                        DeferLocationUpdates = false,
                                                                        DeferralDistanceMeters = 1,
                                                                        DeferralTime = TimeSpan.FromSeconds(1),
                                                                        ListenForSignificantChanges = false,
                                                                        PauseLocationUpdatesAutomatically = false
                                                                    });
            return success;
        }


        protected void StopGeolocator()
        {
            geolocator.StopListeningAsync();
            geolocator.PositionChanged -= OnPositionChanged;
        }


        protected void OnPositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs args)
        {
            if (args.Position.Accuracy > (MinimumAccuracy * 2))
            {
                // the gps position change was really useless since it was outside the accuracy we need
                InaccuratePosition?.Invoke(this,
                    new PositionInaccurateEventArgs(new Position(args.Position.Latitude, args.Position.Longitude),
                                                    args.Position.Accuracy));
                return;
            }

            this.lastFix = DateTime.Now.ToLocalTime();
            this.current = new Position(args.Position.Latitude, args.Position.Longitude);
            this.UpdateFences(args.Position.Latitude, args.Position.Longitude);
            var posArgs = new PositionChangedEventArgs(new Position(args.Position.Latitude, args.Position.Longitude), args.Position.Accuracy);
            PositionChanged?.Invoke(this, posArgs);
        }


        protected void UpdateFences(double lat, double lng)
        {
            var loc = new Position(lat, lng);

            foreach (var fence in this.states.Values)
            {
                var newState = fence.Region.IsPositionInside(loc);
                var status = newState ? GeofenceStatus.Entered : GeofenceStatus.Exited;

                if (fence.Status == GeofenceStatus.Unknown)
                {
                    // status being set for first time as we didn't have current coordinates
                    fence.Status = status;
                    this.RegionStatusChanged?.Invoke(this, new GeofenceStatusChangedEventArgs(fence.Region, status, new Position(lat, lng)));   // initial state!!!
                }
                else if (fence.Status != status)
                {
                    fence.Status = status;
                    this.RegionStatusChanged?.Invoke(this, new GeofenceStatusChangedEventArgs(fence.Region, status, new Position(lat, lng)));
                }
            }
        }
    }
}
