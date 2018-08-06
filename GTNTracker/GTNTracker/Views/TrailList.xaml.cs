using GTNTracker.EventArguments;
using GTNTracker.Services;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace GTNTracker.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrailList : BasePage
    {
        private List<string> _currentTrackingList = new List<string>();

        public TrailList()
        {
            InitializeComponent();
            Xamarin.Forms.NavigationPage.SetBackButtonTitle(this, string.Empty);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            var vm = ViewModelLocator.Instance.TrailListViewModel;       
            BindingContext = vm;

            if (!string.IsNullOrEmpty(AppStateService.Instance.ActiveTrailId))
            {
                TrackingStatusBtn.IsVisible = true;
            }
            else
            {
                TrackingStatusBtn.IsVisible = false;
            }

            NotificationService.Instance.Tracking += HandleTrackingChange;

            SizeChanged += TrailList_SizeChanged;
        }

        private void TrailList_SizeChanged(object sender, EventArgs e)
        {
            AppStateService.Instance.WindowHeight = Height;
            AppStateService.Instance.WindowWidth = Width;
        }

        //private async void TrailListItem_ItemTapped(object sender, ItemTappedEventArgs e)
        private async void TrailListItem_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedItem = e.SelectedItem as TrailListItemVM; // e.Item as TrailListItemVM;
            if (selectedItem != null)
            {
                var anotherActive = false;
                var vm = selectedItem.TrailPage.BindingContext as TrailContentViewModel;
                if (vm != null)
                {
                    var trackingActive = !string.IsNullOrEmpty(AppStateService.Instance.ActiveTrailId);
                    var activeTrail = AppStateService.Instance.ActiveTrailId;
                    if ((trackingActive && vm.TrailId != activeTrail) || !vm.IsViewable) 
                    {
                        anotherActive = true;
                    }
                        
                    vm.EnableStartStopTracking = !anotherActive;
                }

                if (!anotherActive)
                {
                    await Navigation.PushAsync(selectedItem.TrailPage);
                    TrailListView.SelectedItem = null;
                }
                else
                {
                    //await DisplayAlert("Warning", "Cannot look at another trail while tracking!", "OK");
                    TrailListView.SelectedItem = null;
                }
            }
        }

        private void HandleUIStopAllMonitoringRegions()
        {
            var pageVM = BindingContext as TrailListViewModel;
            if (pageVM != null)
            {
                TrackingStatusBtn.IsVisible = false;
                foreach (var listItem in pageVM.TrailList)
                {
                    var vm = listItem.ViewModel;
                    if (vm != null)
                    {
                        if (vm.IsStarted)
                        {
                            vm.Stop();
                        }

                        vm.EnableStartStopTracking = true;

                        var trailPage = listItem.TrailPage as TrailContentPage;
                        if (trailPage != null)
                        {
                            trailPage.UpdateToolbar();
                        }
                    }
                }

                NotificationService.Instance.ClearAllTracking();
            }
        }

        private void HandleTrackingChange(object sender, TrackingEventArgs e)
        {
            if (e.MonitoringState)
            {
                if (!_currentTrackingList.Any(t => t == e.TrailId))
                {
                    _currentTrackingList.Add(e.TrailId);
                }
            }
            else
            {
                _currentTrackingList.Remove(e.TrailId);
            }

            TrackingStatusBtn.IsVisible = _currentTrackingList.Any();
        }

        private async void TrackingStatusBtn_Activated(object sender, EventArgs e)
        {
            var statusPage = PageManager.Instance.CurrentStatusPopup;
            var vm = ViewModelLocator.Instance.CurrentLocationVM;
            vm.UpdateCurrentPosition();
            statusPage.BindingContext = vm;

            //var trailStatus = PageManager.Instance.TrailStatus;
            //trailStatus.BindingContext = vm;
            //await Navigation.PushAsync(trailStatus);

            await Navigation.PushPopupAsync(statusPage);
            //NotificationService.Instance.NotifyNavigateToPage("Trail Tracking Status", typeof(TrailStatus));
        }

        private async void MoreInfoBtn_Clicked(object sender, EventArgs e)
        {
            var congratPage = new CongratsPage();

            await Navigation.PushPopupAsync(congratPage);
        }
    }
}
