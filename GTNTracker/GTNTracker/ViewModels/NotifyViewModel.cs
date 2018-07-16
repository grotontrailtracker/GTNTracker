using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class NotifyViewModel : ViewModelBase
    {
        private ImageSource _image;
        private string _descr;
        private string _title;
        public ImageSource ImageData
        {
            get => _image;
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
