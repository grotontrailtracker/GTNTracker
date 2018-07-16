using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTNTracker.ViewModels;
using Xamarin.Forms;

namespace GTNTracker.Views
{
    public class PageManager
    {
        private static PageManager _instance;

        private CurrentStatusPopup _statusPage;
        private ImagePopup _imagePopup;
        private Dictionary<int, Page> _pageCache = new Dictionary<int, Page>();
        private Dictionary<int, string> _pageTitle = new Dictionary<int, string>();

        // every BasePage needs a page id for reference
        public static int WelcomePageId = 0;
        public static int TrailListPageId = 1;
        public static int TrailMapPageId = 3;
        public static int StatusPageId = 4;
        public static int SettingsPageId = 5;
        public static int AboutPageId = 10;
        public static int StopTrackingId = 75;
        public static int WaypointCaptureId = 40;
        public static int ManageCapturesId = 45;

        public static PageManager Instance => _instance ?? (_instance = new PageManager());
 
        public PageManager()
        {
        }

        public CurrentStatusPopup CurrentStatusPopup => _statusPage ?? (_statusPage = new CurrentStatusPopup());

        public ImagePopup ImagePopup => _imagePopup ?? (_imagePopup = new ImagePopup());

        public Dictionary<int, Page> NavPageCache => _pageCache;

        public void Clear()
        {
            _statusPage = null;
            _pageCache.Clear();
        }

        public void RegisterPageTitle(int pageId, string pageTitle)
        {
            if (!_pageTitle.ContainsKey(pageId))
            {
                _pageTitle.Add(pageId, pageTitle);
            }
        }

        public string GetPageTitle(int pageId)
        {
            var titleString = "Title Not Found";

            string outStr = string.Empty;
            if (_pageTitle.TryGetValue(pageId, out outStr))
            {
                titleString = outStr;
            }

            return titleString;
        }
    }
}
