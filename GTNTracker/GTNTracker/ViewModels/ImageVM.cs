using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class ImageVM : ViewModelBase
    {
        private ImageSource _image;

        public ImageSource ImageData
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }
    }
}
