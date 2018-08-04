using System;
using System.Linq;
using GTNTracker.EventArguments;
using GTNTracker.Services;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppMasterPage : ContentPage
    {
        public ListView ListView => ListViewMenuItems;  // allows root page to have access to menuitems held in this view

        public AppMasterPage()
        {
            InitializeComponent();
            var vm = new AppMenuVM();
            vm.Initialize();

            BindingContext = vm;
            versionLbl.Text = string.Format("Version: {0}", AppStateService.Instance.AppVersion);

            var notifyService = NotificationService.Instance;
            notifyService.RegionUpdated += HandleNotifyRegionUpdate;
            notifyService.TrailComplete += HandleTrailComplete;
            notifyService.ZoomImage += HandleZoomImage;

            NotificationService.Instance.Tracking += HandleTrackingChange;
            notifyService.DevModeGeoServiceState += HandleDevModeGeoServiceState;
        }

        private void HandleTrackingChange(object sender, TrackingEventArgs e)
        {
            var vm = BindingContext as AppMenuVM;
            if (vm == null)
            {
                return;
            }

            // The menu list has an option for stopping the trail tracking, need to activate it only
            // when a trail is being actively monitored.
            var listItem = vm.MenuItems.FirstOrDefault(m => m.Id == PageManager.StopTrackingId);
            if (e.MonitoringState)
            {
                // enable stop option
                listItem.IsAvailable = true;
            }
            else
            {
                // disable stop option
                listItem.IsAvailable = false;
            }
        }

        private void HandleDevModeGeoServiceState(object sender, DevModeGeoFenceStateArgs args)
        {
            var vm = BindingContext as AppMenuVM;
            if (vm == null)
            {
                return;
            }

            // The menu list has an option for stopping the trail tracking, need to activate it only
            // when a trail is being actively monitored.
            var listItem = vm.MenuItems.FirstOrDefault(m => m.Id == PageManager.StopGeoService);
            listItem.IsAvailable = args.Enabled;
        }

        private async void HandleNotifyRegionUpdate(object sender, GeofenceRegionUpdatedArgs args)
        {
            var showPopup = AppSettingsService.Instance.AppSettings.DisplayVisitPopups;
            if (!showPopup)
            {
                return;
            }

            var notifyPageVM = new NotifyViewModel();
            var region = args.Region;
            notifyPageVM.Title = !string.IsNullOrEmpty(region.Name) ? region.Name : region.Identifier;
            notifyPageVM.Description = !string.IsNullOrEmpty(region.Description) ? region.Description : "<To Be Added>";
            var imgNameToUse = !string.IsNullOrEmpty(region.ImageName) ? region.ImageName : "GTNTracker.Images.UnderConstruct.jpg";
            if (string.IsNullOrEmpty(region.ImageName))
            {
                region.IsImageNameURI = false;  // enforce this especially if we didn't have a name
            }

            if (region.IsImageNameURI)
            {
                notifyPageVM.ImageData = ImageSource.FromUri(new System.Uri(region.ImageName));
            }
            else
            {
                notifyPageVM.ImageData = ImageSource.FromResource(imgNameToUse);
            }
            var notifyPage = new NotifyPage();
            notifyPage.BindingContext = notifyPageVM;

            if (AppStateService.Instance.IsAppAwake)
            {
                await Navigation.PushPopupAsync(notifyPage);
            }
        }

        private void HandleTrailComplete(object sender, TrailCompleteEventArgs args)
        {
            var notifyPageVM = new TrailCompleteViewModel();
            var trailName = args.TrailName;
            notifyPageVM.Title = trailName + " Completed!";
            //notifyPageVM.Description = !string.IsNullOrEmpty(region.Description) ? region.Description : "<To Be Added>";
            var imgNameToUse = "GTNTracker.Images.welldone.jpg";

            notifyPageVM.ImageData = ImageSource.FromResource(imgNameToUse);
            var notifyPage = new TrailCompletePage();
            notifyPage.BindingContext = notifyPageVM;
        }

        private async void HandleZoomImage(object sender, ZoomImageEventArgs args)
        {
            var imageVM = ViewModelLocator.Instance.ImageVM;
            imageVM.ImageData = args.Image;
            var popup = PageManager.Instance.ImagePopup;
            popup.Initialize();
            popup.BindingContext = imageVM;
            await Navigation.PushPopupAsync(popup);
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
    }
}
