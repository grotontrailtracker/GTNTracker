using System.Collections.Generic;
using GTNTracker.Types;

namespace GTNTracker.Interfaces
{
    class MsgCenterInterfaces
    {
    }

    public class GeofenceUpdated
    {
        public static string MessageString = "GeofenceUpdated";
    }

    public class GeofenceUpdatedArgs
    {
        public GeofenceUpdatedArgs(GeofenceStatus status, GeofenceRegion region)
        {
            Region = region;
            Status = status;
        }

        public GeofenceRegion Region { get; set; }
        public GeofenceStatus Status { get; set; }
    }

    public class GeofenceVisited
    {
        public static string MessageString = "GeofenceVisited";
    }

    public class GeofenceVisitedArgs
    {
        public GeofenceVisitedArgs(GeofenceRegion region)
        {
            Region = region;
        }

        public GeofenceRegion Region { get; set; }
    }

    public class StartGeofencing
    {
        public static string MessageString = "StartGeofencing";
    }

    public class GeofencingStarted
    {
        public static string MessageString = "GeofencingStarted";
    }

    public class StopGeofencing
    {
        public static string MessageString = "StopGeofencing";
    }

    public class StopGeofencingArgs
    {
        public StopGeofencingArgs(bool forceStop = false)
        {
            ForceStop = forceStop;
        }
        public bool ForceStop { get; set; }
    }

    public class RegisterMonitoringRegions
    {
        public static string MessageString = "RegisterMonitoringRegions";
    }

    public class RegisterMonitoringRegionsArgs
    {
        public RegisterMonitoringRegionsArgs(List<GeofenceRegion> regions)
        {
            Regions = regions;
        }
        public List<GeofenceRegion> Regions { get; set; }
    }

    public class StopMonitoringRegions
    {
        public static string MessageString = "StopMonitoringRegions";
    }

    public class StopMonitoringRegionsArgs
    {
        public StopMonitoringRegionsArgs(List<GeofenceRegion> regions)
        {
            Regions = regions;
        }
        public List<GeofenceRegion> Regions { get; set; }
    }

    public class UIStopAllMonitoringRegions
    {
        public static string MessageString = "UIStopAllMonitoringRegions";
    }

    public class GeoPositionChanged
    {
        public static string MessageString = "GeoPositionChanged";
    }

    public class GeoPositionChangedArgs
    {
        public Position Position { get; set; }
        public double Accuracy { get; set; }

        public GeoPositionChangedArgs(Position pos, double accuracy)
        {
            Position = pos;
            Accuracy = accuracy;
        }
    }

    public class RequestMonitoredRegions
    {
        public static string MessageString = "RequestMonitoredRegions";
    }

    public class GeofenceMonitoredRegions
    {
        public static string MessageString = "GeofenceMonitoredRegions";
    }

    public class GeofenceMonitoredRegionsArgs
    {
        public GeofenceMonitoredRegionsArgs(List<GeofenceRegion> regions)
        {
            Regions = regions;
        }

        public List<GeofenceRegion> Regions { get; set; }
    }

    public class RequestCurrentPosition
    {
        public static string MessageString = "RequestCurrentPosition";
    }

    public class GeofenceCurrentPosition
    {
        public static string MessageString = "GeofenceCurrentPosition";
    }

    public class GeofenceCurrentPositionArgs
    {
        public GeofenceCurrentPositionArgs(Position pos, double accuracy)
        {
            Position = pos;
            Accuracy = accuracy;
        }
        public Position Position { get; set; }
        public double Accuracy { get; set; }
    }

    public class RequestCurrentPositionString
    {
        public static string MessageString = "RequestCurrentPositionString";
    }

    public class GeofenceCurrentPositionString
    {
        public static string MessageString = "GeofenceCurrentPositionString";
    }

    public class NotificationDestroyEvent
    {
        public static string MessageString = "NotificationDestroyEvent";
    }

    public class WaypointEMailedArgs
    {
        public WaypointEMailedArgs(int id)
        {
            Id = id;
        }
        public int Id { get; set; }
    }
    public class WaypointEmailed
    {
        public static string MessageString = "WaypointEmailed";
    }
}
