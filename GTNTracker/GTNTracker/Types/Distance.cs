﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTNTracker.Types
{
    public sealed class Distance
    {
        public const double MILES_TO_KM = 1.60934;
        public const double KM_TO_MILES = 0.621371;
        public const int KM_TO_METERS = 1000;
        public const double METER_TO_FEET = 3.28084;
        public const double METER_TO_YARD = 1.09361;


        public double TotalMiles => this.TotalKilometers * KM_TO_MILES;
        public double TotalMeters => this.TotalKilometers * 1000;
        public double TotalFeet => this.TotalKilometers * 1000 * METER_TO_FEET;

        public double TotalYards => this.TotalKilometers * 1000 * METER_TO_YARD;

        public double TotalKilometers { get; set; }


        public override string ToString()
        {
            return $"{this.TotalKilometers} KM";
        }


        public override bool Equals(object obj)
        {
            var other = obj as Distance;
            if (other == null)
                return false;

            if (this.TotalKilometers.Equals(other.TotalKilometers))
                return false;

            return false;
        }

        public override int GetHashCode()
        {
            return this.TotalKilometers.GetHashCode();
        }


        public static Distance FromMiles(int miles)
        {
            return new Distance
            {
                TotalKilometers = miles * MILES_TO_KM
            };
        }


        public static Distance FromMeters(double meters)
        {
            return new Distance
            {
                TotalKilometers = meters / KM_TO_METERS
            };
        }


        public static Distance FromKilometers(double km)
        {
            return new Distance
            {
                TotalKilometers = km
            };
        }


        public static bool operator ==(Distance x, Distance y)
        {
            return x.TotalKilometers == y.TotalKilometers;
        }


        public static bool operator !=(Distance x, Distance y)
        {
            return x.TotalKilometers != y.TotalKilometers;
        }


        public static bool operator >(Distance x, Distance y)
        {
            return x.TotalKilometers > y.TotalKilometers;
        }


        public static bool operator <(Distance x, Distance y)
        {
            return x.TotalKilometers < y.TotalKilometers;
        }
    }
}
