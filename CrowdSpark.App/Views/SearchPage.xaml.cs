using CrowdSpark.App.Helpers;
using CrowdSpark.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace CrowdSpark.App.Views
{
    public sealed partial class SearchPage : Page, IAppPage
    {
        private readonly SearchPageViewModel _vm;

        private string Query;

        public SearchPage()
        {
            InitializeComponent();

            _vm = App.ServiceProvider.GetService<SearchPageViewModel>();

            DataContext = _vm;
        }

        public void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
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

        public void SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            // navigate to SearchResultPage
            //Frame.Navigate(typeof(SearchPage), args.QueryText);

            var context = (SearchPageViewModel)DataContext;
            context.SearchProjects(args.QueryText);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Query = (string) e.Parameter;

            ((SearchPageViewModel)DataContext).Initialize(Query);

            var rootFrame = Window.Current.Content as Frame;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        }

        private void resultsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Convert arg into project
            var clickedProject = (ProjectViewModel)e.ClickedItem;

            Frame.Navigate(typeof(ProjectPage), clickedProject);
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddProjectPage), CommonAttributes.account);
        }

        private void ProjectsTabButton_Click(object sender, RoutedEventArgs e)
        {
            ((SearchPageViewModel)DataContext).SearchProjects(Query);
        }
    }
}
