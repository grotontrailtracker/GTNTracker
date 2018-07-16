using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GTNTracker.EventArguments;
using GTNTracker.Types;

namespace GTNTracker.Interfaces
{
    public interface IGeofenceManager
    {
        IEnumerable<GeofenceRegion> MonitoredRegions { get; }
        Distance DesiredAccuracy { get; set; }

        event EventHandler<GeofenceStatusChangedEventArgs> RegionStatusChanged;
        event EventHandler<PositionChangedEventArgs> PositionChanged;

        Task<GeofenceStatus> RequestState(GeofenceRegion region, CancellationToken? cancelToken = default(CancellationToken?));

        bool Start();

        bool Stop();

        Task<bool> StartService();

        bool IsServiceRunning { get; }

        GeofenceStatus StartMonitoring(GeofenceRegion region);
        void StopAllMonitoring();
        void StopMonitoring(GeofenceRegion region);
    }
}
