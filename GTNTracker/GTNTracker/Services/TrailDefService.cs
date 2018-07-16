using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GTNTracker.Models;
using GTNTracker.Types;
using Newtonsoft.Json;
using PCLStorage;

namespace GTNTracker.Services
{
    /// <summary>
    /// Provides service to read in trail definitions from either the embedded resource DefaultTrailDef.json or from
    /// a file downloaded into the device - i.e. someday, make a http request and then persist the data.
    /// The json format for a trail should be:
    /// 
    ///{
    ///  "Identifier": "MattyHouse",
    ///  "Name": "Matt's Test",
    ///  "Description": "Test Trail Description",
    ///  "Regions": [
    ///    {
    ///      "Identifier": "MattyHouseLoc",
    ///      "TrailIdentifier": "MattyHouse",
    ///      "Name": "Matt's House",
    ///      "Description": "This is a test description for Matt's House. We need to test a very long description, so let's add some more information. I live with my best friend Allie who's my friendly yellow lab.",
    ///      "ImageName": "GTNTracker.Images.badge-photo.jpg",
    ///      "IsImageNameURI": false,
    ///      "Center": {
    ///        "Latitude": 42.60988875,
    ///        "Longitude": -71.59536214
    ///      },
    ///      "Radius": {
    ///        "TotalMiles": 0.03106855,
    ///        "TotalMeters": 50.0,
    ///        "TotalKilometers": 0.05
    ///      }
    ///    }
    ///  ]
    ///}
    ///
    /// Note that Radius does not have to be defined, if it isn't then the default located in the AppStateService will be
    /// applied to that trail region. It could come in handy if there is a gps location that due to cell tower coverage
    /// is not very accurate so a new larger geofence bubble could be set for that waypoint.
    /// 
    /// </summary>
    public class TrailDefService
    {
        private static TrailDefService _instance;

        private List<TrailDefModel> _trailList = new List<TrailDefModel>();

        private bool _isStarted = false;
        private bool _useDefaultDef = true;

        public static TrailDefService Instance => _instance ?? (_instance = new TrailDefService());

        public event EventHandler<TrailDefsUpdatedArgs> TrailDefsUpdated;

        public bool UseDefaults
        {
            get => _useDefaultDef;
            set => _useDefaultDef = value;
        }

        public TrailDefService()
        {
        }

        public async void Start()
        {
            if (!_isStarted)
            {
                _isStarted = true;
                await RestoreTrailDefinitions();
                Debug.WriteLine("=====>>>>> RegionDefService Start completed");
            }
        }

        public IEnumerable<GeofenceRegion> GetRegionDefinition(string trailId)
        {
            List<GeofenceRegion> trailList;

            var trailDef = _trailList.FirstOrDefault(t => t.Identifier == trailId);
            if (trailDef != null)
            {
                trailList = trailDef.Regions != null ? trailDef.Regions : new List<GeofenceRegion>();
            }
            else
            {
                trailList = new List<GeofenceRegion>();
            }

            return trailList;
        }

        public IEnumerable<TrailDefModel> TrailDefinitions => _trailList;

        public bool IsStarted => _isStarted;

        private async Task RestoreTrailDefinitions()
        {
            var result = await ReadFileContent("GTNTrailDef.json");
            if (!string.IsNullOrEmpty(result) && !_useDefaultDef)
            {
                _trailList = JsonConvert.DeserializeObject<List<TrailDefModel>>(result);
            }
            else
            {
                // fall back to initializing from data here.
                InitializeWithDefaults();
            }

            var bubbleRadius = new Distance();
            bubbleRadius.TotalKilometers = AppStateService.Instance.BubbleRadiusKm;

            // finally adjust the bubble radius values on all the regions
            foreach (var tModel in _trailList)
            {
                if (tModel.Regions != null)
                {
                    foreach (var region in tModel.Regions)
                    {
                        if (region.Radius.TotalKilometers == 0)
                        {
                            region.Radius = bubbleRadius;
                        }
                    }
                }
            }

            Debug.WriteLine("===>>>>> All trail definitions have been read!!!");
            TrailDefsUpdated?.Invoke(this, new TrailDefsUpdatedArgs(_trailList));
        }

        private void InitializeWithDefaults()
        {
            // first check if we can access the distributed definition
            var defStr = this.ReadDefaultsFile();
            if (!string.IsNullOrEmpty(defStr))
            {
                _trailList = JsonConvert.DeserializeObject<List<TrailDefModel>>(defStr);

                // finally, write out the data
                if (!_useDefaultDef)
                {
                    WriteFileData();
                }
            }
            else
            {
                Debug.WriteLine("Error!!! ===> Unable to access the default trail definitions!");
            }
        }

        /// <summary>
        /// Note that it is stored in android at data/data/GTNTracker.Android/files/GrotonTrails/GtnTrailData.json
        /// </summary>
        private async void WriteFileData()
        {
            try
            {
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFolder folder = await rootFolder.CreateFolderAsync("GrotonTrails", CreationCollisionOption.OpenIfExists);

                IFile file = await folder.CreateFileAsync("GTNTrailDef.json", CreationCollisionOption.ReplaceExisting);

                var trailDefString = JsonConvert.SerializeObject(_trailList);
                await file.WriteAllTextAsync(trailDefString);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("===>Error during writing: {0}", ex.Message);
            }
        }


        public async Task<string> ReadFileContent(string fileName)
        {
            string text = null;
            try
            {
                IFolder rootFolder = FileSystem.Current.LocalStorage;

                ExistenceCheckResult folderExists = await rootFolder.CheckExistsAsync("GrotonTrails");
                if (folderExists == ExistenceCheckResult.FolderExists)
                {
                    IFolder dataFolder = await rootFolder.GetFolderAsync("GrotonTrails");
                    ExistenceCheckResult exist = await dataFolder.CheckExistsAsync(fileName);

                    if (exist == ExistenceCheckResult.FileExists)
                    {
                        IFile file = await dataFolder.GetFileAsync("GTNTrailDef.json");
                        text = await file.ReadAllTextAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("===>Error during reading: {0}", ex.Message);
            }

            return text;
        }

        private string ReadDefaultsFile()
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("GTNTracker.Data.DefaultTrailDef.json");
            string text = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }
    }

    public class TrailDefsUpdatedArgs: EventArgs
    {
        public TrailDefsUpdatedArgs(List<TrailDefModel> trails)
        {
            TrailList = trails;
        }
        public List<TrailDefModel> TrailList { get; set; }
    }
}
