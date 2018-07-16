using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTNTracker.Types;

namespace GTNTracker.EventArguments
{
    public class GeofenceStatusChangedEventArgs : EventArgs
    {
        public GeofenceStatusChangedEventArgs(GeofenceRegion region, GeofenceStatus status, Position pos)
        {
            this.Region = region;
            this.Status = status;
            Position = pos;
        }

        public GeofenceRegion Region { get; }
        public GeofenceStatus Status { get; }
        public Position Position { get; }
    }

    //public class GeoFenceServiceEventArgs : EventArgs
    //{
    //    public GeoFenceServiceEventArgs(GeofenceStatus status, GeofenceRegion region, Position pos)
    //    {
    //        Status = status;
    //        Region = region;
    //        Position = pos;
    //    }
    //    public GeofenceStatus Status { get; set; }
    //    public GeofenceRegion Region { get; set; }

    //    public Position Position { get; set; }
    //}

    //public class GeoFenceServicePositionChangedEventArgs : EventArgs
    //{
    //    public Position Position { get; set; }
    //    public double Accuracy { get; set; }

    //    public GeoFenceServicePositionChangedEventArgs(Position pos, double accuracy)
    //    {
    //        Position = pos;
    //        Accuracy = accuracy;
    //    }
    //}
}
