using System;
using System.Diagnostics;
using GTNTracker.EventArguments;
using GTNTracker.Interfaces;
using GTNTracker.Services;
using GTNTracker.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppRootPage : MasterDetailPage
    {
        //private string _currentPageTitle;
        private int _currentPageId;
        private int _priorPageId;
        //private string _priorPageTitle;

        public AppRootPage()
        {
            InitializeComponent();
            TheMasterPage.ListView.ItemSelected += ListView_ItemSelected;
            NotificationService.Instance.NavigateToPage += HandlePageNavigate;
            NotificationService.Instance.NavigatePrior += HandlePriorPageNavigate;
            //_currentPageTitle = "Groton Trail Tracker";
            _currentPageId = PageManager.WelcomePageId;
            //_priorPageTitle = string.Empty;
            _priorPageId = -1;
            
        }

        //protected override bool OnBackButtonPressed()
        //{
        //    return true;
        //}

        private void HandlePageNavigate(object sender, NavigateEventArgs args)
        {
            var pageId = args.PageId;

            Page navPage = null;
            if (PageManager.Instance.NavPageCache.ContainsKey(pageId))
            {
                navPage = PageManager.Instance.NavPageCache[pageId];
            }
            else
            {
                var page = (Page)Activator.CreateInstance(args.PageType);
                page.Title = PageManager.Instance.GetPageTitle(pageId);
                navPage = new NavigationPage(page);
                PageManager.Instance.NavPageCache[pageId] = navPage;
            }

            //_priorPageTitle = _currentPageTitle;
            _priorPageId = _currentPageId;
            _currentPageId = pageId;
            //_currentPageTitle = pageTitle;
            Detail = navPage;
            IsPresented = false;
        }

        private void HandlePriorPageNavigate(object sender, EventArgs args)
        {
            Page navPage = null;
            if (_priorPageId >= 0 && PageManager.Instance.NavPageCache.ContainsKey(_priorPageId))
            {
                navPage = PageManager.Instance.NavPageCache[_priorPageId];
            }
            else
            {
                if (PageManager.Instance.NavPageCache.ContainsKey(PageManager.WelcomePageId))
                {
                    navPage = PageManager.Instance.NavPageCache[PageManager.WelcomePageId];
                }
                else
                {
                    var page = (Page)Activator.CreateInstance(typeof(Welcome));
                    page.Title = PageManager.Instance.GetPageTitle(PageManager.WelcomePageId);
                    navPage = new NavigationPage(page);
                    PageManager.Instance.NavPageCache[PageManager.WelcomePageId] = navPage;
                }
            }

            //var tmp = _currentPageTitle;
            var tmpId = _currentPageId;
            _currentPageId = _priorPageId;
            _priorPageId = tmpId;
            //_currentPageTitle = _priorPageTitle;
            //_priorPageTitle = tmp;

            Detail = navPage;
            IsPresented = false;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as AppMenuItemVM;
            if (item == null)
            {
                return;
            }

            if (item.Id == PageManager.StopTrackingId)
            {
                //if (Device.RuntimePlatform == Device.Android)
                //{
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (GeoFenceService.IsRunning())
                        {
                            var result = await DisplayAlert("Stop Tracking?", "Do you really want to stop waypoint tracking?", "OK", "Cancel");
                            if (result)
                            {
                                // do something to go away.
                                MessagingCenter.Send(new UIStopAllMonitoringRegions(), UIStopAllMonitoringRegions.MessageString);
                                AppStateService.Instance.ActiveTrailId = String.Empty;
                            }
                        }
                    });
                    //Android.OS.Process.KillProcess(Android.OS.Process.MyPid());                   
               // }

                TheMasterPage.ListView.SelectedItem = null;
                IsPresented = false;
                return;
            }

            var titleKey = item.Title;
            var pageId = item.Id;
            Page navPage = null;
            if (PageManager.Instance.NavPageCache.ContainsKey(pageId))
            {
                navPage = PageManager.Instance.NavPageCache[pageId];
            }
            else
            {
                try
                {
                    var page = (Page)Activator.CreateInstance(item.TargetType);
                    page.Title = item.Title;
                    navPage = new NavigationPage(page);
                    PageManager.Instance.NavPageCache[pageId] = navPage;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("exception {0}", ex.Message);
                }
            }

            _priorPageId = _currentPageId;
            _currentPageId = pageId;
            //_priorPageTitle = _currentPageTitle;
            //_currentPageTitle = titleKey;

            navPage.Parent = null;
            Detail = navPage; //new NavigationPage(page);
            TheMasterPage.ListView.SelectedItem = null;
            IsPresented = false;

        }
    }

}
