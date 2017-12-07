using CrowdSpark.App.Helpers;
using CrowdSpark.App.ViewModels;
using CrowdSpark.App.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CrowdSpark.App
{
    public sealed partial class MainPage : Page, IAppPage
    {

        private readonly MainPageViewModel _vm;

        public MainPage()
        { 
            InitializeComponent();

            _vm = App.ServiceProvider.GetService<MainPageViewModel>();

            DataContext = _vm;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        }

        private void projectsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Convert arg into project
            var clickedProject = (ProjectViewModel)e.ClickedItem;

            Frame.Navigate(typeof(ProjectPage), clickedProject);
        }

        //Handle hamburger menu open/close 
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }
        
        private void SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            // navigate to SearchResultPage
            this.Frame.Navigate(typeof(SearchPage), args.QueryText); 
        }
        
        void IAppPage.OptionsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Convert e to MenuOption
            var clickedOption = (MenuOption)e.ClickedItem;

            switch (clickedOption.Icon)
            {
                case "Account":
                    //this.Frame.Navigate(typeof(SearchPage), args.QueryText);
                    break;
                case "Page":
                    //MainPage doesn't need arguments
                    this.Frame.Navigate(typeof(MainPage), null);
                    break;
                case "Setting":
                    break;
                case "Message":
                    break;
            }

            Debug.WriteLine("Text: " + clickedOption.Text);
        }

        void IAppPage.SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
