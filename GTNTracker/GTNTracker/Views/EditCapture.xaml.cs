using System;
using GTNTracker.EventArguments;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCapture : PopupPage
	{
        public EventHandler<DialogEventArgs> DialogClosed;

		public EditCapture ()
		{
			InitializeComponent ();
		}

        private void OnCloseButtonTapped(object sender, EventArgs e)
        {
            var vm = BindingContext as EditCaptureVM;
            vm.IsCancelled = false;
            vm.IsSend = true;   // want to email this data
            CloseAllPopup();
        }

        private void OnCancelButtonTapped(object sender, EventArgs e)
        {
            var vm = BindingContext as EditCaptureVM;
            vm.IsCancelled = true;
            CloseAllPopup();
        }

        private void OnSaveButtonTapped(object sender, EventArgs e)
        {
            var vm = BindingContext as EditCaptureVM;
            vm.IsCancelled = false;
            vm.IsSend = false; // ok, just save data away then
            CloseAllPopup();
        }

        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();

            return false;
        }

        private async void CloseAllPopup()
        {
            var vm = BindingContext as EditCaptureVM;
            await Navigation.PopAllPopupAsync();
            DialogClosed?.Invoke(this, new DialogEventArgs(vm));
        }

    }

    
}