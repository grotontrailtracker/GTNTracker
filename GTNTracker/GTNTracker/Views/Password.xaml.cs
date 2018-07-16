using System;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Password : PopupPage
	{
        public EventHandler<PasswordEventArgs> DialogClosed;

		public Password ()
		{
			InitializeComponent ();
		}

        private void OnCloseButtonTapped(object sender, EventArgs e)
        {
            var vm = BindingContext as PasswordVM;
            vm.Cancelled = false;
            CloseAllPopup();
        }

        private void OnCancelButtonTapped(object sender, EventArgs e)
        {
            var vm = BindingContext as PasswordVM;
            vm.Cancelled = true;
            CloseAllPopup();
        }

        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();

            return false;
        }

        private async void CloseAllPopup()
        {
            var vm = BindingContext as PasswordVM;
            await Navigation.PopAllPopupAsync();
            DialogClosed?.Invoke(this, new PasswordEventArgs(vm));
        }

    }

    
}