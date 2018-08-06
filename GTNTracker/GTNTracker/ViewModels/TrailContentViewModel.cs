using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GTNTracker.EventArguments;
using GTNTracker.Interfaces;
using GTNTracker.Models;
using GTNTracker.Services;
using GTNTracker.Types;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class TrailContentViewModel : ViewModelBase
    {
        private string _trailName;
        private string _trailDescription;
        private string _trailIdentifier;
        private bool _isTrailListComplete;
        private bool _isStarted;
        private int _numberOfDestinations;
        private int _numberEntered;
        private DateTime? _dateCompleted;
        private bool _allowStart = true;
        private bool _allowStop;
        private bool _enableStartStopTracking;
        private bool _isViewable = true;

        private IEnumerable<GeofenceRegion> _trailGeoRegions;
        private List<string> _destinationsEntered = new List<string>();
        private ObservableCollection<TrailRegionVM> _trailRegionList = new ObservableCollection<TrailRegionVM>();

        public TrailContentViewModel()
        {
            var visitService = TrailVisitService.Instance;
            visitService.VisitsUpdated += VisitService_visitsUpdated;

            NotificationService.Instance.Tracking += HandleTrackingChange;

            MessagingCenter.Subscribe<GeofenceUpdated, GeofenceUpdatedArgs>(this, GeofenceUpdated.MessageString,
                                (sender, args) => { HandleGeofenceUpdatedMessage(args); });
            IsViewable = true;
        }

        public bool IsViewable
        {
            get => _isViewable;
            set => SetProperty(ref _isViewable, value);
        }

        private void HandleTrackingChange(object sender, TrackingEventArgs e)
        {
            CheckIsViewable(e.MonitoringState);
        }

        private void CheckIsViewable(bool monitoring)
        {
            IsViewable = IsStarted || (!monitoring && !IsStarted);
        }

        public void Start()
        {
            if (!IsStarted)
            {
                //if (IsTrailListComplete)
                //{
                //    return;
                //}

                //IsTrailListComplete = false;

                MessagingCenter.Send(new StartGeofencing(), StartGeofencing.MessageString);
                MessagingCenter.Send(new RegisterMonitoringRegions(), RegisterMonitoringRegions.MessageString,
                                    new RegisterMonitoringRegionsArgs(_trailGeoRegions.ToList()));
              
                NotificationService.Instance.NotifyTracking(TrailId, true);
                AppStateService.Instance.ActiveTrailId = TrailId;
                ViewModelLocator.Instance.ActiveTrailContentVM = this;
                IsStarted = true;
                CheckIsViewable(true);
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                MessagingCenter.Send(new StopMonitoringRegions(), StopMonitoringRegions.MessageString,
                                    new StopMonitoringRegionsArgs(_trailGeoRegions.ToList()));
                MessagingCenter.Send(new StopGeofencing(), StopGeofencing.MessageString, new StopGeofencingArgs(false));
                IsStarted = false;
                NotificationService.Instance.NotifyTracking(TrailId, false);
                AppStateService.Instance.ActiveTrailId = string.Empty;
                ViewModelLocator.Instance.ActiveTrailContentVM = null;
            }
        }


        public void Initialize(string trailId, string name, IEnumerable<GeofenceRegion> regions, IEnumerable<TrailRegionVisit> visited)
        {
            _trailIdentifier = trailId;
            TrailName = name;
            _trailGeoRegions = regions;
            NumberDestinations = _trailGeoRegions.Count();

            foreach (var region in _trailGeoRegions)
            {
                region.TrailName = TrailName;   // set for the notifications!

                TrailRegionVM tr;
                var prior = visited.FirstOrDefault(r => r.RegionIdentifier == region.Identifier);
 
                tr = new TrailRegionVM()
                {
                    TrailIdentifier = _trailIdentifier,
                    RegionIdentifier = region.Identifier,
                    RegionName = region.Name,
                    Entered = false,
                    DateCompleted = null
                };

                // process associated region def image
                var imgNameToUse = !string.IsNullOrEmpty(region.ImageName) ? region.ImageName : "GTNTracker.Images.UnderConstruct.jpg";
                if (string.IsNullOrEmpty(region.ImageName))
                {
                    region.IsImageNameURI = false;  // enforce this especially if we didn't have a name
                    tr.ShowImage = false;
                }
                else
                {
                    tr.ShowImage = true;
                }

                if (region.IsImageNameURI)
                {
                    //tr.TrailImage = ImageSource.FromUri(new System.Uri(region.ImageName));
                    tr.TrailImage = new UriImageSource { CachingEnabled = false, Uri = new Uri(region.ImageName) };//ImageSource.FromUri(new System.Uri(region.ImageName));
                }
                else
                {
                    tr.TrailImage = ImageSource.FromResource(imgNameToUse);
                }

                if (prior != null)
                {
                    tr.DateCompleted = prior.Completed;
                    tr.Entered = true;
                }

                _trailRegionList.Add(tr);
            }

            RegionList = _trailRegionList;
            NumberEntered = visited.Count();
            if (NumberEntered == RegionList.Count)
            {
                IsTrailListComplete = true;
                DateCompleted = visited.Max(r => r.Completed);
            }
        }

        // controls whether we should allow the start/stop trail tracking button to be used.
        public bool EnableStartStopTracking
        {
            get => _enableStartStopTracking;
            set => SetProperty(ref _enableStartStopTracking, value);
        }

        public bool AllowStart
        {
            get => _allowStart;
            set => SetProperty(ref _allowStart, value);
        }

        public bool AllowStop
        {
            get => _allowStop; 
            set => SetProperty(ref _allowStop, value);
        }

        public bool IsStarted
        {
            get => _isStarted;
            set
            {
                SetProperty(ref _isStarted, value);
            }
        }

        public string TrailName
        {
            get => _trailName;
            set => SetProperty(ref _trailName, value);
        }

        public string TrailId
        {
            get => _trailIdentifier; 
            set => SetProperty(ref _trailIdentifier, value);
        }

        public string TrailDescription
        {
            get => _trailDescription;
            set => SetProperty(ref _trailDescription, value);
        }
        public ObservableCollection<TrailRegionVM> RegionList
        {
            get => _trailRegionList;
            set => SetProperty(ref _trailRegionList, value);
        }

        public bool IsTrailListComplete
        {
            get => _isTrailListComplete;
            set => SetProperty(ref _isTrailListComplete, value);
        }

        public DateTime? DateCompleted
        {
            get => _dateCompleted;
            set => SetProperty(ref _dateCompleted, value);
        }

        public int NumberDestinations
        {
            get => _numberOfDestinations; 
            set => SetProperty(ref _numberOfDestinations, value);
        }

        public int NumberEntered
        {
            get => _numberEntered; 
            set => SetProperty(ref _numberEntered, value); 
        }

        private void VisitService_visitsUpdated(object sender, VisitsUpdatedArgs e)
        {
            var visitService = TrailVisitService.Instance;
            var visits = visitService.GetVisits(_trailIdentifier);
            if (!visits.Any())
            {
                foreach (var trailRef in RegionList)
                {
                    trailRef.DateCompleted = null;
                    trailRef.Entered = false;
                }
                IsTrailListComplete = false;
                DateCompleted = null;
            }
            else
            {
                // go and update all the visits.
                foreach (var trailRef in RegionList)
                {
                    var visitedRef = visits.FirstOrDefault(v => v.RegionIdentifier == trailRef.RegionIdentifier);
                    if (visitedRef == null)
                    {
                        if (trailRef.Entered)
                        {
                            trailRef.Entered = false;
                            trailRef.DateCompleted = null;
                        }
                    }
                    else
                    {
                        if (!trailRef.Entered)
                        {
                            trailRef.Entered = true;
                            trailRef.DateCompleted = visitedRef.Completed;
                        }
                    }
                }
            }

            NumberEntered = RegionList.Count(c => c.Entered);
            if (NumberEntered == NumberDestinations)
            {
                IsTrailListComplete = true;
                DateCompleted = visits.Max(v => v.Completed);
            }
        }

        private void HandleGeofenceUpdatedMessage(GeofenceUpdatedArgs args)
        {
            // need to queue this up since we might not have completed the Start activation sequence in the case
            // of a trail with a single waypoint that we are currently at.
            Device.BeginInvokeOnMainThread(() =>
            {
                var status = args.Status;
                var region = args.Region;
                var msg = string.Format("status: {0}, region: {1}", status, region.Identifier);
                if (status == GeofenceStatus.Entered)
                {
                    var alertMsg = string.Format("Region: {0}", region.Identifier);


                    var trailRegion = _trailRegionList.FirstOrDefault(r => r.RegionIdentifier == region.Identifier);
                    if (trailRegion != null)
                    {
                        if (!trailRegion.Entered)
                        {
                            trailRegion.Entered = true;
                            trailRegion.DateCompleted = DateTime.Now;
                            if (!_trailRegionList.Any(r => !r.Entered))
                            {
                                DateCompleted = DateTime.Now;
                                IsTrailListComplete = true;
                            }

                            if (AppStateService.Instance.IsAppAwake)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    NotificationService.Instance.NotifyRegion(region, false, "Waypoint Reached");
                                });
                            }

                            NumberEntered = _trailRegionList.Count(c => c.Entered);

                            if (IsTrailListComplete)
                            {
                                var trailDef = TrailDefService.Instance.TrailDefinitions.FirstOrDefault(t => t.Identifier == region.TrailIdentifier);
                                if (trailDef != null && AppStateService.Instance.IsAppAwake)
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        NotificationService.Instance.NotifyTrailComplete(trailDef.Name);
                                    });
                                }

                                //Stop();
                                //EnableStartStopTracking = false;
                            }
                        }
                    }
                }
            });
        }
    }
}
