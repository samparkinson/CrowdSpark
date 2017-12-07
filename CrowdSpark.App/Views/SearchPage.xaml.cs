using CrowdSpark.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace CrowdSpark.App.Views
{
    public sealed partial class SearchPage : Page
    {
        private readonly SearchPageViewModel _vm;

        public SearchPage()
        {
            InitializeComponent();

            _vm = App.ServiceProvider.GetService<SearchPageViewModel>();

            DataContext = _vm;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var Query = (string) e.Parameter;

            _vm.Initialize(Query);

            var rootFrame = Window.Current.Content as Frame;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        }
    }
}
