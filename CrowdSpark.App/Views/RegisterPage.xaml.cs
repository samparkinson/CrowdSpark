using CrowdSpark.App.ViewModels;
using CrowdSpark.Common;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.Security.Credentials;

namespace CrowdSpark.App.Views
{
    public sealed partial class RegisterPage : Page
    {
        private List<SkillCreateDTO> SkillsList { get; set; }

        private readonly RegisterPageViewModel _vm;

        public RegisterPage()
        {
            InitializeComponent();

            SkillsList = new List<SkillCreateDTO>();

            _vm = App.ServiceProvider.GetService<RegisterPageViewModel>();

            DataContext = _vm;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;

            MailTextBox.Text = ((WebAccount)e.Parameter).UserName;
            //write account to view model
            ((RegisterPageViewModel)DataContext).account = (WebAccount)e.Parameter;
        }

        private void SkillsTextBox_TextChanged(object sender, TextChangedEventArgs e)
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
                        SkillsList.Add(new SkillCreateDTO { Name = text });
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
                textBox.BorderThickness = new Thickness(15);
                textBox.Padding = new Thickness(10);
                textBox.Background = new SolidColorBrush(Colors.White);
                textBox.TextChanged += new TextChangedEventHandler(SkillsTextBox_TextChanged);

                SkillsPanel.Children.Add(textBox);
            }
        }

        private async void registerButton_Click(object sender, RoutedEventArgs e)
        {
            var checkList = new List<string>();
            var Name = NameTextBox.Text; checkList.Add(Name);
            var Surname = SurnameTextBox.Text; checkList.Add(Surname);
            var Mail = MailTextBox.Text; checkList.Add(Mail);
            var Country = "";

            var CountrySelection = CountryComboBox.SelectedItem;
            if (CountrySelection != null)
            {
                Country = CountrySelection.ToString();
            }

            var City = CityTextBox.Text; 

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

            LocationDTO location = new LocationDTO { Country = Country, City = City, Id = 0 };

            var userCreateDTO = new UserCreateDTO { Firstname = Name, Surname = Surname, Mail = Mail, Location = location };

            var isSuccess = await ((RegisterPageViewModel)DataContext).RegisterUser(userCreateDTO, SkillsList);
            
            if (!isSuccess)
            {
                ContentDialog fillAllFieldsDialog = new ContentDialog
                {
                    Title = "Couldn't create account!",
                    CloseButtonText = "Shame"
                };
                await fillAllFieldsDialog.ShowAsync();
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = "";
            SurnameTextBox.Text = "";
            MailTextBox.Text = "";
            CountryComboBox.SelectedIndex = -1;
            CityTextBox.Text = "";
            
            ((RegisterPageViewModel)DataContext).Cancel();
        }
    }
}
