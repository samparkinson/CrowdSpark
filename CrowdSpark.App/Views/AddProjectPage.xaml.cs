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
            Frame.Navigate(typeof(SearchPage), args.QueryText);
        }

        public void PostProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var checkList = new List<string>();
            //should implement a null checker for these
            var ProjectTitle = TitleTextBox.Text; checkList.Add(ProjectTitle);
            var ProjectDescription = DescriptionTextBox.Text; checkList.Add(ProjectDescription);
            var ProjectCategoryText = categoryAutoSuggestBox.Text; checkList.Add(ProjectCategoryText);

            var ProjectCountry = default(string);
            if (CountryComboBox.SelectedItem != null)
            {
                ProjectCountry = CountryComboBox.SelectedItem.ToString(); checkList.Add(ProjectCountry);
            }
            var ProjectCity = default(string);
            if (CityComboBox.SelectedItem != null)
            {
                ProjectCity = CityComboBox.SelectedItem.ToString(); checkList.Add(ProjectCity);
            }

            var ProjectLocation = new LocationDTO { Country = ProjectCountry, City = ProjectCity };
            var ProjectCategory = new CategoryDTO { Name = ProjectCategoryText };

            //TODO:needs work
            var SparkList = new List<SparkDTO>();
            SparkList.Add(new SparkDTO());

            foreach (var s in checkList)
            {
                if (String.IsNullOrEmpty(s))
                {
                    return;
                }
            }
            var projectDTO = new CreateProjectDTO
            {
                Title = ProjectTitle,
                Description = ProjectDescription,
                Location = ProjectLocation,
                Skills = SkillsList,
                Category = ProjectCategory
            };

            //TODO:Use the ProjectLogic class to post the project
            ((AddProjectPageViewModel)DataContext).PostProjectCommand.Execute(projectDTO);

        }
        private string[] SkillsSuggestions = new string[] { "Music", "Guitar", "Piano" };

        private void skillsAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //CategorySuggestions.Clear();

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
                categoryAutoSuggestBox.Text = sender.Text;
                SkillsList.Add(new SkillDTO { Name = sender.Text });
            }

            AutoSuggestBox suggestBox = new AutoSuggestBox();
            suggestBox.PlaceholderText = "TYPE IN A SKILL";
            suggestBox.HorizontalAlignment = HorizontalAlignment.Stretch;

            suggestBox.Margin = new Thickness(15,0,15,15);
           
            suggestBox.TextChanged += skillsAutoSuggestBox_TextChanged;
            suggestBox.SuggestionChosen += skillsAutoSuggestBox_SuggestionChosen;
            suggestBox.QuerySubmitted += skillsAutoSuggestBox_QuerySubmitted;

            SkillsPanel.Children.Add(suggestBox);
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
                //CategoryList.Add(new CategoryDTO { Name = sender.Text });
            }
        }

        private void categoryAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        
    }

}
