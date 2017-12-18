using CrowdSpark.App.Helpers;
using CrowdSpark.App.ViewModels;
using CrowdSpark.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
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
        private List<SkillCreateDTO> SkillsList = new List<SkillCreateDTO>();
        private int textBlockCount = 0;
        private List<TextBlock> addButtonTextBlockList = new List<TextBlock>();
        private List<StorageFile> attachments = new List<StorageFile>();

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
            Frame.Navigate(typeof(SearchPage), args.QueryText);
        }

        public async void PostProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var checkList = new List<string>();
            //should implement a null checker for these
            var ProjectTitle = TitleTextBox.Text; checkList.Add(ProjectTitle);
            var ProjectDescription = DescriptionTextBox.Text; checkList.Add(ProjectDescription);
            var ProjectCategoryText = categoryAutoSuggestBox.Text; checkList.Add(ProjectCategoryText);

            var ProjectCountry = default(string);
            if (CountryComboBox.SelectedItem != null)
            {
                ProjectCountry = CountryComboBox.SelectedItem.ToString();
            }

            var ProjectCity = CityTextBox.Text; 

            var ProjectLocation = new LocationDTO { Country = ProjectCountry, City = ProjectCity };
            var ProjectCategory = new CategoryDTO { Name = ProjectCategoryText };

            //TODO:needs work
            var SparkList = new List<SparkDTO>();
            SparkList.Add(new SparkDTO());

            foreach (var s in checkList)
            {
                if (String.IsNullOrEmpty(s) || SkillsList.Count == 0)
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
            var createProjectDTO = new CreateProjectDTO
            {
                Title = ProjectTitle,
                Description = ProjectDescription,
                Location = ProjectLocation,
                Category = ProjectCategory,
            };
            
            var isSuccess = await ((AddProjectPageViewModel)DataContext).PostProject(createProjectDTO, SkillsList);

            if (!isSuccess)
            {
                ContentDialog fillAllFieldsDialog = new ContentDialog
                {
                    Title = "Couldn't create project!",
                    CloseButtonText = "Shame"
                };
                await fillAllFieldsDialog.ShowAsync();
            }
            else
            {
                Frame.Navigate(typeof(ProjectPage), new ProjectViewModel(createProjectDTO));
            }
        }
        
        private async void skillsAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput && !String.IsNullOrEmpty(sender.Text))
            {
                //dunno whats going on
                var skillDTOs = new List<SkillDTO>();
                
                skillDTOs = await ((AddProjectPageViewModel)DataContext).GetSkillsAsync(sender.Text);
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

            //create a new AutoSuggestBox 
            AutoSuggestBox suggestBox = new AutoSuggestBox();
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

        private async void addAttachmentButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                attachments.Insert(0, file);
                addAttachmentButton();
                fileNameTextBlock.Text = file.Name;
            }
        }

        private async void addAttachmentButton_Click_Generated(object sender, RoutedEventArgs e)
        {
            if (textBlockCount < 8)
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    TextBlock textBlock = addButtonTextBlockList.ElementAt(textBlockCount);
                    textBlock.Text = file.Name;
                    attachments.Add(file);
                    addAttachmentButton();
                }
            }

        }

        private void addAttachmentButton()
        {
            textBlockCount++;
            StackPanel buttonTextStackPanel = new StackPanel();
            buttonTextStackPanel.Margin = new Thickness(0, 0, 25, 0); 

            Button addButton = new Button();
            SymbolIcon addIcon = new SymbolIcon();
            addIcon.Symbol = Symbol.Add;
            addButton.Content = addIcon;
            addButton.Height = 50; addButton.Width = 50;

            TextBlock textBlock = new TextBlock();
            textBlock.Name = "fileNameTextBlock" + textBlockCount;
            textBlock.MaxWidth = 50;
            textBlock.TextWrapping = TextWrapping.Wrap;
            addButton.Click += new RoutedEventHandler(addAttachmentButton_Click_Generated);

            buttonTextStackPanel.Children.Add(addButton);
            buttonTextStackPanel.Children.Add(textBlock);
            AttachmentsPanel.Children.Add(buttonTextStackPanel);
            addButtonTextBlockList.Add(textBlock);
        }
    }
}
