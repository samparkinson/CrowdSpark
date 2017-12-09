using CrowdSpark.App.Helpers;
using CrowdSpark.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserPage : Page, IAppPage
    {
        private readonly UserPageViewModel _vm;

        public UserPage()
        {
            this.InitializeComponent();

            _vm = App.ServiceProvider.GetService<UserPageViewModel>();

            DataContext = _vm;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var _user = e.Parameter as UserViewModel;

            _vm.Initialize(_user);
            CountryFlagImage.Source = _vm.CountryFlag;

            var rootFrame = Window.Current.Content as Frame;
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
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

        public void SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
