using System;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class AppMenuItemVM : ViewModelBase
    {
        private int _id;
        private string _menuTitle;
        private string _title;
        private Type _pageType;
        private ImageSource _imageSource;
        private bool _showImage;
        private string _imageFile;
        private bool _isAvailable = true;

        public AppMenuItemVM()
        {
        }

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        public string MenuTitle
        {
            get => _menuTitle;
            set => SetProperty(ref _menuTitle, value);
        }
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public Type TargetType
        {
            get => _pageType;
            set => SetProperty(ref _pageType, value);
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set => SetProperty(ref _isAvailable, value);
        }

        public bool ShowImage
        {
            get => _showImage;
            set => SetProperty(ref _showImage, value);
        }

        public string ImageFilename
        {
            get => _imageFile;
            set
            {
                _imageFile = value;
                if (string.IsNullOrEmpty(value))
                {
                    ShowImage = false;
                    IconImage = ImageSource.FromResource("GTNTracker.Images.UnderConstruct.jpg");
                }
                else
                {
                    IconImage = ImageSource.FromResource(value);
                }
            }
        }
        public ImageSource IconImage
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }
    }
}
