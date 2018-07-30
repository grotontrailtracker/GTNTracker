using System.Collections.Generic;
using GTNTracker.Types;

namespace GTNTracker.Models
{
    public class TrailDefModel
    {
        public string Identifier { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public bool DeveloperMode { get; set; }

        public string ImageName { get; set; }

        public List<GeofenceRegion> Regions { get; set; }
    }
}
