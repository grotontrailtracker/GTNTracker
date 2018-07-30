using System.Collections.ObjectModel;
using System.Linq;
using GTNTracker.EventArguments;
using GTNTracker.Services;
using GTNTracker.Views;
using Xamarin.Forms;

namespace GTNTracker.ViewModels
{
    public class AppMenuVM : ViewModelBase
    {
        private ObservableCollection<AppMenuItemVM> _menuItems = new ObservableCollection<AppMenuItemVM>();

        public ObservableCollection<AppMenuItemVM> MenuItems
        {
            get => _menuItems;
            set => SetProperty(ref _menuItems, value);
        }

        public AppMenuVM()
        {
            NotificationService.Instance.AllowWaypointCaptureMode += HandleEnableCaptureMode;
        }

        public void Initialize()
        {
            //MenuItems.Add(new AppMenuItemVM()
            //{
            //    Id = PageManager.WelcomePageId,
            //    MenuTitle = "Welcome",
            //    Title = "Groton Trail Tracker",
            //    TargetType = typeof(Welcome),
            //    ShowImage = true,
            //    IconImage = ImageSource.FromResource("GTNTracker.Images.gtn.png")
            //});
            MenuItems.Add(new AppMenuItemVM()
            {
                Id = PageManager.TrailListPageId,
                MenuTitle = "Trails List",
                Title = "Groton Trails",
                TargetType = typeof(TrailList),
                ShowImage = true,
                IconImage = ImageSource.FromResource("GTNTracker.Images.terrain.png")
            });
            //MenuItems.Add(new AppMenuItemVM()
            //                {   Id = 2,
            //                    MenuTitle = "Tracking Status",
            //                    Title = "Trail Tracking Status",
            //                    TargetType = typeof(TrailStatus),
            //                    IconImage = ImageSource.FromResource("GTNTracker.Images.compass.png"),
            //                    ShowImage = true
            //                });
            MenuItems.Add(new AppMenuItemVM()
            {
                Id = PageManager.TrailMapPageId,
                MenuTitle = "Trails Map",
                Title = "Trails Map",
                TargetType = typeof(TrailMap),
                IconImage = ImageSource.FromResource("GTNTracker.Images.map.png"),
                ShowImage = true
            });
            MenuItems.Add(new AppMenuItemVM
            {
                Id = PageManager.StopTrackingId,
                MenuTitle = "Stop Trail Tracking",
                TargetType = typeof(AppDetailPage),
                IconImage = ImageSource.FromResource("GTNTracker.Images.cancel.png"),
                ShowImage = true,
                IsAvailable = false
            });
            //MenuItems.Add(new AppMenuItemVM()
            //{
            //    Id = PageManager.StatusPageId,
            //    MenuTitle = "Status (Debugging)",
            //    Title = "System Status",
            //    ImageFilename = string.Empty,
            //    IconImage = ImageSource.FromResource("GTNTracker.Images.UnderConstruct.png"),
            //    ShowImage = false,
            //    TargetType = typeof(Status)
            //});
            MenuItems.Add(new AppMenuItemVM()
            {
                Id = PageManager.SettingsPageId,
                MenuTitle = "Settings",
                Title = "Settings",
                TargetType = typeof(Settings),
                IconImage = ImageSource.FromResource("GTNTracker.Images.settings.png"),
                ShowImage = true,
                IsAvailable = true
            });
            MenuItems.Add(new AppMenuItemVM()
            {
                Id = PageManager.AboutPageId,
                MenuTitle = "About",
                Title = "About Trail Tracker",
                TargetType = typeof(AboutCarousel), //typeof(About),
                IconImage = ImageSource.FromResource("GTNTracker.Images.info.png"),
                ShowImage = true
            });
            if (AppSettingsService.Instance.AppSettings.AllowCapture && AppSettingsService.Instance.AppSettings.DeveloperMode)
            {
                AddWaypointCapturePages();
            }

            foreach (var item in MenuItems)
            {
                PageManager.Instance.RegisterPageTitle(item.Id, item.Title);
            }
        }

        private void AddWaypointCapturePages()
        {
            MenuItems.Add(new AppMenuItemVM()
            {
                Id = PageManager.WaypointCaptureId,
                MenuTitle = "Waypoint Capture",
                Title = "Waypoint Capture",
                TargetType = typeof(WaypointCapture),
                IconImage = ImageSource.FromResource("GTNTracker.Images.camera.png"),
                ShowImage = true
            });
            MenuItems.Add(new AppMenuItemVM()
            {
                Id = PageManager.StopGeoService,
                MenuTitle = "Stop GeoService",
                Title = "Stop GeoService",
                TargetType = typeof(AppDetailPage),
                IconImage = ImageSource.FromResource("GTNTracker.Images.cancel.png"),
                ShowImage = true,
                IsAvailable = false
            });
            MenuItems.Add(new AppMenuItemVM()
            {
                Id = PageManager.ManageCapturesId,
                MenuTitle = "Manage Captures",
                Title = "Manage Captures",
                TargetType = typeof(CaptureManager),
                IconImage = ImageSource.FromResource("GTNTracker.Images.collections.png"),
                ShowImage = true
            });

            PageManager.Instance.RegisterPageTitle(PageManager.WaypointCaptureId, "Waypoint Capture");
            PageManager.Instance.RegisterPageTitle(PageManager.StopGeoService, "Stop GeoService");
            PageManager.Instance.RegisterPageTitle(PageManager.ManageCapturesId, "Manage Captures");
        }

        private void RemoveWaypointCapturePages()
        {
            var capture = MenuItems.FirstOrDefault(m => m.Id == PageManager.WaypointCaptureId);
            if (capture != null)
            {
                MenuItems.Remove(capture);
            }
            var mgr = MenuItems.FirstOrDefault(m => m.Id == PageManager.ManageCapturesId);
            if (mgr != null)
            {
                MenuItems.Remove(mgr);
            }
        }

        private void HandleEnableCaptureMode(object sender, AllowCaptureModeArgs args)
        {
            // are we adding or removing?

            if (args.Enabled)
            {
                var item = MenuItems.FirstOrDefault(m => m.Id == PageManager.WaypointCaptureId);
                if (item == null)
                {
                    AddWaypointCapturePages();
                }
            }
            else
            {
                // need to remove the entries.
                RemoveWaypointCapturePages();
            }
        }
    }
}
