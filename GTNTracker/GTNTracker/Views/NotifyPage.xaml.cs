using System;
using System.Diagnostics;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotifyPage : PopupPage
    {
        private double _ScrollerMaxHeight = 150.0;
        private double _ScrollerDefaultMinHeight = 50.0;
        private double _scrollerContentFullSize;
        private bool _firstSizingDone = false;

        public NotifyPage()
        {
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
            await Navigation.PopAllPopupAsync();
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
                    if (scroller.HeightRequest == 0)
                    {
                        scroller.HeightRequest = _ScrollerDefaultMinHeight;
                    }
                }
                else
                {
                    scroller.HeightRequest = _ScrollerDefaultMinHeight;
                }

                _firstSizingDone = true;
            }
        }

        private async void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            Debug.WriteLine("Here I am!");
            var imageVM = ViewModelLocator.Instance.ImageVM;
            var myContext = BindingContext as NotifyViewModel;
            if (myContext != null)
            {
                imageVM.ImageData = myContext.ImageData;
                var popup = PageManager.Instance.ImagePopup;
                popup.Initialize();
                popup.BindingContext = imageVM;
                await Navigation.PushPopupAsync(popup);
            }
        }
    }
}
