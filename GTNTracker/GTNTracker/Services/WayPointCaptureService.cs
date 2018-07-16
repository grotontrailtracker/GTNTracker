using GTNTracker.Models;
using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTNTracker.Services
{
    public class WayPointCaptureService
    {
        private static WayPointCaptureService _instance;
        private List<WaypointCaptureData> _waypointSet = new List<WaypointCaptureData>();
        private int _idCounter = 1;
        private ISettings _settingsPlugin;
        private string _settingKey = "gtnCaptureList";

        public static WayPointCaptureService Instance => _instance ?? (_instance = new WayPointCaptureService());

        private WayPointCaptureService()
        {
            _settingsPlugin = CrossSettings.Current;
            Initialize();
        }

        public void AddWaypoint(WaypointCaptureData data)
        {
            data.Id = _idCounter++;
            _waypointSet.Add(data);
            SaveCapturedWaypoints();
        }

        public void UpdateWaypoint(WaypointCaptureData data)
        {
            var waypoint = _waypointSet.FirstOrDefault(w => w.Id == data.Id);
            if (waypoint != null)
            {
                var index = _waypointSet.IndexOf(waypoint);
                _waypointSet[index] = data;
                SaveCapturedWaypoints();
            }
        }

        public void RemoveAllWaypoints()
        {
            _waypointSet.Clear();
            _settingsPlugin.Remove(_settingKey);
        }

        public IEnumerable<WaypointCaptureData> WaypointData => _waypointSet;

        private void Initialize()
        {
            var data = _settingsPlugin.GetValueOrDefault(_settingKey, string.Empty);
            if (!string.IsNullOrEmpty(data))
            {
                var obj = JsonConvert.DeserializeObject<List<WaypointCaptureData>>(data);
                if (obj != null)
                {
                    _waypointSet = obj;
                    // adjust id counter to not bump into these loaded ones.
                    var max = _waypointSet.Max(w => w.Id);
                    _idCounter = max + 1;
                }
            }
        }
        private void SaveCapturedWaypoints()
        {
            var data = JsonConvert.SerializeObject(_waypointSet);
            _settingsPlugin.AddOrUpdateValue(_settingKey, data);
        }
    }
}
