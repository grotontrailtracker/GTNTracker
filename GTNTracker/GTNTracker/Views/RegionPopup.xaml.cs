﻿using System;
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
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace GTNTracker.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegionPopup : PopupPage
	{
		public RegionPopup ()
		{
            //On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
			InitializeComponent();
		}

        private void OnCloseButtonTapped(object sender, EventArgs e)
        {
            CloseAllPopup();
        }

        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();

            return false;
        }

        private async void CloseAllPopup()
        {
            await Navigation.PopPopupAsync();
            //await Navigation.PopAllPopupAsync();
        }

        private void TrailRegionListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            CloseAllPopup();    // ok, the VM has the selected object, we're done here.
        }
    }

    
}