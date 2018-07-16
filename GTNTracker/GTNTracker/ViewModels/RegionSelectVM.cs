using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTNTracker.ViewModels
{
    public class RegionSelectVM : TrailRegionVM
    {
        private string _distance;
        public string Distance
        {
            get => _distance;
            set => SetProperty(ref _distance, value);
        }

        public RegionSelectVM(TrailRegionVM other)
        {
            Entered = other.Entered;
            RegionName = other.RegionName;
            DateCompleted = other.DateCompleted;
            TrailImage = other.TrailImage;
            ShowImage = other.ShowImage;
            RegionIdentifier = other.RegionIdentifier;
        }
    }
}
