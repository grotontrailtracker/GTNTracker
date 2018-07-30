using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using GTNTracker.Interfaces;
using GTNTracker.Services;
using GTNTracker.Types;
using Plugin.Compass;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class CurrentLocationVM : ViewModelBase
    {
        private string _trailName;
        private string _currentTrailId;
        private bool _isActiveTrailAvailable;
        private Position _currPosition;
        private double _currPositionAccuracy;
        private string _latString = "Unknown";
        private string _longString = "Unknown";
        private string _accuracyString = "Unknown";
        private bool _positionAvailable;
        private DateTime _lastUpdate;
        private string _closestRegionName;
        private string _distanceToRegion;
        private GeofenceRegion _nextRegion;
        private int _bearing;
        private string _directionToRegion;
        private bool _closestRegionAvailable;
        private ImageSource _imageSource;
        private bool _showImage;
        private string _currentHeading;
        private string _currentDirection;
        private double _currentHeadingDeg;
        private ICommand _refreshCommand;
        private bool _canRefreshGPS;
        private bool _waitingForGPSRefresh;
        private bool _overrideClosestRegion;

        private GeofenceRegion _currentRegion;
        private bool _isInCurrentRegion;
        private ImageSource _currentRegionImage;
        private string _currentRegionName;

        public CurrentLocationVM()
        {
            MessagingCenter.Subscribe<GeoPositionChanged, GeoPositionChangedArgs>(this, GeoPositionChanged.MessageString,
                                (sender, args) => { HandleGeoPositionChanged(args); });

            MessagingCenter.Subscribe<GeofenceCurrentPosition, GeofenceCurrentPositionArgs>(this, GeofenceCurrentPosition.MessageString,
                                (sender, args) => { HandleGeofenceCurrentPositionMsg(args); });

            MessagingCenter.Subscribe<GeofenceMonitoredRegions, GeofenceMonitoredRegionsArgs>(this, GeofenceMonitoredRegions.MessageString,
                                (sender, args) => { HandleGeofenceMonitoredRegionsMsg(args); });

            _lastUpdate = DateTime.Now;

            RefreshGPSCommand = new Command(HandleGPSRefresh, CanRefreshGPS);
        }

        public string TrailName
        {
            get => _trailName;
            set => SetProperty(ref _trailName, value);
        }

        public bool IsActiveTrailAvailable
        {
            get => _isActiveTrailAvailable;
            set => SetProperty(ref _isActiveTrailAvailable, value);
        }

        public string Longitude
        {
            get => _longString;
            set => SetProperty(ref _longString, value);
        }

        public string Latitude
        {
            get => _latString;
            set => SetProperty(ref _latString, value);
        }

        public string Accuracy
        {
            get => _accuracyString;
            set => SetProperty(ref _accuracyString, value);
        }

        public Position Location
        {
            get => _currPosition;
        }

        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set => SetProperty(ref _lastUpdate, value);
        }

        public bool PositionAvailable
        {
            get => _positionAvailable;
            set => SetProperty(ref _positionAvailable, value);
        }

        public string ClosestRegionName
        {
            get => _closestRegionName;
            set => SetProperty(ref _closestRegionName, value);
        }

        public string DistanceToClosestRegion
        {
            get => _distanceToRegion;
            set => SetProperty(ref _distanceToRegion, value);
        }

        public int Bearing
        {
            get => _bearing;
            set => SetProperty(ref _bearing, value);
        }

        public string DirectionToClosest
        {
            get => _directionToRegion;
            set => SetProperty(ref _directionToRegion, value);
        }

        public bool IsClosestRegionAvailable
        {
            get => _closestRegionAvailable;
            set => SetProperty(ref _closestRegionAvailable, value);
        }

        public string CurrentHeading
        {
            get => _currentHeading;
            set => SetProperty(ref _currentHeading, value);
        }

        public double CurrentHeadingDeg
        {
            get => _currentHeadingDeg;
            set => SetProperty(ref _currentHeadingDeg, value);
        }

        public string CurrentDirection
        {
            get => _currentDirection;
            set => SetProperty(ref _currentDirection, value);
        }

        public ImageSource TrailImage
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        public bool ShowImage
        {
            get => _showImage;
            set => SetProperty(ref _showImage, value);
        }

        public GeofenceRegion NextRegion
        {
            get => _nextRegion;
            set => _nextRegion = value;
        }
        public GeofenceRegion PriorRegion { get; set; }

        public bool WaitingForGPSRefresh
        {
            get => _waitingForGPSRefresh;
            set => SetProperty(ref _waitingForGPSRefresh, value);
        }

        public ICommand RefreshGPSCommand
        {
            get => _refreshCommand;
            set => SetProperty(ref _refreshCommand, value);
        }

        public GeofenceRegion CurrentRegion
        {
            get => _currentRegion;
            set => SetProperty(ref _currentRegion, value);
        }

        public string CurrentRegionName
        {
            get => _currentRegionName;
            set => SetProperty(ref _currentRegionName, value);
        }

        public ImageSource CurrentRegionImage
        {
            get => _currentRegionImage;
            set => SetProperty(ref _currentRegionImage, value);
        }

        public bool IsCurrentRegion
        {
            get => _isInCurrentRegion;
            set => SetProperty(ref _isInCurrentRegion, value);
        }

        public void UpdateCurrentPosition(bool resetData = false)
        {
            if (resetData)
            {
                ResetClosestRegionData();
            }

            var currTrailId = AppStateService.Instance.ActiveTrailId;
            if (!string.IsNullOrEmpty(currTrailId))
            {
                // is this a new trail from when this was last viewed?
                if (currTrailId != _currentTrailId)
                {
                    ResetClosestRegionData();
                    _currentTrailId = currTrailId;
                }

                var trailDef = TrailDefService.Instance.TrailDefinitions.FirstOrDefault(t => t.Identifier == currTrailId);
                if (trailDef != null)
                {
                    TrailName = trailDef.Name;
                    IsActiveTrailAvailable = true;
                }
            }
            else
            {
                TrailName = "No Trail Available";
                ResetClosestRegionData();
                IsActiveTrailAvailable = false;
            }

            if (CrossCompass.Current.IsSupported)
            {
                CrossCompass.Current.CompassChanged += Current_CompassChanged;
                CrossCompass.Current.Start(Plugin.Compass.Abstractions.SensorSpeed.UI);
            }
            else
            {
                CurrentHeading = "N/A";
            }

            _canRefreshGPS = false; // sending current location request
            WaitingForGPSRefresh = true;
            RefreshCommands();

            MessagingCenter.Send<RequestCurrentPosition>(new RequestCurrentPosition(), RequestCurrentPosition.MessageString);
        }

        public void Disconnect()
        {
            // do any cleanup's here like to not listen to compass changes.
            if (CrossCompass.Current.IsSupported)
            {
                CrossCompass.Current.Stop();
                CrossCompass.Current.CompassChanged -= Current_CompassChanged;
            }
        }

        public string FindDistance(string regionIdentifier)
        {
            string distanceString = string.Empty;
            var regionService = TrailDefService.Instance;
            var regionDef = regionService.GetRegionDefinition(AppStateService.Instance.ActiveTrailId).FirstOrDefault(r => r.Identifier == regionIdentifier);
            if (regionDef != null)
            {
                var distance = regionDef.Center.GetDistanceTo(Location);
                distanceString = ToFormattedString(distance);    
            }

            return distanceString;
        }

        private string ToFormattedString(Distance distance)
        {
            string distanceString = string.Empty;

            if (AppSettingsService.Instance.AppSettings.DisplayMeters)
            {
                if (distance.TotalMeters >= 1000)
                {
                    if (distance.TotalKilometers < 1.0)
                    {
                        distanceString = $"{distance.TotalKilometers:F2} KM";
                    }
                    else
                    {
                        distanceString = $"{distance.TotalKilometers:F1} KM";
                    }
                }
                else
                {
                    distanceString = $"{Math.Round(distance.TotalMeters)} meters";
                }
            }
            else
            {
                if (distance.TotalYards >= 1000)
                {
                    if (distance.TotalMiles < 1.0)
                    {
                        distanceString = $"{distance.TotalMiles:F2} miles";
                    }
                    else
                    {
                        distanceString = $"{distance.TotalMiles:F1} miles";
                    }
                }
                else
                {
                    distanceString = $"{Math.Round(distance.TotalYards)} yards";
                }
            }

            return distanceString;
        }

        private bool CanRefreshGPS()
        {
            return _canRefreshGPS;
        }

        private void HandleGPSRefresh()
        {
            _canRefreshGPS = false;
            WaitingForGPSRefresh = true;
            MessagingCenter.Send<RequestCurrentPosition>(new RequestCurrentPosition(), RequestCurrentPosition.MessageString);
            RefreshCommands();
        }

        private void RefreshCommands()
        {
            ((Command)RefreshGPSCommand).ChangeCanExecute();
        }

        public void ResetClosestRegionData()
        {
            PriorRegion = null;
            NextRegion = null;
            IsClosestRegionAvailable = false;
            ClosestRegionName = string.Empty;
            DistanceToClosestRegion = string.Empty;
            ShowImage = false;
            _overrideClosestRegion = false;
            TrailName = "Trail Not Set";
            _currentTrailId = string.Empty;
        }

        public void UpdateNextRegion(string regionId)
        {
            if (!string.IsNullOrEmpty(regionId))
            {
                var trailId = AppStateService.Instance.ActiveTrailId;
                var regions = TrailDefService.Instance.GetRegionDefinition(trailId);
                if (regions != null)
                {
                    var nextRegion = regions.FirstOrDefault(r => r.Identifier == regionId);
                    if (nextRegion != null)
                    {
                        var distance = nextRegion.Center.GetDistanceTo(Location);
                        PriorRegion = NextRegion;
                        UpdateUsingRegion(nextRegion, distance);
                        NextRegion = nextRegion;   // don't forget to update this since it's usually done when looking for the next closest
                        _overrideClosestRegion = true;
                    }
                }
            }
        }

        private void HandleGeoPositionChanged(GeoPositionChangedArgs e)
        {
            UpdatePositionProperties(e.Position, e.Accuracy);
            MessagingCenter.Send<RequestMonitoredRegions>(new RequestMonitoredRegions(), RequestMonitoredRegions.MessageString);
        }

        private void HandleGeofenceCurrentPositionMsg(GeofenceCurrentPositionArgs args)
        {
            _canRefreshGPS = true;
            WaitingForGPSRefresh = false;
            RefreshCommands();
            UpdatePositionProperties(args.Position, args.Accuracy);
            MessagingCenter.Send<RequestMonitoredRegions>(new RequestMonitoredRegions(), RequestMonitoredRegions.MessageString);
        }

        private void UpdatePositionProperties(Position pos, double accuracy)
        {
            _currPosition = pos;
            _currPositionAccuracy = accuracy;

            if (accuracy >= 0)
            {
                if (AppSettingsService.Instance.AppSettings.DisplayMeters)
                {
                    Accuracy = string.Format("{0:F1} meters", accuracy);
                }
                else
                {
                    Accuracy = string.Format("{0:F1} yards", accuracy * 1.09361);
                }
            }
            else
            {
                Accuracy = "Not Available";
            }

            var longStr = string.Format("{0:F6}°", pos.Longitude);
            Longitude = longStr.Contains(".") ? longStr.TrimEnd('0').TrimEnd('.') : longStr;

            var latStr = string.Format("{0:F6}°", pos.Latitude);
            Latitude = latStr.Contains(".") ? latStr.TrimEnd('0').TrimEnd('.') : latStr;

            PositionAvailable = true;
            LastUpdate = DateTime.Now;
        }

        private void Current_CompassChanged(object sender, Plugin.Compass.Abstractions.CompassChangedEventArgs e)
        {
            var heading = e.Heading;
            //string.Format("{0}°")
            CurrentHeading = string.Format("{0}°", (int)Math.Floor(heading));
            if (IsClosestRegionAvailable)
            {
                CurrentHeadingDeg = Bearing - heading;
                CurrentDirection = string.Empty;
            }
            else
            {
                //CurrentHeadingDeg = 360.0 - (heading + AppStateService.Instance.Declination); // more of a true heading, not magnetic
                CurrentHeadingDeg = (heading + AppStateService.Instance.Declination);
                var directionStr = GetDirection((int)Math.Floor(heading));
                CurrentDirection = directionStr;
            }
        }

        private void HandleGeofenceMonitoredRegionsMsg(GeofenceMonitoredRegionsArgs args)
        {
            var regionList = args.Regions;
            var _currPosition = Location;
            var currTrailId = AppStateService.Instance.ActiveTrailId;

            if (PositionAvailable && !string.IsNullOrEmpty(currTrailId))
            {
                // should do an accuracy check, if it's totally inaccurate, don't compute nearest way point.
                if (_currPositionAccuracy > 100.0)
                {
                    ClosestRegionName = "Current position inaccurate";
                    DistanceToClosestRegion = string.Empty;
                    _nextRegion = null;
                    IsClosestRegionAvailable = false;
                    ShowImage = false;
                    return;
                }

                if (regionList.Any())
                {
                    GeofenceRegion closestRegion = null;
                    PriorRegion = _nextRegion;
                    Distance closestDistance = Distance.FromMeters(0);
                    IsCurrentRegion = false;
                    CurrentRegion = null;
                    CurrentRegionName = string.Empty;

                    if (!_overrideClosestRegion)
                    {
                        foreach (var region in regionList)
                        {
                            var inRegion = region.IsPositionInside(_currPosition);
                            var inRegionStr = inRegion ? "In Region" : "Out of Region";
                            var distance = region.Center.GetDistanceTo(_currPosition);
                            if (closestRegion == null || distance.TotalMeters < closestDistance.TotalMeters)
                            {
                                // need to check if it's been visited!!!!, don't give distance to already visited regions!
                                var priorVisit = TrailVisitService.Instance.GetVisits(currTrailId).FirstOrDefault(v => v.RegionIdentifier == region.Identifier);
                                if (priorVisit == null)
                                {
                                    closestRegion = region;
                                    closestDistance = distance;
                                    _nextRegion = region;
                                }
                            }

                            if (inRegion)
                            {
                                // we now want to display the current region so setup binding variables
                                IsCurrentRegion = true;
                                CurrentRegion = region;
                                CurrentRegionName = region.Name;
                                ProcessCurrentRegionImage(region);
                            }

                            var msg = string.Format("region: {0}, lat: {1}, lng: {2}, loc?: {3}, dist: {4:F2} meters",
                                region.Identifier, region.Center.Latitude, region.Center.Longitude, inRegionStr, distance.TotalMeters);
                        }
                    }
                    else
                    {
                        closestRegion = NextRegion;
                        closestDistance = NextRegion.Center.GetDistanceTo(_currPosition);
                    }

                    if (closestRegion != null)
                    {
                        UpdateUsingRegion(closestRegion, closestDistance);
                    }
                    else
                    {
                        ClosestRegionName = "All Way Points Visited";
                        DistanceToClosestRegion = string.Empty;
                        IsClosestRegionAvailable = false;
                        ShowImage = false;
                    }
                }
            }
            else
            {
                ClosestRegionName = "No Position Available";
                _nextRegion = null;
                IsClosestRegionAvailable = false;
                ShowImage = false;
            }
        }

        private void UpdateUsingRegion(GeofenceRegion region, Distance distance)
        {
            ClosestRegionName = region.Name;
            DistanceToClosestRegion = ToFormattedString(distance);

            try
            {
                var bearing = GetBearing(Location.Latitude, Location.Longitude,
                                region.Center.Latitude, region.Center.Longitude);
                if (bearing < 0)
                {
                    bearing += 360;
                }
                var directionStr = GetDirection((int)Math.Floor(bearing));

                Bearing = (int)bearing;
                DirectionToClosest = directionStr;

                // don't update the image if we're already showing the one for this region
                if (PriorRegion == null || (PriorRegion != null && PriorRegion.Identifier != region.Identifier))
                {
                    ProcessClosestRegionImage(region);
                }

                IsClosestRegionAvailable = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: {0}", e.Message);
            }
        }

        double DEG_PER_RAD = (180.0 / Math.PI);
        // Return Bearing (degrees)
        private double GetBearing(double lat1, double lon1, double lat2, double lon2)
        {
            var dLon = lon2 - lon1;
            var y = Math.Sin(dLon) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            return DEG_PER_RAD * Math.Atan2(y, x);
        }

        private string GetDirection(int degrees)
        {
            string[] points = new string[] { "N", "NNE", "NE", "ENE", "E", "ESE", 
                                             "SE",  "SSE", "S", "SSW",  "SW",  "WSW",  "W",  "WNW", 
                                            "NW",  "NNW", };
            double calc = degrees / 360.0 * 16.0;
            int point = (int) Math.Floor(calc + 0.5);
            return points[point % 16];
        }

        private void ProcessClosestRegionImage(GeofenceRegion region)
        {
            var imgNameToUse = !string.IsNullOrEmpty(region.ImageName) ? region.ImageName : "GTNTracker.Images.UnderConstruct.jpg";
            if (string.IsNullOrEmpty(region.ImageName))
            {
                region.IsImageNameURI = false;  // enforce this especially if we didn't have a name
                ShowImage = false;
            }
            else
            {
                ShowImage = true;
            }

            if (region.IsImageNameURI)
            {
                TrailImage = ImageSource.FromUri(new System.Uri(region.ImageName));
            }
            else
            {
                TrailImage = ImageSource.FromResource(imgNameToUse);
            }
        }
        private void ProcessCurrentRegionImage(GeofenceRegion region)
        {
            var imgNameToUse = !string.IsNullOrEmpty(region.ImageName) ? region.ImageName : "GTNTracker.Images.UnderConstruct.jpg";

            if (region.IsImageNameURI)
            {
                CurrentRegionImage = ImageSource.FromUri(new System.Uri(region.ImageName));
            }
            else
            {
                CurrentRegionImage = ImageSource.FromResource(imgNameToUse);
            }
        }
    }
}
