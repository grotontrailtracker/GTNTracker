using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class CongratsViewModel : ViewModelBase
    {
        private ImageSource _image;
        private string _descr;
        private string _title;
        private string _websiteDescr;

        public CongratsViewModel()
        {
            Title = "Congratulations!";
            Description = "You've completed visiting our list of trails! We hope you had a fun time exploring the Groton Trail Network with the Tracker App. "
                + "If you'd like, use the button below to send us an email.";
            GotoWebsiteDescription = "This application covers only some of the wonderful trails the Town of Groton has to offer. Visit the Groton Trail Network website for more information";
        }

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

        public string GotoWebsiteDescription
        {
            get => _websiteDescr;
            set => SetProperty(ref _websiteDescr, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}
