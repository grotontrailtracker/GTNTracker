using GTNTracker.Interfaces;
using GTNTracker.Models;
using GTNTracker.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class CaptureManagerVM : ViewModelBase
    {
        private ObservableCollection<WaypointCaptureDataVM> _waypoints = new ObservableCollection<WaypointCaptureDataVM>();
        private int _waypointIdBeingEmailed = -1;
        private ICommand _deleteAllWaypoints;

        public ObservableCollection<WaypointCaptureDataVM> Waypoints => _waypoints;

        public CaptureManagerVM()
        {
            MessagingCenter.Subscribe<WaypointEmailed, WaypointEMailedArgs>(this, WaypointEmailed.MessageString,
                    (sender, args) => { HandleWaypointEMailed(args); });
            DeleteAllWaypoints = new Command(HandleDeleteAllWaypoints, CanDeleteAllWaypoints);
        }

        public ICommand DeleteAllWaypoints
        {
            get => _deleteAllWaypoints;
            set => SetProperty(ref _deleteAllWaypoints, value);
        }

        public void UpdateVM()
        {
            _waypoints.Clear();

            foreach (var wp in WayPointCaptureService.Instance.WaypointData)
            {
                _waypoints.Add(new WaypointCaptureDataVM(wp));
            }

            RefreshCommands();
        }

        public bool EMailWaypoint(int id)
        {
            if (_waypointIdBeingEmailed > 0)
            {
                return false;   // already emailing one of the waypoints, wait until we're ready again
            }

            var waypointToEmail = _waypoints.FirstOrDefault(w => w.Id == id);
            if (waypointToEmail != null)
            {
                waypointToEmail.IsEmailed = true;

                var data = JsonConvert.SerializeObject(new WaypointCaptureDataEmail(waypointToEmail.CaptureData));
                DependencyService.Get<IEmailService>().EmailWaypoint(data, waypointToEmail.FilePath, id);

                _waypointIdBeingEmailed = id;

                WayPointCaptureService.Instance.UpdateWaypoint(waypointToEmail.CaptureData);
            }
            else
            {
                return false;
            }

            return true;
        }

        private void RemoveWaypoint(int id)
        {
            var wp = _waypoints.FirstOrDefault(w => w.Id == id);
            if (wp != null)
            {
                _waypoints.Remove(wp);
                DependencyService.Get<IUtilityService>().DeleteFile(wp.FilePath);
            }
        }

        private void HandleWaypointEMailed(WaypointEMailedArgs args)
        {
            _waypointIdBeingEmailed = -1;
            RefreshCommands();
        }

        private bool CanDeleteAllWaypoints()
        {
            return (_waypoints.Count() > 0 && _waypointIdBeingEmailed < 0);
        }

        private async void HandleDeleteAllWaypoints()
        {
            // confirm that the user really wants to flush everything!
            var result = await Application.Current.MainPage.DisplayAlert("Delete", "Are you sure you want to delete all captures?", "Yes", "No");
            if (result)
            {
                var ids = _waypoints.Select(w => w.Id).ToList();
                foreach (var id in ids)
                {
                    RemoveWaypoint(id);
                }

                WayPointCaptureService.Instance.RemoveAllWaypoints();

                RefreshCommands();
            }
        }

        private void RefreshCommands()
        {
            ((Command)DeleteAllWaypoints).ChangeCanExecute();
        }
    }
}
