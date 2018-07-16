using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTNTracker.ViewModels
{
    public class ViewModelLocator
    {
        private static ViewModelLocator _instance;
        public static ViewModelLocator Instance => _instance ?? (_instance = new ViewModelLocator());

        private CurrentLocationVM _currLocationVM;
        private TrailListViewModel _trailListVM;
        private ImageVM _imageVM;
        private WelcomeVM _welcomeVM;
        private TrailContentViewModel _activeTrailContentVM;
        private WaypointCaptureVM _waypointCaptureVM;
        private CaptureManagerVM _captureManagerVM;

        private ViewModelLocator()
        {

        }

        public CurrentLocationVM CurrentLocationVM => _currLocationVM ?? (_currLocationVM = new CurrentLocationVM());

        public TrailListViewModel TrailListViewModel => _trailListVM ?? (_trailListVM = new TrailListViewModel());

        public WelcomeVM WelcomeVM => _welcomeVM ?? (_welcomeVM = new WelcomeVM());

        public ImageVM ImageVM => _imageVM ?? (_imageVM = new ImageVM());

        public TrailContentViewModel ActiveTrailContentVM
        {
            get => _activeTrailContentVM;
            set => _activeTrailContentVM = value;
        }

        public WaypointCaptureVM WaypointCaptureVM => _waypointCaptureVM ?? (_waypointCaptureVM = new WaypointCaptureVM());

        public CaptureManagerVM CaptureManagerVM => _captureManagerVM ?? (_captureManagerVM = new CaptureManagerVM());

        public void Initialize()
        {
            ResetAllVMs();  // takes care of the case where we reenter the code at the OnStart via notification intent

            var locVM = CurrentLocationVM;
            var trailVM = TrailListViewModel;
            var welcomeVM = WelcomeVM;
            var waypointCaptureVM = WaypointCaptureVM;
            var captureManagerVM = CaptureManagerVM;
        }

        private void ResetAllVMs()
        {
            _currLocationVM = null;
            _trailListVM = null;
            _welcomeVM = null;
            _activeTrailContentVM = null;
            _waypointCaptureVM = null;
            _captureManagerVM = null;
        }
    }
}
