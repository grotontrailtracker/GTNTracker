﻿using GTNTracker.EventArguments;
using GTNTracker.Services;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrailList : BasePage
    {
        private List<string> _currentTrackingList = new List<string>();

        public TrailList()
        {
            InitializeComponent();
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
        }

        private async void TrailListItem_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedItem = e.Item as TrailListItemVM;
            if (selectedItem != null)
            {
                var vm = selectedItem.TrailPage.BindingContext as TrailContentViewModel;
                if (vm != null)
                {
                    var anotherActive = false;
                    var activeTrail = AppStateService.Instance.ActiveTrailId;
                    if (!string.IsNullOrEmpty(activeTrail) && vm.TrailId != activeTrail || (vm.IsTrailListComplete && !vm.IsStarted))
                    {
                        anotherActive = true;
                    }
                        
                    vm.EnableStartStopTracking = !anotherActive;
                }

                await Navigation.PushAsync(selectedItem.TrailPage);
                TrailListView.SelectedItem = null;
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
            var statusPage = PageManager.Instance.CurrentStatusPopup;  //new CurrentStatusPage();
            var vm = ViewModelLocator.Instance.CurrentLocationVM;
            vm.UpdateCurrentPosition();
            statusPage.BindingContext = vm;

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