using CrowdSpark.App.Helpers;
using CrowdSpark.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CrowdSpark.App.Views
{
    public sealed partial class ProjectPage : Page, IAppPage
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
        
        
        private void SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            this.Frame.Navigate(typeof(MainPage), args.QueryText); // navigate to SearchResultPage
        }

        public void OptionsList_ItemClick(object sender, ItemClickEventArgs e)
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
