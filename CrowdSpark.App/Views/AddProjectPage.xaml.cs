using CrowdSpark.App.Helpers;
using CrowdSpark.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CrowdSpark.App.Views
{
    public sealed partial class AddProjectPage : Page, IAppPage
    {
        private readonly AddProjectPageViewModel _vm;

        public AddProjectPage()
        {
            InitializeComponent();

            _vm = App.ServiceProvider.GetService<AddProjectPageViewModel>();
            
            DataContext = _vm;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
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
            this.Frame.Navigate(typeof(SearchPage), args.QueryText);
        }

        public void PostProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var flag = true;
            var ProjectTitle = projectTitleTextBox.Text;
            if (ProjectTitle == "")
            {
                projectTitleTextBox.BorderThickness = new Thickness(2);
                projectTitleTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                flag = false;
            }
            else
            {
                projectTitleTextBox.BorderThickness = new Thickness(2);
                projectTitleTextBox.BorderBrush = new SolidColorBrush(Colors.Green);
            }
            var ProjectDescription = projectDescriptionTextBox.Text;
            if (ProjectDescription == "")
            {
                projectDescriptionTextBox.BorderThickness = new Thickness(2);
                projectDescriptionTextBox.BorderBrush = new SolidColorBrush(Colors.Red);
                flag = false;
            }
            else
            {
                projectDescriptionTextBox.BorderThickness = new Thickness(2);
                projectDescriptionTextBox.BorderBrush = new SolidColorBrush(Colors.Green);
            }
            var Country = CountryComboBox.SelectedItem;
            if (Country == null)
            {
                CountryComboBox.BorderBrush = new SolidColorBrush(Colors.Red);
                flag = false;
            }
            else
            {
                CountryComboBox.BorderBrush = new SolidColorBrush(Colors.Green);
            }
            var City = CityComboBox.SelectedItem;
            if (City == null)
            {
                CityComboBox.BorderBrush = new SolidColorBrush(Colors.Red);
                flag = false;
            }
            else
            {
                CityComboBox.BorderBrush = new SolidColorBrush(Colors.Green);
            }
            
            if (flag)
            {
                Debug.WriteLine("City: " + City);
                Debug.WriteLine("Country: " + Country);
                Debug.WriteLine("Description: " + ProjectDescription);
                Debug.WriteLine("Title: " + ProjectTitle);
                //send project details to relevant function
            }
        }
        
    }
}
