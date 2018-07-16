using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using GTNTracker.Models;

namespace GTNTracker.Services
{
    public class VisitBackupData
    {
        public List<TrailRegionVisit> Visits { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class VisitsUpdatedArgs: EventArgs
    {
        public VisitsUpdatedArgs(bool updated)
        {
            Updated = updated;
        }

        public bool Updated { get; set; }
    }

    public class TrailVisitService
    {
        private List<TrailRegionVisit> _allVisits = new List<TrailRegionVisit>();
        private static TrailVisitService _instance;
        private ISettings _settingsPlugin;

        private const string _settingsName = "gtnVisits";
        private const string _settingsBackupName = "gtnVisitsBackup";

        public event EventHandler<VisitsUpdatedArgs> VisitsUpdated;
        public static TrailVisitService Instance => _instance ?? (_instance = new TrailVisitService());

        private TrailVisitService()
        {
            _settingsPlugin = CrossSettings.Current;

            //TestJson();

            Initialize();
        }

        public IEnumerable<TrailRegionVisit> GetVisits(string trailId)
        {
            return _allVisits.Where(v => v.TrailIdentifer == trailId);
        }

        public void ClearSettings()
        {
            if (_settingsPlugin != null)
            {
                // first make a backup
                if (_allVisits.Any())
                {
                    var backup = new VisitBackupData()
                    {
                        Visits = _allVisits,
                        Timestamp = DateTime.Now
                    };

                    var data = JsonConvert.SerializeObject(backup);
                    _settingsPlugin.AddOrUpdateValue(_settingsBackupName, data);
                }

                _settingsPlugin.Remove(_settingsName);
                _allVisits.Clear();
                VisitsUpdated?.Invoke(this, new VisitsUpdatedArgs(true));
            }
        }

        public void RecoverBackupVisitData()
        {
            var jsonData = _settingsPlugin.GetValueOrDefault(_settingsBackupName, string.Empty);
            if (!string.IsNullOrEmpty(jsonData))
            {
                var backup = JsonConvert.DeserializeObject<VisitBackupData>(jsonData);
                if (backup != null)
                {
                    _allVisits = backup.Visits;
                    var dataStr = JsonConvert.SerializeObject(_allVisits);
                    _settingsPlugin.AddOrUpdateValue(_settingsName, dataStr);

                    VisitsUpdated?.Invoke(this, new VisitsUpdatedArgs(true));
                }
            }
        }

        public DateTime? IsBackupAvailable()
        {
            var val = _settingsPlugin.GetValueOrDefault(_settingsBackupName, string.Empty);
            if (!string.IsNullOrEmpty(val))
            {
                var backup = JsonConvert.DeserializeObject<VisitBackupData>(val);
                if (backup != null)
                {
                    return backup.Timestamp;
                }
            }

            return null;
        }

        public void AddTrailVisit(TrailRegionVisit visit)
        {
            if (!_allVisits.Any(v => v.RegionIdentifier == visit.RegionIdentifier))
            {
                _allVisits.Add(visit);
                var dataStr = JsonConvert.SerializeObject(_allVisits);
                _settingsPlugin.AddOrUpdateValue(_settingsName, dataStr);
            }
        }

        public void LoadTestData()
        {
            // really, the best way to do this is to cycle through every trail def and automatically
            // add a visit for it.
            foreach (var trailDef in TrailDefService.Instance.TrailDefinitions)
            {
                if (trailDef.Regions != null)
                {
                    foreach (var region in trailDef.Regions)
                    {
                        if (!_allVisits.Any(v => v.RegionIdentifier == region.Identifier
                                                 && v.TrailIdentifer == trailDef.Identifier))
                        {
                            _allVisits.Add(new TrailRegionVisit()
                            {
                                TrailIdentifer = trailDef.Identifier,
                                RegionIdentifier = region.Identifier,
                                Completed = DateTime.Now
                            });
                        }
                    }
                }
            }

            var dataStr = JsonConvert.SerializeObject(_allVisits);
            _settingsPlugin.AddOrUpdateValue(_settingsName, dataStr);

            VisitsUpdated?.Invoke(this, new VisitsUpdatedArgs(true));
        }

        private void Initialize()
        {
            var val = _settingsPlugin.GetValueOrDefault(_settingsName, string.Empty);
            var obj = JsonConvert.DeserializeObject<List<TrailRegionVisit>>(val);
            if (obj != null)
            {
                _allVisits = obj;
            }
        }

        private void TestJson()
        {
            List<TrailRegionVisit> visits = new List<TrailRegionVisit>();
            _allVisits.Add(new TrailRegionVisit()
            {
                TrailIdentifer = "GrotonDogPark",
                RegionIdentifier = "start dog park trail",
                Completed = DateTime.Now - TimeSpan.FromHours(3)
            });
            _allVisits.Add(new TrailRegionVisit()
            {
                TrailIdentifer = "GrotonDogPark",
                RegionIdentifier = "first fork on trail",
                Completed = DateTime.Now - TimeSpan.FromHours(2.5)
            });
            //_allVisits.Add(new TrailRegionVisit()
            //{
            //    TrailIdentifer = "GrotonDogPark",
            //    RegionIdentifier = "bench on trail",
            //    Completed = DateTime.Now - TimeSpan.FromHours(1.5)
            //});
            //_allVisits.Add(new TrailRegionVisit()
            //{
            //    TrailIdentifer = "GrotonDogPark",
            //    RegionIdentifier = "trail intersection at river",
            //    Completed = DateTime.Now - TimeSpan.FromHours(1.0)
            //});
            //_allVisits.Add(new TrailRegionVisit()
            //{
            //    TrailIdentifer = "GrotonDogPark",
            //    RegionIdentifier = "dog fire pit",
            //    Completed = DateTime.Now - TimeSpan.FromHours(0.5)
            //});
            //_allVisits.Add(new TrailRegionVisit()
            //{
            //    TrailIdentifer = "GrotonDogPark",
            //    RegionIdentifier = "approaching end of loop",
            //    Completed = DateTime.Now
            //});

            var dataStr = JsonConvert.SerializeObject(_allVisits);
            _settingsPlugin.AddOrUpdateValue(_settingsName, dataStr);
        }
    }
}
