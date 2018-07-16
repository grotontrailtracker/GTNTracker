using System;
using System.Windows.Input;
using GTNTracker.Interfaces;
using GTNTracker.Models;
using GTNTracker.Services;
using GTNTracker.Types;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class WaypointCaptureVM : ViewModelBase
    {
        private Position _currPosition;
        private double _currPositionAccuracy;
        private string _latString = "Unknown";
        private string _longString = "Unknown";
        private string _accuracyString = "Unknown";
        private bool _positionAvailable;
        private DateTime _lastUpdate;
        private string _trailName;
        private string _waypointName;
        private string _description;
        private ImageSource _imageSource;
        private ICommand _takePictureCommand;
        private ICommand _saveCommand;
        private MediaFile _currentPhoto;
        private bool _isImageAvailable;

        public WaypointCaptureVM()
        {
            TakePictureCommand = new Command(HandleTakePicture, CanTakePicture);
            SaveCommand = new Command(HandleSave, CanSave);
            MessagingCenter.Subscribe<GeoPositionChanged, GeoPositionChangedArgs>(this, GeoPositionChanged.MessageString,
                    (sender, args) => { HandleGeoPositionChanged(args); });

            MessagingCenter.Subscribe<GeofenceCurrentPosition, GeofenceCurrentPositionArgs>(this, GeofenceCurrentPosition.MessageString,
                    (sender, args) => { HandleGeofenceCurrentPositionMsg(args); });

            //Image = ImageSource.FromResource("GTNTracker.Images.UnderConstruct.jpg");
            _currentPhoto = null;
        }

        public string Longitude
        {
            get => _longString;
            set => SetProperty(ref _longString, value);
        }

        public string Latitude
        {
            get => _latString;
            set => SetProperty(ref _latString, value);
        }

        public string Accuracy
        {
            get => _accuracyString;
            set => SetProperty(ref _accuracyString, value);
        }

        public Position Location
        {
            get => _currPosition;
        }

        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set => SetProperty(ref _lastUpdate, value);
        }

        public bool PositionAvailable
        {
            get => _positionAvailable;
            set => SetProperty(ref _positionAvailable, value);
        }

        public string TrailName
        {
            get => _trailName;
            set => SetProperty(ref _trailName, value);
        }

        public string WaypointName
        {
            get => _waypointName;
            set => SetProperty(ref _waypointName, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public ImageSource Image
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        public bool IsImageAvailable
        {
            get => _isImageAvailable;
            set => SetProperty(ref _isImageAvailable, value);
        }

        public ICommand TakePictureCommand
        {
            get => _takePictureCommand;
            set => SetProperty(ref _takePictureCommand, value);
        }

        public ICommand SaveCommand
        {
            get => _saveCommand;
            set => SetProperty(ref _saveCommand, value);
        }

        public void UpdateVM()
        {
            MessagingCenter.Send<RequestCurrentPosition>(new RequestCurrentPosition(), RequestCurrentPosition.MessageString);
            RefreshCommands();
        }

        private void HandleGeofenceCurrentPositionMsg(GeofenceCurrentPositionArgs args)
        {
            UpdatePositionProperties(args.Position, args.Accuracy);
        }
        private void HandleGeoPositionChanged(GeoPositionChangedArgs e)
        {
            UpdatePositionProperties(e.Position, e.Accuracy);
        }

        private void UpdatePositionProperties(Position pos, double accuracy)
        {
            _currPosition = pos;
            _currPositionAccuracy = accuracy;

            if (accuracy >= 0)
            {
                if (AppSettingsService.Instance.AppSettings.DisplayMeters)
                {
                    Accuracy = string.Format("{0:F1} meters", accuracy);
                }
                else
                {
                    Accuracy = string.Format("{0:F1} yards", accuracy * 1.09361);
                }
            }
            else
            {
                Accuracy = "Not Available";
            }

            var longStr = string.Format("{0:F6}°", pos.Longitude);
            Longitude = longStr.Contains(".") ? longStr.TrimEnd('0').TrimEnd('.') : longStr;

            var latStr = string.Format("{0:F6}°", pos.Latitude);
            Latitude = latStr.Contains(".") ? latStr.TrimEnd('0').TrimEnd('.') : latStr;

            PositionAvailable = true;
            LastUpdate = DateTime.Now;
        }

        private bool CanTakePicture()
        {
            var result = CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported;

            return result;
        }

        private async void HandleTakePicture()
        {
            var photo = await CrossMedia.Current.TakePhotoAsync(
                new Plugin.Media.Abstractions.StoreCameraMediaOptions()
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                    CompressionQuality = 92
                });

            if (photo != null)
            {
                // flush prior photo image then if available
                if (_currentPhoto != null)
                {
                    var path = _currentPhoto.Path;
                    DependencyService.Get<IUtilityService>().DeleteFile(path);
                }

                Image = ImageSource.FromStream(() => { return photo.GetStream(); });
                IsImageAvailable = true;
                _currentPhoto = photo;
            }

            RefreshCommands();
        }

        private bool CanSave()
        {
            bool ableToSave = false;
            if (_currentPhoto != null)
            {
                ableToSave = true;
            }

            return ableToSave;
        }

        private async void HandleSave()
        {
            // for now, let's try mailing it out
            var waypointData = new WaypointCaptureData()
            {
                Longitude = _currPosition.Longitude,
                Latitude = _currPosition.Latitude,
                Accuracy = _currPositionAccuracy,
                Name = WaypointName,
                Description = Description,
                TrailName = TrailName,
                ImagePath = _currentPhoto.Path,
                DateCaptured = DateTime.Now
            };

            WayPointCaptureService.Instance.AddWaypoint(waypointData);
            ResetData();

            await Application.Current.MainPage.DisplayAlert("Captured", "Waypoint Capture Saved", "OK");
        }

        private void ResetData()
        {
            _currentPhoto = null;
            TrailName = string.Empty;
            Description = string.Empty;
            WaypointName = string.Empty;
            IsImageAvailable = false;
        }

        private void RefreshCommands()
        {
            ((Command)TakePictureCommand).ChangeCanExecute();
            ((Command)SaveCommand).ChangeCanExecute();
        }
    }
}
