using GTNTracker.Models;
using GTNTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    /// <summary>
    /// View model object used to bind captured waypoint data for Capture Manager functionality
    /// </summary>
    public class WaypointCaptureDataVM : ViewModelBase
    {
        private ImageSource _image;
        private WaypointCaptureData _data;
        private bool _isEmailed;

        public WaypointCaptureDataVM()
        {
        }

        public WaypointCaptureDataVM(WaypointCaptureData data)
        {
            _data = data;
            _image = ImageSource.FromFile(_data.ImagePath);
            _isEmailed = false;
        }

        public int Id => _data.Id;
        public string Longitude => FormatGPS(_data.Longitude);
        public string Latitude => FormatGPS(_data.Latitude);
        public string Accuracy => FormatAccuracy(_data.Accuracy);
        public string TrailName
        {
            get => _data.TrailName;
            set
            {
                _data.TrailName = value;
                NotifyPropertyChanged();
            }
        }
        public string WaypointName
        {
            get => _data.Name;
            set
            {
                _data.Name = value;
                NotifyPropertyChanged();
            }
        }

        public string Description
        {
            get => _data.Description;
            set
            {
                _data.Description = value;
                NotifyPropertyChanged();
            }
        }
        public string FilePath => _data.ImagePath;
        public bool IsEmailed
        {
            get => _data.IsEmailed;
            set
            {
                _data.IsEmailed = value;
                SetProperty(ref _isEmailed, value);
            }
        }
        public DateTime DateCaptured => _data.DateCaptured;
        public ImageSource Image => _image;

        public WaypointCaptureData CaptureData
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        private string FormatGPS(double position)
        {
            return string.Format("{0:F6}°", position);
        }
        private string FormatAccuracy(double accuracy)
        {
            if (AppSettingsService.Instance.AppSettings.DisplayMeters)
            {
                return string.Format("{0:F1} meters", accuracy);
            }
            else
            {
                return string.Format("{0:F1} yards", accuracy * 1.09361);
            }
        }
    }
}

