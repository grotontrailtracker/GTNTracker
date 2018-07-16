﻿using GTNTracker.Services;
using GTNTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GTNTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WaypointCapture : BasePage
    {
        public WaypointCapture()
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.Instance.WaypointCaptureVM;
        }

        protected override void OnAppearing()
        {
            var vm = BindingContext as WaypointCaptureVM;
            if (vm != null)
            {
                vm.UpdateVM();
            }

            base.OnAppearing();
        }

    }
}