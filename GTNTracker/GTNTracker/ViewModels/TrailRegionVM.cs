using System;
using System.Linq;
using System.Windows.Input;
using GTNTracker.Services;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class TrailRegionVM : ViewModelBase
    {
        private string _regionIdentifier;
        private string _regionName;
        private string _trailIdentifier;
        private bool _entered;
        private DateTime? _dateCompleted;
        private ICommand _getInfoCommand;
        private ImageSource _imageSource;
        private bool _showImage;

        public string RegionIdentifier
        {
            get => _regionIdentifier;
            set => SetProperty(ref _regionIdentifier, value);
        }

        public string RegionName
        {
            get => _regionName;
            set => SetProperty(ref _regionName, value);
        }
        public string TrailIdentifier
        {
            get=> _trailIdentifier; 
            set => SetProperty(ref _trailIdentifier, value);
        }

        public bool Entered
        {
            get => _entered;
            set => SetProperty(ref _entered, value);
        }

        public DateTime? DateCompleted
        {
            get => _dateCompleted;
            set => SetProperty(ref _dateCompleted, value);
        }

        public ImageSource TrailImage
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        public bool ShowImage
        {
            get => _showImage;
            set => SetProperty(ref _showImage, value);
        }
        public ICommand GetInfoCommand
        {
            get => _getInfoCommand;
            set => SetProperty(ref _getInfoCommand, value);
        }

        public TrailRegionVM()
        {
            GetInfoCommand = new Command(GetRegionInfo);
        }

        private void GetRegionInfo()
        {
            // need to find the region object and then do a notify on it.
            var regionService = TrailDefService.Instance;
            var regionDef = regionService.GetRegionDefinition(TrailIdentifier).FirstOrDefault(r => r.Identifier == RegionIdentifier);

            if (regionDef != null)
            {
                NotificationService.Instance.NotifyRegion(regionDef, true);
            }
        }
    }
}
