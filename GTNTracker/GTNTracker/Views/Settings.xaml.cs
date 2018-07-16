using System;
using GTNTracker.Models;
using GTNTracker.Services;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : BasePage
    {
        private TableSection _developerSection;
        public AppSettings _appSettings;
        public Settings()
        {
            InitializeComponent();
            _appSettings = AppSettingsService.Instance.AppSettings;

            _developerSection = SettingsRoot[SettingsRoot.IndexOf(developerSection)];

            if (Device.RuntimePlatform == Device.iOS)
            {
                TimestampLabel.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
                WarningText.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
                RecoverText.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
                ResetText.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
            }
        }

        private async void ResetWaypointsBtn_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Reset All Visits?", "Do you really want to reset all your waypoint visits? Data will be gone forever!", "OK", "Cancel");
            if (result)
            {
                var askAgain = await DisplayAlert("Confirm Reset Visits?", "This will remove all your trail visits and cannot be undone! Are you really sure?", "Yes", "No");
                if (askAgain)
                {
                    var visitService = TrailVisitService.Instance;
                    if (visitService != null)
                    {
                        visitService.ClearSettings();
                        await DisplayAlert("", "All Trail Visits Have Been Removed", "OK");
                        WarningPanel.IsVisible = false;
                        resetStackPanel.IsVisible = false;
                        RecoverPanel.IsVisible = true;
                        TimestampPanel.IsVisible = true;
                        RecoverViewCell.ForceUpdateSize();
                        var timeStamp = visitService.IsBackupAvailable();
                        if (timeStamp.HasValue)
                        {
                            var time = timeStamp.Value.ToString("f");
                            RecoverWaypointsBtn.IsEnabled = true;
                            TimestampLabel.Text = "Last Visit Backup: " + time;
                        }
                        else
                        {
                            RecoverWaypointsBtn.IsEnabled = false;
                        }
                    }
                }
            }
        }

        private void DisplayPopupsChanged(object sender, ToggledEventArgs e)
        {
            _appSettings.DisplayVisitPopups = PopupSwitch.On;
            AppSettingsService.Instance.UpdateAppSettings(_appSettings);
        }

        private void MetricChanged(object sender, ToggledEventArgs e)
        {
            _appSettings.DisplayMeters = MetricSwitch.On;
            AppSettingsService.Instance.UpdateAppSettings(_appSettings);
        }

        private void DeveloperModeChanged(object sender, ToggledEventArgs e)
        {
            _appSettings.DeveloperMode = DeveloperSwitch.On;
            AppSettingsService.Instance.UpdateAppSettings(_appSettings);
        }

        private async void HandlePasswordDialogClosed(object sender, PasswordEventArgs args)
        {
            var vm = args.ViewModel;
            if (vm.Cancelled)
            {
                CaptureSwitch.On = false;
                return;
            }
            if (vm.Password != "Troop1")
            {
                await DisplayAlert("Error", "Incorrect Password!", "OK");
                CaptureSwitch.On = false;
                return;
            }

            _appSettings.AllowCapture = CaptureSwitch.On; //CaptureToggle.IsToggled;
            AppSettingsService.Instance.UpdateAppSettings(_appSettings);
            NotificationService.Instance.NotifyAllowWaypointCapture(true);
        }

        private void HandleCaptureSwitchChanged(object sender, ToggledEventArgs e)
        {
            if (CaptureSwitch.On)
            {
                var currValue = AppSettingsService.Instance.AppSettings.AllowCapture;
                if (currValue)
                {
                    return;
                }

                //var vm = new PasswordVM()
                //{
                //    Title = "Password Required",
                //    Prompt = "Enter Password:"
                //};
                //var passwordDlg = new Password();
                //passwordDlg.DialogClosed += HandlePasswordDialogClosed;
                //passwordDlg.BindingContext = vm;
                //await Navigation.PushPopupAsync(passwordDlg);

                _appSettings.AllowCapture = CaptureSwitch.On; //CaptureToggle.IsToggled;
                AppSettingsService.Instance.UpdateAppSettings(_appSettings);
                NotificationService.Instance.NotifyAllowWaypointCapture(true);
            }
            else
            {
                _appSettings.AllowCapture = false;
                AppSettingsService.Instance.UpdateAppSettings(_appSettings);
                NotificationService.Instance.NotifyAllowWaypointCapture(false);
            }
        }

        protected override void OnAppearing()
        {
            _appSettings = AppSettingsService.Instance.AppSettings;

            if (_appSettings.DeveloperMode)
            {
                if (SettingsRoot.IndexOf(developerSection) < 0)
                {
                    SettingsRoot.Add(_developerSection);
                }
            }
            else
            {
                if (SettingsRoot.IndexOf(developerSection) >= 0)
                {
                    SettingsRoot.Remove(developerSection);
                }
            }
            DeveloperSwitch.On = _appSettings.DeveloperMode;

            // adjust toggle states to settings values
            PopupSwitch.On = _appSettings.DisplayVisitPopups;
            MetricSwitch.On = _appSettings.DisplayMeters;

            CaptureSwitch.On = _appSettings.AllowCapture;

            hostNameEntry.Text = AppStateService.Instance.TestIISHost;

            base.OnAppearing();
        }

        private async void RecoverWaypointsBtn_Clicked(object sender, EventArgs e)
        {
            var ask = await DisplayAlert("Recover Visit Data", "Do you want to reload reset visit data?", "Yes", "No");
            if (ask)
            {
                var visitService = TrailVisitService.Instance;
                visitService.RecoverBackupVisitData();
                RecoverPanel.IsVisible = false;
                WarningPanel.IsVisible = true;
                TimestampPanel.IsVisible = false;
                resetStackPanel.IsVisible = true;
            }
        }

        private async void LoadTestDataBtn_Clicked(object sender, EventArgs e)
        {
            TrailVisitService.Instance.LoadTestData();
            await DisplayAlert("", "Trail Test Data Loaded", "OK");
        }

        private void setHostNameBtn_Clicked(object sender, EventArgs e)
        {
            AppStateService.Instance.TestIISHost = hostNameEntry.Text;
        }
    }
}