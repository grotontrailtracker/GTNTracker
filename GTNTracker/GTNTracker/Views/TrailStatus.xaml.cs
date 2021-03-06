﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopAsync();
            return true;
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
                var tmpList = new List<RegionSelectVM>();
                foreach (var reg in currActiveVM.RegionList)
                {
                    var regionVM = new RegionSelectVM(reg);
                    var value = locVM.FindDistance(reg.RegionIdentifier);
                    regionVM.DisplayDistance = value.Item2;
                    regionVM.DistanceValue = value.Item1;
                    tmpList.Add(regionVM);
                }
                tmpList = tmpList.OrderBy(r => r.DistanceValue).ToList();
                vm.RegionList = new ObservableCollection<RegionSelectVM>(tmpList);
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

        private async void OnNextWaypointImageTapped(object sender, EventArgs args)
        {
            var imageVM = ViewModelLocator.Instance.ImageVM;
            var myContext = BindingContext as CurrentLocationVM;

            if (myContext != null)
            {
                imageVM.ImageData = myContext.TrailImage;
                var popup = PageManager.Instance.ImagePopup;
                popup.Initialize();
                popup.BindingContext = imageVM;
                await Navigation.PushPopupAsync(popup);
            }
        }

        private async void OnCurrentWaypointImageTapped(object sender, EventArgs args)
        {
            var imageVM = ViewModelLocator.Instance.ImageVM;
            var myContext = BindingContext as CurrentLocationVM;
            if (myContext != null)
            {
                if (!myContext.IsCurrentRegion)
                {
                    return;
                }

                imageVM.ImageData = myContext.CurrentRegionImage;
                var popup = PageManager.Instance.ImagePopup;
                popup.Initialize();
                popup.BindingContext = imageVM;
                await Navigation.PushPopupAsync(popup);
            }
        }
    }
}