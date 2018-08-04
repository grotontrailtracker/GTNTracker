using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTNTracker.Types;
using Xamarin.Forms;

namespace GTNTracker.EventArguments
{
    public class TrailCompleteEventArgs : EventArgs
    {
        public TrailCompleteEventArgs(string trailName, bool alert)
        {
            TrailName = trailName;
            Alert = alert;
        }
        public string TrailName { get; set; }
        public bool Alert { get; set; }
    }

    public class GeofenceRegionUpdatedArgs : EventArgs
    {
        public GeofenceRegionUpdatedArgs(GeofenceRegion region, bool useDialog, bool alert)
        {
            Region = region;
            Alert = alert;
            UseDialog = useDialog;
        }
        public GeofenceRegion Region { get; set; }
        public bool Alert { get; set; }

        public bool UseDialog { get; set; }
    }

    public class NavigateEventArgs : EventArgs
    {
        public NavigateEventArgs(int pageId, Type pageType)
        {
            PageId = pageId;
            PageType = pageType;
        }
        public Type PageType { get; set; }
        public int PageId { get; set; }
    }

    public class WebMapEventArgs : EventArgs
    {
        public WebMapEventArgs(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;
        }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class TrackingEventArgs : EventArgs
    {
        public TrackingEventArgs(string trailId, bool monitoringState)
        {
            TrailId = trailId;
            MonitoringState = monitoringState;
        }

        public string TrailId { get; set; }
        public bool MonitoringState { get; set; }
    }

    public class ResumeTrailMonitoringArgs : EventArgs
    {
        public ResumeTrailMonitoringArgs(string trailId)
        {
            TrailId = trailId;
        }
        public string TrailId { get; set; }
    }

    public class AllowCaptureModeArgs : EventArgs
    {
        public AllowCaptureModeArgs(bool enabled) => Enabled = enabled;

        public bool Enabled { get; set; }
    }

    public class DevModeGeoFenceStateArgs : EventArgs
    {
        public DevModeGeoFenceStateArgs(bool enabled) => Enabled = enabled;

        public bool Enabled { get; set; }
    }

    public class ZoomImageEventArgs : EventArgs
    {
        public ZoomImageEventArgs(ImageSource image) => Image = image;

        public ImageSource Image { get; set; }
    }
}
