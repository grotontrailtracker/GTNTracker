using System.Linq;
using GTNTracker.Interfaces;
using GTNTracker.Types;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class StatusViewModel : ViewModelBase
    {
        private string _editorData;
        private Position _currPosition;

        public StatusViewModel()
        {
            MessagingCenter.Subscribe<GeofenceMonitoredRegions, GeofenceMonitoredRegionsArgs>(this, GeofenceMonitoredRegions.MessageString,
                                (sender, args) => { HandleGeofenceMonitoredRegionsMsg(args); });
            MessagingCenter.Subscribe<GeofenceCurrentPosition, GeofenceCurrentPositionArgs>(this, GeofenceCurrentPosition.MessageString,
                                (sender, args) => { HandleGeofenceCurrentPositionMsg(args); });
            MessagingCenter.Subscribe<GeofenceCurrentPositionString, string>(this, GeofenceCurrentPositionString.MessageString,
                                (sender, args) => { HandleGeofenceCurrentPositionStringMsg(args); });
        }

        public string EditorData
        {
            get => _editorData;
            set => SetProperty(ref _editorData, value);
        }


        public void CheckStatus()
        {
            MessagingCenter.Send<RequestCurrentPositionString>(new RequestCurrentPositionString(), RequestCurrentPositionString.MessageString);

            MessagingCenter.Send<RequestCurrentPosition>(new RequestCurrentPosition(), RequestCurrentPosition.MessageString);
        }

        private void HandleGeofenceCurrentPositionStringMsg(string posStr)
        {
            EditorData += string.Format("{0}\n", posStr);
        }

        private void HandleGeofenceCurrentPositionMsg(GeofenceCurrentPositionArgs args)
        {
            _currPosition = args.Position;
            MessagingCenter.Send<RequestMonitoredRegions>(new RequestMonitoredRegions(), RequestMonitoredRegions.MessageString);
        }

        private void HandleGeofenceMonitoredRegionsMsg(GeofenceMonitoredRegionsArgs args)
        {
            var regionList = args.Regions;
            if (_currPosition != null)
            {
                if (regionList.Any())
                {
                    foreach (var region in regionList)
                    {
                        var inRegion = region.IsPositionInside(_currPosition);
                        var inRegionStr = inRegion ? "In Region" : "Out of Region";
                        var distance = region.Center.GetDistanceTo(_currPosition);
                        var msg = string.Format("region: {0}, lat: {1}, lng: {2}, loc?: {3}, dist: {4:F2} meters",
                            region.Identifier, region.Center.Latitude, region.Center.Longitude, inRegionStr, distance.TotalMeters);
                        //Application.Current.MainPage.DisplayAlert("Current Status: ", msg, "OK");
                        EditorData += string.Format("Curr: {0}\n", msg);
                    }
                }
                else
                {
                    EditorData += "No monitored regions\n";
                }
            }
            else
            {
                EditorData += "No current position\n";
            }
        }
    }
}
