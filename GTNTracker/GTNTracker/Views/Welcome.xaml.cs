using System;
using GTNTracker.Services;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Welcome : BasePage
    {
        public Welcome()
        {
            InitializeComponent();

            welcomeImage.Source = ImageSource.FromResource("GTNTracker.Images.badge-photo.jpg");
            versionLbl.Text = string.Format("Version: {0}", AppStateService.Instance.AppVersion);

        }

        //protected override bool OnBackButtonPressed()
        //{
        //    return true;
        //}
    }
}
