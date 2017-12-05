using CrowdSpark.App.ViewModels;
using CrowdSpark.App.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CrowdSpark.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
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

            //ProjectViewModel
            Debug.WriteLine(clickedProject.Title);

            Frame.Navigate(typeof(ProjectPage), clickedProject);
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void mySearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            this.Frame.Navigate(typeof(MainPage), args.QueryText); // navigate to SearchResultPage
        }
    }
}
