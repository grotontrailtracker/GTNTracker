using System;
using System.Collections.Generic;
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
    public partial class TrailContentPage : BasePage
    {
        private double _ScrollerMaxHeight = 150.0;
        private double _ScrollerDefaultMinHeight = 50.0;
        private double _scrollerContentFullSize;
        private bool _firstSizingDone = false;
        private List<string> _activeTrailTrackings = new List<string>();

        public TrailContentPage()
        {
            InitializeComponent();
            NotificationService.Instance.Tracking += HandleTrackingChange;
            foreach (var trail in NotificationService.Instance.CurrentTrackingList)
            {
                _activeTrailTrackings.Add(trail);
            }

            TrackingBtn.IsVisible = _activeTrailTrackings.Any();
            UpdateToolbar();
        }

        private void TrailRegionListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedItem = e.SelectedItem as TrailRegionVM;
            if (selectedItem != null)
            {
                selectedItem.GetInfoCommand.Execute(null);
                TrailRegionListView.SelectedItem = null;
            }
        }

        private void TrailInfoBtn_Activated(object sender, EventArgs e)
        {
            var vm = BindingContext as TrailContentViewModel;
            if (vm != null)
            {
                var trailRegion = vm.RegionList.FirstOrDefault(r => !r.Entered);
                if (trailRegion == null)
                {
                    trailRegion = vm.RegionList.FirstOrDefault();   // fallback to first on list then if all are complete!
                }

                if (trailRegion != null)
                {
                    var region = TrailDefService.Instance.GetRegionDefinition(trailRegion.TrailIdentifier).FirstOrDefault(r => trailRegion.RegionIdentifier == r.Identifier);
                    if (region != null)
                    {
                        NotificationService.Instance.NotifyNavigateToPage(PageManager.TrailMapPageId, typeof(TrailMap));
                        NotificationService.Instance.NotifyWebMap(region.Center.Latitude, region.Center.Longitude);
                    }
                }
                
            }
        }

        private async void StartTrailBtn_Activated(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Start Trail Tracking?", 
                "GPS Location Service will start tracking trail waypoints.", "Yes", "No");
            if (answer)
            {
                var vm = BindingContext as TrailContentViewModel;
                if (vm != null)
                {
                    vm.Start();
                    UpdateToolbar();
                }
            }
        }

        private async void StopTrailBtn_Activated(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Stop Trail Tracking?", 
                "GPS Location Service will stop monitoring waypoints.", "Yes", "No");
            if (answer)
            {
                var vm = BindingContext as TrailContentViewModel;
                if (vm != null)
                {
                    vm.Stop();
                    UpdateToolbar();
                }
            }
        }

        private void HandleMoreBtn(object sender, EventArgs e)
        {
            double heightToUse = _scrollerContentFullSize;
            if (heightToUse > _ScrollerMaxHeight)
            {
                heightToUse = _ScrollerMaxHeight;
            }
            this.DescriptionScroll.HeightRequest = heightToUse;
            MoreBtn.IsVisible = false;
            LessBtn.IsVisible = true;
        }

        private void HandleLessBtn(object sender, EventArgs e)
        {
            this.DescriptionScroll.HeightRequest = 50;
            MoreBtn.IsVisible = true;
            LessBtn.IsVisible = false;
        }

        private void Handle_ContentAppearing(object sender, EventArgs e)
        {
            if (!_firstSizingDone)
            {
                var scroller = DescriptionScroll;
                var size = DescriptionScroll.ContentSize;
                _scrollerContentFullSize = size.Height;
                // let's be a bit smart
                if (_scrollerContentFullSize < _ScrollerDefaultMinHeight)
                {
                    // don't bother with the buttons, resize to fit
                    MoreBtn.IsVisible = false;
                    scroller.HeightRequest = _scrollerContentFullSize; // + 5.0;    // add some padding
                    var newMargin = new Thickness(0, 0, 0, -5);
                    this.StartStopLayout.Margin = newMargin;
                }
                else
                {
                    scroller.HeightRequest = _ScrollerDefaultMinHeight;
                }

                _firstSizingDone = true;
            }
        }

        private async void TrailStatusBtn_Activated(object sender, EventArgs e)
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

        private void HandleTrackingChange(object sender, TrackingEventArgs args)
        {
            if (args.MonitoringState)
            {
                if (!_activeTrailTrackings.Any(t => t == args.TrailId))
                {
                    _activeTrailTrackings.Add(args.TrailId);
                }
            }
            else
            {
                _activeTrailTrackings.Remove(args.TrailId);
            }

            TrackingBtn.IsVisible = _activeTrailTrackings.Any();
            UpdateToolbar();
        }
    }
}