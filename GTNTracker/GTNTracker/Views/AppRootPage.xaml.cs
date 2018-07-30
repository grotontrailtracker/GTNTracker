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
        private int _currentPageId;
        private int _priorPageId;

        public AppRootPage()
        {
            InitializeComponent();
            TheMasterPage.ListView.ItemSelected += ListView_ItemSelected;
            NotificationService.Instance.NavigateToPage += HandlePageNavigate;
            NotificationService.Instance.NavigatePrior += HandlePriorPageNavigate;
            _currentPageId = PageManager.TrailListPageId; //PageManager.WelcomePageId;
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

            _priorPageId = _currentPageId;
            _currentPageId = pageId;
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
                //if (PageManager.Instance.NavPageCache.ContainsKey(PageManager.WelcomePageId))
                //{
                //    navPage = PageManager.Instance.NavPageCache[PageManager.WelcomePageId];
                //}
                //else
                //{
                //    var page = (Page)Activator.CreateInstance(typeof(Welcome));
                //    page.Title = PageManager.Instance.GetPageTitle(PageManager.WelcomePageId);
                //    navPage = new NavigationPage(page);
                //    PageManager.Instance.NavPageCache[PageManager.WelcomePageId] = navPage;
                //}
                if (PageManager.Instance.NavPageCache.ContainsKey(PageManager.TrailListPageId))
                {
                    navPage = PageManager.Instance.NavPageCache[PageManager.TrailListPageId];
                }
                else
                {
                    var page = (Page)Activator.CreateInstance(typeof(TrailList));
                    page.Title = PageManager.Instance.GetPageTitle(PageManager.TrailListPageId);
                    navPage = new NavigationPage(page);
                    PageManager.Instance.NavPageCache[PageManager.TrailListPageId] = navPage;
                }
            }

            var tmpId = _currentPageId;
            _currentPageId = _priorPageId;
            _priorPageId = tmpId;

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

            if (item.Id == PageManager.StopGeoService)
            {
                //if (Device.RuntimePlatform == Device.Android)
                //{
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (GeoFenceService.IsRunning())
                    {
                        var result = await DisplayAlert("Stop GPS Service?", "Do you want to stop GPS Service?", "OK", "Cancel");
                        if (result)
                        {
                            // do something to go away.
                            MessagingCenter.Send(new StopGeofencing(), StopGeofencing.MessageString, new StopGeofencingArgs(false));
                            NotificationService.Instance.NotifyDevModeGeoServiceState(false);
                        }
                    }
                });

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

            navPage.Parent = null;
            Detail = navPage; //new NavigationPage(page);
            TheMasterPage.ListView.SelectedItem = null;
            IsPresented = false;

        }
    }

}
