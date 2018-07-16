using System;
using GTNTracker.EventArguments;
using GTNTracker.Services;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrailStatus : BasePage
    {
        public TrailStatus()
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.Instance.CurrentLocationVM;
            NotificationService.Instance.Tracking += HandleTrackingChange;
        }

        protected override void OnAppearing()
        {
            var vm = BindingContext as CurrentLocationVM;
            if (vm != null)
            {
                vm.UpdateCurrentPosition(false);
            }

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            var vm = BindingContext as CurrentLocationVM;
            if (vm != null)
            {
                vm.Disconnect();
            }

            base.OnDisappearing();
        }

        private void HandleTrackingChange(object sender, TrackingEventArgs e)
        {
            var vm = BindingContext as CurrentLocationVM;
            if (e.MonitoringState)
            {
                vm.UpdateCurrentPosition(true);
            }
            else
            {
                vm.ResetClosestRegionData();
            }

        }

        private async void HandleChangeRegion(object sender, EventArgs args)
        {
            var vm = new RegionListVM();
            var currActiveVM = ViewModelLocator.Instance.ActiveTrailContentVM;
            var locVM = BindingContext as CurrentLocationVM;
            if (currActiveVM != null)
            {
                foreach (var reg in currActiveVM.RegionList)
                {
                    var regionVM = new RegionSelectVM(reg);
                    regionVM.Distance = locVM.FindDistance(reg.RegionIdentifier);
                    vm.RegionList.Add(regionVM);
                }
            }

            vm.PropertyChanged += ViewModel_PropertyChanged;

            var popup = new RegionPopup();
            popup.BindingContext = vm;
            await Navigation.PushPopupAsync(popup);
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedRegion")
            {
                var vm = sender as RegionListVM;
                if (vm != null)
                {
                    if (vm.SelectedRegion != null)
                    {
                        var waypointId = vm.SelectedRegion.RegionIdentifier;
                        var currVm = BindingContext as CurrentLocationVM;
                        if (currVm != null)
                        {
                            currVm.UpdateNextRegion(waypointId);
                        }
                    }
                }
            }
        }
    }
}