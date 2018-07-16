using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GTNTracker.Types
{
    public class GeofenceRegion
    {
        public string Identifier { get; set; }
        public string TrailIdentifier { get; set; }

        [JsonIgnore]
        public string TrailName { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string ImageName { get; set; }

        public bool IsImageNameURI { get; set; }
        public Position Center { get; set; }

        /// <summary>
        /// Geofence Bubble Radius - note do not include in serialized JSON, we'll just initialize
        /// it in the TrailDefService when it's read - don't think we need custom bubbles, might be
        /// useful if gps signal is bad????
        /// </summary>
        public Distance Radius { get; set; }

        public GeofenceRegion()
        {
            Radius = new Distance();
        }

        public override bool Equals(object obj)
        {
            return this.Identifier.Equals(obj);
        }


        public override int GetHashCode()
        {
            return this.Identifier.GetHashCode();
        }
    }
}
