using System;
using System.Collections.Generic;
using GTNTracker.EventArguments;
using GTNTracker.Types;

namespace GTNTracker.Services
{
    public class NotificationService
    {
        private static NotificationService _instance;
        private List<string> _currentTrackingList = new List<string>();

        public static NotificationService Instance => _instance ?? (_instance = new NotificationService());

        public EventHandler<GeofenceRegionUpdatedArgs> RegionUpdated;
        public EventHandler<TrailCompleteEventArgs> TrailComplete;
        public EventHandler<NavigateEventArgs> NavigateToPage;
        public EventHandler<WebMapEventArgs> WebMap;
        public EventHandler<TrackingEventArgs> Tracking;
        public EventHandler<ResumeTrailMonitoringArgs> ResumeMonitoring;
        public EventHandler NavigatePrior;
        public EventHandler<AllowCaptureModeArgs> AllowWaypointCaptureMode;
        public EventHandler<DevModeGeoFenceStateArgs> DevModeGeoServiceState;

        public NotificationService()
        {
        }

        public void NotifyRegion(GeofenceRegion region, bool useDialog = false, bool alert = true)
        {
            RegionUpdated?.Invoke(this, new GeofenceRegionUpdatedArgs(region, useDialog, alert));
        }

        public void NotifyTrailComplete(string trailName, bool alert = true)
        {
            TrailComplete?.Invoke(this, new TrailCompleteEventArgs(trailName, alert));
        }

        public void NotifyNavigateToPage(int pageId, Type pageType)
        {
            NavigateToPage?.Invoke(this, new NavigateEventArgs(pageId, pageType));
        }

        public void NotifyWebMap(double lat, double lng)
        {
            WebMap?.Invoke(this, new WebMapEventArgs(lat, lng));
        }

        public void NotifyTracking(string trailId, bool monitoring)
        {
            if (monitoring)
            {
                if (!_currentTrackingList.Contains(trailId))
                {
                    _currentTrackingList.Add(trailId);
                }
            }
            else
            {
                _currentTrackingList.Remove(trailId);
            }
            Tracking?.Invoke(this, new TrackingEventArgs(trailId, monitoring));
        }

        public void NotifyResumeTrailMonitoring(string trailId)
        {
            ResumeMonitoring?.Invoke(this, new ResumeTrailMonitoringArgs(trailId));
        }

        public void NotifyNavigatePriorPage()
        {
            NavigatePrior?.Invoke(this, new EventArgs());
        }

        public void NotifyAllowWaypointCapture(bool enabled)
        {
            AllowWaypointCaptureMode?.Invoke(this, new AllowCaptureModeArgs(enabled));
        }

        public void NotifyDevModeGeoServiceState(bool state)
        {
            DevModeGeoServiceState?.Invoke(this, new DevModeGeoFenceStateArgs(state));
        }

        public void ClearAllTracking()
        {
            foreach (var trail in _currentTrackingList)
            {
                Tracking?.Invoke(this, new TrackingEventArgs(trail, false));
            }
            _currentTrackingList.Clear();
        }

        public IEnumerable<string> CurrentTrackingList => _currentTrackingList;

    }
}
