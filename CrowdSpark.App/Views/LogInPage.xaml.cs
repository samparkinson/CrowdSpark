using CrowdSpark.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace CrowdSpark.App.Views
{
    public sealed partial class LogInPage : Page
    {

        private readonly LogInPageViewModel _vm;

        public LogInPage()
        {
            InitializeComponent();

            _vm = App.ServiceProvider.GetService<LogInPageViewModel>();

            DataContext = _vm;

            ((LogInPageViewModel)DataContext).InitCommand.Execute(null);
        }
    }
}
