using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class TrailCompleteViewModel : ViewModelBase
    {
        private ImageSource _image;
        private string _descr;
        private string _title;
        public ImageSource ImageData
        {
            get =>  _image; 
            set => SetProperty(ref _image, value);
        }

        public string Description
        {
            get => _descr; 
            set => SetProperty(ref _descr, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}
