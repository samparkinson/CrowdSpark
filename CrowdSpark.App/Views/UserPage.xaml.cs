using CrowdSpark.App.Helpers;
using CrowdSpark.App.ViewModels;
using System;
using Windows.UI.Core;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Security.Credentials;
using CrowdSpark.Common;
using System.Collections.Generic;

namespace CrowdSpark.App.Views
{
    public sealed partial class UserPage : Page, IAppPage
    {
        private readonly UserPageViewModel _vm;
        private List<SkillCreateDTO> SkillsList { get; set; }

        public UserPage()
        {
            InitializeComponent();

            _vm = App.ServiceProvider.GetService<UserPageViewModel>();

            DataContext = _vm;

            SkillsList = new List<SkillCreateDTO>();

            UserCountryComboBox.ItemsSource = ((UserPageViewModel)DataContext).Countries;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if user not signed in
            if (e.Parameter == null)
            {
                ((UserPageViewModel)DataContext).SignInOutCommand.Execute(null);
            }
            
            ((UserPageViewModel)DataContext).Initialize((WebAccount)e.Parameter);

            var userLocation = ((UserPageViewModel)DataContext).Location;
            if (userLocation != null)
            {
                setCountryComboBoxSelection(userLocation.Country);
            }

            fillSkillsList();

            var rootFrame = Window.Current.Content as Frame;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        }
       
        public void OptionsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Convert e to MenuOption
            var clickedOption = (MenuOption)e.ClickedItem;

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
            Frame.Navigate(typeof(SearchPage), args.QueryText); // navigate to SearchResultPage
        }

        public void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddProjectPage), CommonAttributes.account);
        }

        private async void UpdateUserButton_Click(object sender, RoutedEventArgs e)
        {
            var checkList = new List<string>();
            var Name = UserNameTextBox.Text; checkList.Add(Name);
            var Surname = UserSurnameTextBox.Text; checkList.Add(Surname);
            var Mail = UserMailTextBlock.Text;
            var Country = default(string);
            if (UserCountryComboBox.SelectedItem != null)
            {
                Country = UserCountryComboBox.SelectedItem.ToString();

            }
            //checkList.Add(Country);
            var City = UserCityTextBox.Text; //checkList.Add(City);

            foreach (var s in checkList)
            {
                if (String.IsNullOrEmpty(s))
                {
                    ContentDialog fillAllFieldsDialog = new ContentDialog
                    {
                        Title = "Please fill all fields!",
                        CloseButtonText = "Ok"
                    };
                    await fillAllFieldsDialog.ShowAsync();
                    return;
                }
            }

            LocationDTO location = new LocationDTO { Country = Country, City = City };

            UserDTO UserDTO = new UserDTO { Firstname = Name, Surname = Surname, Mail = Mail, Location = location };

            var result = await ((UserPageViewModel)DataContext).UpdateUser(UserDTO, SkillsList);

            if (result)
            {
                UpdateButton.Content = "UPDATED!";
            }
            else
            {
                UpdateButton.Content = "TRY AGAIN LATER";
            }
        }

        private async void skillsAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput && !String.IsNullOrEmpty(sender.Text))
            {
                //dunno whats going on
                var skillDTOs = new List<SkillDTO>();
                
                skillDTOs = await ((UserPageViewModel)DataContext).GetSkillsAsync(sender.Text);
                
                var Suggestions = new List<string>();

                if (skillDTOs != null)
                {
                    foreach (var skillDTO in skillDTOs)
                    {
                        Suggestions.Add(skillDTO.Name);
                    }
                    Suggestions.Sort();
                }
                sender.ItemsSource = Suggestions;
            }
        }

        private void skillsAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
                sender.Text = args.ChosenSuggestion.ToString();
            else
                SkillsList.Add(new SkillCreateDTO { Name = sender.Text });

            addAutoSuggestBox(null);
        }

        private void addAutoSuggestBox(string optionalText)
        {
            //create a new AutoSuggestBox 
            AutoSuggestBox suggestBox = new AutoSuggestBox();

            if (optionalText != null)
                suggestBox.Text = optionalText;
            else
                suggestBox.PlaceholderText = "TYPE IN A SKILL";
            
            suggestBox.HorizontalAlignment = HorizontalAlignment.Stretch;

            suggestBox.Margin = new Thickness(15, 0, 15, 15);

            suggestBox.TextChanged += skillsAutoSuggestBox_TextChanged;
            suggestBox.SuggestionChosen += skillsAutoSuggestBox_SuggestionChosen;
            suggestBox.QuerySubmitted += skillsAutoSuggestBox_QuerySubmitted;

            SkillsPanel.Children.Add(suggestBox);
        }

        private void skillsAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
            SkillsList.Add(new SkillCreateDTO { Name = sender.Text });
        }

        private void setCountryComboBoxSelection(string Country)
        {
            foreach (var item in UserCountryComboBox.Items)
            {
                if (item.ToString().Equals(Country))
                {
                    UserCountryComboBox.SelectedItem = item;
                }
            }
        }

        private void fillSkillsList()
        {
            var skillCreateDTOs = ((UserPageViewModel)DataContext).populateSkillsList();

            foreach (var skillCreateDTO in skillCreateDTOs)
            {
                addAutoSuggestBox(skillCreateDTO.Name);
            }
        }
    }
}
