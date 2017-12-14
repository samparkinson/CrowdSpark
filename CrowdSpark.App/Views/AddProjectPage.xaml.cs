using CrowdSpark.App.Helpers;
using CrowdSpark.App.ViewModels;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CrowdSpark.App.Views
{
    public sealed partial class AddProjectPage : Page, IAppPage
    {
        private readonly AddProjectPageViewModel _vm;
        private List<Skill> SkillsList { get; set; }

        public AddProjectPage()
        {
            InitializeComponent();

            _vm = App.ServiceProvider.GetService<AddProjectPageViewModel>();
            
            DataContext = _vm;

            SkillsList = new List<Skill>();
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
            //should implement a null checker for these
            var ProjectTitle = TitleTextBox.Text;
            var ProjectDescription = DescriptionTextBox.Text;
            var ProjectCategoryText = CategoryTextBox.Text;
            var ProjectCountry = CountryComboBox.SelectedItem.ToString();
            var ProjectCity = CityComboBox.SelectedItem.ToString();
            Location ProjectLocation = new Location { Country = ProjectCountry, City = ProjectCity };
            var ProjectCategory = new Category { Name = ProjectCategoryText};

            //TODO:needs work
            var SparkList = new List<Spark>();
            SparkList.Add(new Spark());
            
            var projectDTO = new ProjectDTO { Title = ProjectTitle, Description = ProjectDescription,
                Location = ProjectLocation, Skills = SkillsList, Category = ProjectCategory, CreatedDate = DateTime.Now, Sparks = SparkList};

            //TODO:Use the ProjectLogic class to post the project
            //await ProjectLogic().CreateAsync(projectDTO);
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
                        SkillsList.Add(new Skill { Name = text});
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
                //set up and add a new textbox
                TextBox textBox = new TextBox();
                textBox.PlaceholderText = "TYPE IN A SKILL";
                textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                textBox.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);

                SkillsPanel.Children.Add(textBox);
            }
        }
    }
}
