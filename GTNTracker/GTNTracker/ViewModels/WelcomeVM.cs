using System.Collections.Generic;
using System.Linq;
using GTNTracker.EventArguments;
using GTNTracker.Services;

namespace GTNTracker.ViewModels
{
    public class WelcomeVM : ViewModelBase
    {
        private bool _isTracking;
        private List<string> _currTrackingList = new List<string>();

        public WelcomeVM()
        {
            NotificationService.Instance.Tracking += HandleTrackingChange;
            foreach(var trail in NotificationService.Instance.CurrentTrackingList)
            {
                _currTrackingList.Add(trail);
            }

            if (_currTrackingList.Any())
            {
                IsTracking = true;
            }

            // just in case take a look to see if the Active trail is going
            var currActive = AppStateService.Instance.ActiveTrailId;
            if (!string.IsNullOrEmpty(currActive))
            {
                IsTracking = true;
                if (!_currTrackingList.Contains(currActive))
                {
                    _currTrackingList.Add(currActive);
                }
            }
        }

        public bool IsTracking
        {
            get => _isTracking;
            set => SetProperty(ref _isTracking, value);
           
        }

        private void HandleTrackingChange(object sender, TrackingEventArgs e)
        {
            if (e.MonitoringState)
            {
                if (!_currTrackingList.Any(t => t == e.TrailId))
                {
                    _currTrackingList.Add(e.TrailId);
                }
            }
            else
            {
                _currTrackingList.Remove(e.TrailId);
            }

            IsTracking = _currTrackingList.Any();
        }

    }
}
