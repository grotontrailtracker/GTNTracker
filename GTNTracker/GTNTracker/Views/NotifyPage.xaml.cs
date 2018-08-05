using System;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotifyPage : PopupPage
    {
        public NotifyPage()
        {
            InitializeComponent();
        }

        // we need to use this method since we want to control whether we use a standard image
        // when it's for a notification (non-full screen) or allow the user to go and use the
        // zoom image directly by creating a ZoomImage.
        public void SetupContentView(bool isFullScreen, ImageSource image)
        {
            if (!isFullScreen)
            {
                var imageViewer = new FFImageLoading.Forms.CachedImage
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    DownsampleToViewSize = true,
                    BindingContext = this.BindingContext,
                    LoadingPlaceholder = ImageSource.FromFile("loading.gif"),
                    ErrorPlaceholder = ImageSource.FromFile("error404.png"),
                    Source = image
                };
                this.ImageContentView.Content = imageViewer;
                var gesture = new TapGestureRecognizer();
                gesture.Tapped += (sender, e) =>
                {
                    OnTapGestureRecognizerTapped(this, null);
                };
                this.ImageContentView.GestureRecognizers.Add(gesture);
            }
            else
            {
                var zoomViewer = new ZoomImage
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    DownsampleToViewSize = true,
                    BindingContext = this.BindingContext,
                    LoadingPlaceholder = ImageSource.FromFile("loading.gif"),
                    ErrorPlaceholder = ImageSource.FromFile("error404.png"),
                    Source = image
                };
                this.ImageContentView.Content = zoomViewer;
            }
        }

        private void OnCloseButtonTapped(object sender, EventArgs e)
        {
            ClosePopup();
        }

        protected override bool OnBackgroundClicked()
        {
            //ClosePopup();

            return false;
        }

        private async void ClosePopup()
        {
            var vm = BindingContext as NotifyViewModel;
            if (vm != null)
            {
                if (vm.IsFullScreen)
                {
                    await Navigation.PopAllPopupAsync();
                }
                else
                {
                    await Navigation.PopPopupAsync();
                }
            }
            else
            {
                await Navigation.PopPopupAsync();
            }
        }

        private async void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
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
