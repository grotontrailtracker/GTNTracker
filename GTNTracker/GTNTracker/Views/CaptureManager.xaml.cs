using System.Linq;
using GTNTracker.EventArguments;
using GTNTracker.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CaptureManager : BasePage
	{
		public CaptureManager ()
		{
			InitializeComponent ();

            BindingContext = ViewModelLocator.Instance.CaptureManagerVM;
		}

        protected override void OnAppearing()
        {
            var vm = BindingContext as CaptureManagerVM;
            if (vm != null)
            {
                vm.UpdateVM();
            }

            base.OnAppearing();
        }

        private void HandleEditDialogClosed(object sender, DialogEventArgs args)
        {
            var vm = args.ViewModel as EditCaptureVM;
            if (vm.IsCancelled)
            {
                return;
            }
            if (!vm.IsCancelled && !vm.IsSend)
            {
                // just update current waypoint then.
                UpdateWaypointData(vm);
                return;
            }

            EmailWaypointWithUpdate(vm);
        }

        private void UpdateWaypointData(EditCaptureVM capture)
        {
            var vm = BindingContext as CaptureManagerVM;
            var item = vm.Waypoints.FirstOrDefault(w => w.Id == capture.Id);
            if (item != null)
            {
                item.Description = capture.Description;
                item.TrailName = capture.TrailName;
                item.WaypointName = capture.WaypointName;
            }
        }

        private async void EmailWaypointWithUpdate(EditCaptureVM capture)
        {
            UpdateWaypointData(capture);

            var vm = BindingContext as CaptureManagerVM;

            var success = vm.EMailWaypoint(capture.Id);
            if (success == false)
            {
                await DisplayAlert("Error!", "Unable to email waypoint", "OK");
            }
            
        }

        private async void WaypointList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedWaypoint = e.Item as WaypointCaptureDataVM;
            //if (selectedWaypoint.IsEmailed)
            //{
            //    return; // already done!
            //}
            var newVM = new EditCaptureVM(selectedWaypoint);
            var dialog = new EditCapture();
            dialog.BindingContext = newVM;
            dialog.DialogClosed += HandleEditDialogClosed;
            await Navigation.PushPopupAsync(dialog);
        }
    }
}