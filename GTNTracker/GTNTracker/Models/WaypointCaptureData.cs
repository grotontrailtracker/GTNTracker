using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTNTracker.Models
{
    public class WaypointCaptureData
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Accuracy { get; set; }
        public string TrailName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCaptured { get; set; }

        // Properties to help manage the acquistion of waypoint data, but not emailed.
        public string ImagePath { get; set; }
        public int Id { get; set; }
        public bool IsEmailed { get; set; }
    }

    /// <summary>
    /// Helper class to cut down on the actual data needed to be emailed.
    /// </summary>
    public class WaypointCaptureDataEmail
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Accuracy { get; set; }
        public string TrailName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCaptured { get; set; }

        public WaypointCaptureDataEmail(WaypointCaptureData other)
        {
            Longitude = other.Longitude;
            Latitude = other.Latitude;
            Accuracy = other.Accuracy;
            TrailName = other.TrailName;
            Name = other.Name;
            Description = other.Description;
            DateCaptured = other.DateCaptured;
        }
    }
}
