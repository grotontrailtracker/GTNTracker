using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GTNTracker.Services;
using Xamarin.Forms;

namespace GTNTracker.Views
{
    public class BasePage : ContentPage
    {
        public IList<CustomToolbarItem> CustomToolbar { get; private set; }

        public BasePage()
        {
            var items = new ObservableCollection<CustomToolbarItem>();
            items.CollectionChanged += ToolbarItemsChanged;

            CustomToolbar = items;
        }

        private void ToolbarItemsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ToolbarItems.Clear();

            foreach (var item in CustomToolbar)
            {
                item.PropertyChanged += OnToolbarItemPropertyChanged;
                if (item.IsVisible)
                {
                    ToolbarItems.Add(item);
                }
            }
        }

        private void OnToolbarItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CustomToolbarItem.IsVisibleProperty.PropertyName)
            {
                UpdateToolbar();
            }
        }

        public void UpdateToolbar()
        {
            foreach (var item in CustomToolbar)
            {
                if (item.IsVisible)
                {
                    if (!ToolbarItems.Contains(item))
                    {
                        ToolbarItems.Add(item);
                    }
                }
                else
                {
                    if (ToolbarItems.Contains(item))
                    {
                        ToolbarItems.Remove(item);
                    }
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            NotificationService.Instance.NotifyNavigatePriorPage();
            return true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //ToolbarItems.Clear();
            //CustomToolbar.Clear();
            //foreach (var item in CustomToolbar)
            //{
            //    item.PropertyChanged -= OnToolbarItemPropertyChanged;
            //}
        }
    }

    public class CustomToolbarItem : ToolbarItem
    {
        public static readonly BindableProperty IsVisibleProperty =
                BindableProperty.Create(nameof(IsVisible),
                                        typeof(bool),
                                        typeof(CustomToolbarItem),
                                        true);
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }
    }
}
