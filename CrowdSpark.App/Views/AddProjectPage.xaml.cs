using CrowdSpark.App.Helpers;
using CrowdSpark.App.ViewModels;
using CrowdSpark.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private List<SkillDTO> SkillsList { get; set; }
        private List<CategoryDTO> CategoryList { get; set; }

        public AddProjectPage()
        {
            InitializeComponent();

            _vm = App.ServiceProvider.GetService<AddProjectPageViewModel>();
            
            DataContext = _vm;

            SkillsList = new List<SkillDTO>();
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
            var ProjectCategoryText = categoryAutoSuggestBox.Text;
            var ProjectCountry = CountryComboBox.SelectedItem.ToString();
            var ProjectCity = CityComboBox.SelectedItem.ToString();
            var ProjectLocation = new LocationDTO { Country = ProjectCountry, City = ProjectCity };
            var ProjectCategory = new CategoryDTO { Name = ProjectCategoryText};
            
            //TODO:needs work
            var SparkList = new List<SparkDTO>();
            SparkList.Add(new SparkDTO());
            
            var projectDTO = new CreateProjectDTO { Title = ProjectTitle, Description = ProjectDescription,
                Location = ProjectLocation, Skills = SkillsList, Category = ProjectCategory};
            
            //TODO:Use the ProjectLogic class to post the project
            ((AddProjectPageViewModel)DataContext).PostProjectCommand.Execute(projectDTO);
        }

        public void AddSkill_Click(object sender, RoutedEventArgs e)
        {
            AutoSuggestBox autoSuggestionBox = new AutoSuggestBox();
            autoSuggestionBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            autoSuggestionBox.Margin = new Thickness(15,0,15,15);
            autoSuggestionBox.PlaceholderText = "TYPE IN A SKILL";
            autoSuggestionBox.TextChanged += skillsAutoSuggestBox_TextChanged;
            autoSuggestionBox.QuerySubmitted += skillsAutoSuggestBox_QuerySubmitted;
            autoSuggestionBox.SuggestionChosen += skillsAutoSuggestBox_SuggestionChosen;

            SkillsPanel.Children.Add(autoSuggestionBox);
        }
   
        private string[] SkillsSuggestions = new string[] { "Music", "Dance", "Sing", "Piano" };

        private void skillsAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var Suggestion = SkillsSuggestions.Where(p => p.StartsWith(sender.Text, StringComparison.OrdinalIgnoreCase)).ToArray();
                sender.ItemsSource = Suggestion;
            }
        }

        private void skillsAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
                sender.Text = args.ChosenSuggestion.ToString();
            else
            {
                skillsAutoSuggestBox.Text = sender.Text;
                SkillsList.Add(new SkillDTO { Name = sender.Text });
            }
        }

        private void skillsAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        private string[] CategorySuggestions = new string[] { "Apple", "Banana", "Orange", "Strawberry" };

        private void categoryAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        { 
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //CategorySuggestions.Clear();

                var Suggestion = CategorySuggestions.Where(p => p.StartsWith(sender.Text, StringComparison.OrdinalIgnoreCase)).ToArray();
                sender.ItemsSource = Suggestion;
            }
        }

        private void categoryAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
                sender.Text = args.ChosenSuggestion.ToString();
            else
            {
                categoryAutoSuggestBox.Text = sender.Text;
                CategoryList.Add(new CategoryDTO { Name = sender.Text });
            }
        }

        private void categoryAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }
    }


}
