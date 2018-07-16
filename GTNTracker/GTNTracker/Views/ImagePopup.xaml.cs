using System;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagePopup : PopupPage
    {
        public ImagePopup()
        {
            InitializeComponent();
            this.zoomImage.SingleTapEvent += HandleSingleTap;
        }

        public void Initialize()
        {
            this.zoomImage.Initialize();
        }

        private async void CloseAllPopup()
        {
            await Navigation.PopPopupAsync();   // only close this image popup!
        }

        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();

            return false;
        }

        //private async void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        //{
        //    CloseAllPopup();
        //}

        private void HandleSingleTap(object sender, EventArgs args)
        {
            CloseAllPopup();
        }
    }
}