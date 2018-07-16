using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTNTracker.Types
{
    internal class GeofenceState
    {
        public GeofenceState(GeofenceRegion region)
        {
            this.Region = region;
            this.Status = GeofenceStatus.Unknown;
        }

        public GeofenceRegion Region { get; }
        public GeofenceStatus Status { get; set; }

    }
}
