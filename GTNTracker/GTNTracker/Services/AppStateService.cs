using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTNTracker.Services
{
    public class AppStateService
    {
        private static AppStateService _instance;
        private string _versionNumber = "1.0.0";
        private string _testIISHost;
        private string _activeTrailIdTrack;
        private double _declination = -14.5;
        private double _bubbleRadiusKilometers = 0.025; //0.05;
        private string _emailAddress = "grotontrailtracker@gmail.com";

        public static AppStateService Instance => _instance ?? (_instance = new AppStateService());

        private AppStateService()
        {
        }

        public bool IsAppAwake
        {
            get; set;
        }

        public string AppVersion => _versionNumber;

        public double Declination => _declination;

        public double BubbleRadiusKm => _bubbleRadiusKilometers;

        public string TestIISHost
        {
            get => _testIISHost;
            set => _testIISHost = value; 
        }

        public string ActiveTrailId
        {
            get => _activeTrailIdTrack;
            set => _activeTrailIdTrack = value;
        }

        public bool IsNotificationLaunch
        {
            get; set;
        }

        public double WindowHeight { set; get; }

        public double WindowWidth { set; get; }
 

        public string EmailAddress => _emailAddress;
    }
}
