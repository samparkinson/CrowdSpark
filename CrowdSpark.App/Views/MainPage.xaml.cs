using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.App.ViewModels;
using CrowdSpark.App.Views;
using CrowdSpark.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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
            
            RecentTab.Background = new SolidColorBrush(Colors.DimGray);
            ((MainPageViewModel)DataContext).RepopulateContentCommand.Execute("Recent");
            ProjectsListStackPanel.Visibility = Visibility.Visible;
            CategoriesListStackPanel.Visibility = Visibility.Collapsed;
        }

        //Navigate to the associated project page
        private void projectsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Convert arg into project
            var clickedProject = (ProjectViewModel)e.ClickedItem;

            Frame.Navigate(typeof(ProjectPage), clickedProject);
        }
        
        public void OptionsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Convert e to MenuOption
            var clickedOption = (MenuOption)e.ClickedItem;

            //get current page type
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
        }

        private async Task SignIn()
        {
            await new AuthenticationHelper(new Settings()).SignInAsync();
        }

        public void SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            // navigate to SearchResultPage
            Frame.Navigate(typeof(SearchPage), args.QueryText);
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserPage), null);
        }

        public void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddProjectPage), CommonAttributes.account);
        }

        //do UI stuff
        private void RecentTabButton_Click(object sender, RoutedEventArgs e)
        {
            CategoriesTab.Background = new SolidColorBrush(Colors.Transparent);
            RecentTab.Background = new SolidColorBrush(Colors.DimGray);
            ProjectsListStackPanel.Visibility = Visibility.Visible;
            CategoriesListStackPanel.Visibility = Visibility.Collapsed;
        }

        //do UI stuff
        private void CategoriesTabButton_Click(object sender, RoutedEventArgs e)
        {
            CategoriesTab.Background = new SolidColorBrush(Colors.DimGray);
            RecentTab.Background = new SolidColorBrush(Colors.Transparent); 
            ProjectsListStackPanel.Visibility = Visibility.Collapsed;
            CategoriesListStackPanel.Visibility = Visibility.Visible;
        }

        private void CategoriesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Convert arg into projects
            var clickedCategory = (CategoryDTO)e.ClickedItem;
            Frame.Navigate(typeof(SearchPage), clickedCategory.Name);
        }
    }
}
