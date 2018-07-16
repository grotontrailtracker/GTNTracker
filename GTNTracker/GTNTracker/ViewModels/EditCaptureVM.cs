using System;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class EditCaptureVM : ViewModelBase
    {
        private string _longitude;
        private string _latitude;
        private string _accuracy;
        private string _trailName;
        private string _name;
        private string _description;
        private DateTime _capturedTime;
        private ImageSource _image;

        public bool IsCancelled { get; set; }
        public bool IsSend { get; set; }
        public int Id { get; set; }

        public string Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }
        public string Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        public string Accuracy
        {
            get => _accuracy;
            set => SetProperty(ref _accuracy, value);
        }
        public string TrailName
        {
            get => _trailName;
            set => SetProperty(ref _trailName, value);
        }

        public string WaypointName
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public DateTime CapturedTime
        {
            get => _capturedTime;
            set => SetProperty(ref _capturedTime, value);
        }

        public EditCaptureVM(WaypointCaptureDataVM other)
        {
            Id = other.Id;
            Longitude = other.Longitude;
            Latitude = other.Latitude;
            Accuracy = other.Accuracy;
            TrailName = other.TrailName;
            WaypointName = other.WaypointName;
            Description = other.Description;
            CapturedTime = other.DateCaptured;
            Image = other.Image;
        }
    }
}
