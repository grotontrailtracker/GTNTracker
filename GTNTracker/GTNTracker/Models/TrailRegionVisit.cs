using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTNTracker.Models
{
    public class TrailRegionVisit
    {
        public string TrailIdentifer { get; set; }

        public string RegionIdentifier { get; set; }

        public DateTime Completed { get; set; }
    }
}
