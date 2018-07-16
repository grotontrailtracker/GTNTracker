using System;
using GTNTracker.Services;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Status : ContentPage
    {
        private ImageSource _imageSource;

        public Status()
        {
            InitializeComponent();
            var vm = new StatusViewModel();
            BindingContext = vm;
            vm.EditorData = "Welcome to the Groton Trail Tracker...\n";
        }

        protected override bool OnBackButtonPressed()
        {
            //NotificationService.Instance.NotifyNavigateToPage("Groton Trail Tracker", typeof(Welcome));
            NotificationService.Instance.NotifyNavigatePriorPage();
            return true;
        }

        private void ClearBtn_Clicked(object sender, EventArgs e)
        {
            var vm = BindingContext as StatusViewModel;
            if (vm != null)
            {
                vm.EditorData = string.Empty;
            }
        }

        private void GeoCheckBtn_Clicked(object sender, EventArgs e)
        {
            var vm = BindingContext as StatusViewModel;
            if (vm != null)
            {
                vm.CheckStatus();
            }
        }

        private async void ClearSettingsBtn_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Delete?", "Do you really want to delete all settings?", "Yes", "No");
            if (answer)
            {
                var visitService = TrailVisitService.Instance;
                if (visitService != null)
                {
                    visitService.ClearSettings();
 
                    await DisplayAlert("", "All Settings Cleared, Don't forget to stop monitoring", "OK");
                }
            }
        }

        private async void LoadTestBtn_Clicked(object sender, EventArgs e)
        {
            TrailVisitService.Instance.LoadTestData();
            await DisplayAlert("", "Test Settings Loaded", "OK");
        }

        private void IISButton_Clicked(object sender, EventArgs e)
        {
            AppStateService.Instance.TestIISHost = IISHostEntry.Text;
        }

        private async void CheckStatusBtn_Clicked(object sender, EventArgs e)
        {
            var statusPage = PageManager.Instance.CurrentStatusPopup;  //new CurrentStatusPage();
            var vm = ViewModelLocator.Instance.CurrentLocationVM;
            vm.UpdateCurrentPosition();
            statusPage.BindingContext = vm;

            await Navigation.PushPopupAsync(statusPage);
        }

        private async void PhotoBtn_Clicked(object sender, EventArgs e)
        {
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

            if (photo != null)
                _imageSource = ImageSource.FromStream(() => { return photo.GetStream(); });
        }
    }
}
