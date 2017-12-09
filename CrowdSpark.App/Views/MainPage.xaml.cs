using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.App.ViewModels;
using CrowdSpark.App.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await _vm.Initialize();

            var rootFrame = Window.Current.Content as Frame;
            
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        }

        //Navigate to the associated project page
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
        
        public void OptionsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Convert e to MenuOption
            var clickedOption = (MenuOption)e.ClickedItem;

            var currentFrame = Window.Current.Content as Frame;
            var currentPage = currentFrame.SourcePageType;

            switch (clickedOption.Icon)
            {
                case "Account":
                    if (currentPage != typeof(UserPage))
                    {
                        //Navigate to user page, send account details
                        Frame.Navigate(typeof(UserPage), CommonAttributes.account);
                    }
                    break;
                case "Page":
                    if (currentPage != typeof(MainPage))
                    {
                        //MainPage doesn't need arguments
                        Frame.Navigate(typeof(MainPage));
                    }
                    break;
                case "Setting":
                    break;
                case "Message":
                    break;
            }

            //Debug.WriteLine("Text: " + clickedOption.Text);
        }

        private async Task SignIn()
        {
            await new AuthenticationHelper(new Settings()).SignInAsync();
        }

        void IAppPage.SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserPage), null);
        }
    }
}
