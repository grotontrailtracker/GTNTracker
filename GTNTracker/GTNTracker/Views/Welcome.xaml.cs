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
            //var vm = ViewModelLocator.Instance.WelcomeVM;  //new WelcomeVM();
            //BindingContext = vm;
            //vm.PropertyChanged += ViewModel_PropertyChanged;

            welcomeImage.Source = ImageSource.FromResource("GTNTracker.Images.badge-photo.jpg");
            versionLbl.Text = string.Format("Version: {0}", AppStateService.Instance.AppVersion);
            //TrackingBtn.IsVisible = vm.IsTracking;
            //UpdateToolbar();
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsTracking")
            {
                var vm = BindingContext as WelcomeVM;
                if (vm != null)
                {
                    TrackingBtn.IsVisible = vm.IsTracking;
                    UpdateToolbar();
                }
            }
        }

        private async void TrailStatusBtn_Activated(object sender, EventArgs e)
        {
            var statusPage = PageManager.Instance.CurrentStatusPopup;
            var vm = ViewModelLocator.Instance.CurrentLocationVM;
            vm.UpdateCurrentPosition();
            statusPage.BindingContext = vm;

            await Navigation.PushPopupAsync(statusPage);
            //NotificationService.Instance.NotifyNavigateToPage("Trail Tracking Status", typeof(TrailStatus));
        }

        private async void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            var vm = new PasswordVM()
            {
                Title = "Password Required",
                Prompt = "Enter Password:"
            };
            var passwordDlg = new Password();
            passwordDlg.DialogClosed += HandlePasswordDialogClosed;
            passwordDlg.BindingContext = vm;
            await Navigation.PushPopupAsync(passwordDlg);
        }

        private async void HandlePasswordDialogClosed(object sender, PasswordEventArgs args)
        {
            var vm = args.ViewModel;
            if (vm.Cancelled)
            {
                return;
            }
            if (vm.Password != "Troop1")
            {
                await DisplayAlert("Error", "Incorrect Password!", "OK");
                return;
            }

            await this.DisplayAlert("Developer Mode", "Developer mode has been enabled", "OK");
            var appSettings = AppSettingsService.Instance.AppSettings;
            appSettings.DeveloperMode = true;
            AppSettingsService.Instance.UpdateAppSettings(appSettings);
        }

        //protected override bool OnBackButtonPressed()
        //{
        //    return true;
        //}
    }
}
