using CrowdSpark.App.Helpers;
using CrowdSpark.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
        private List<string> SkillsList { get; set; }

        public AddProjectPage()
        {
            InitializeComponent();

            _vm = App.ServiceProvider.GetService<AddProjectPageViewModel>();
            
            DataContext = _vm;

            SkillsList = new List<string>();
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
            var ProjectTitle = projectTitleTextBox.Text;
            var ProjectDescription = projectDescriptionTextBox.Text;
            var Country = CountryComboBox.SelectedItem;
            var City = CityComboBox.SelectedItem;

            Debug.WriteLine("City: " + City);
            Debug.WriteLine("Country: " + Country);
            Debug.WriteLine("Description: " + ProjectDescription);
            Debug.WriteLine("Title: " + ProjectTitle);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var allFull = true;

            //reset the skill list everytime 
            SkillsList.Clear();

            foreach (var UIElement in SkillsPanel.Children)
            {
                if (UIElement is TextBox)
                {
                    var textBox = (TextBox)UIElement;
                    var text = textBox.Text;
                    if (!String.IsNullOrEmpty(text))
                    {
                        SkillsList.Add(text);
                        //set the border color back to normal
                        textBox.BorderBrush = null;
                    }
                    else
                    {
                        //if the current textbox is empty remove it 
                        if (sender.Equals(textBox))
                        {
                            SkillsPanel.Children.Remove(textBox);
                        }
                        allFull = false;
                        break;
                    }
                }
            }

            //add a new textbox if all the text boxes are filled in
            if (allFull)
            {
                TextBox textBox = new TextBox();
                textBox.PlaceholderText = "TYPE IN A SKILL";
                textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                textBox.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);

                SkillsPanel.Children.Add(textBox);
            }
        }
    }
}
