﻿using CrowdSpark.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CrowdSpark.App.Views
{
    /// <summary>
    /// Projects page which displays info in a more detailed way
    /// </summary>
    public sealed partial class ProjectPage : Page
    {
        private readonly ProjectPageViewModel _vm;

        public ProjectPage()
        {
            InitializeComponent();

            _vm = App.ServiceProvider.GetService<ProjectPageViewModel>();

            DataContext = _vm;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var _project = e.Parameter as ProjectViewModel;

            _vm.Initialize(_project);

            CountryFlagImage.Source = _vm.CountryFlag;

            var rootFrame = Window.Current.Content as Frame;
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void MenuOptionsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
