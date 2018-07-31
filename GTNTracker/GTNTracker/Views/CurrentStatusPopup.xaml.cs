using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTNTracker.Interfaces;
using GTNTracker.Services;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentStatusPopup : PopupPage
    {
        public CurrentStatusPopup()
        {
            InitializeComponent();
            CloseWhenBackgroundIsClicked = false;
        }

        private void OnCloseButtonTapped(object sender, EventArgs e)
        {
            CloseAllPopup();
        }

        //protected override bool OnBackgroundClicked()
        //{
        //    CloseAllPopup();

        //    return false;
        //}

        private async void CloseAllPopup()
        {
            var vm = BindingContext as CurrentLocationVM;
            if (vm != null)
            {
                vm.Disconnect();
            }

            BindingContext = null;
            await Navigation.PopAllPopupAsync();
        }

        private async void HandleChangeRegion(object sender, EventArgs args)
        {
            var vm = new RegionListVM();
            var currActiveVM = ViewModelLocator.Instance.ActiveTrailContentVM;
            var currLocVM = BindingContext as CurrentLocationVM;
            if (currActiveVM != null)
            {
                foreach (var reg in currActiveVM.RegionList)
                {
                    var regionVM = new RegionSelectVM(reg);
                    regionVM.Distance = currLocVM.FindDistance(regionVM.RegionIdentifier);
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

        /// <summary>
        /// Until the trail map gets the way points, this is not very useful.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void Locate_Clicked(object sender, EventArgs e)
        //{
        //    var vm = BindingContext as CurrentStatusVM;
        //    if (vm != null)
        //    {
        //        CloseAllPopup();

        //        if (vm.CurrentLocationVM.PositionAvailable)
        //        {
        //            var position = vm.CurrentLocationVM.Location;
        //            NotificationService.Instance.NotifyNavigateToPage("Trails Map", typeof(TrailMap));
        //            NotificationService.Instance.NotifyWebMap(position.Latitude, position.Longitude);
        //        }
        //    }
        //}
    }
}