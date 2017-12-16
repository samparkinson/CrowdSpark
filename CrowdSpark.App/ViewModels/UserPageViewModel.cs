using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.Common;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace CrowdSpark.App.ViewModels
{
    class UserPageViewModel : BaseViewModel
    {
        private int _id;
        public int Id { get => _id; set { if (value != _id) { _id = value; OnPropertyChanged(); } } }

        private string _firstname;
        public string Firstname { get => _firstname; set { if (value != _firstname) { _firstname = value; OnPropertyChanged(); } } }

        private string _surname;
        public string Surname { get => _surname; set { if (value != _surname) { _surname = value; OnPropertyChanged(); } } }

        private string _mail;
        public string Mail { get => _mail; set { if (value != _mail) { _mail = value; OnPropertyChanged(); } } }

        private LocationDTO _location;
        public LocationDTO Location { get => _location; set { if (!value.Equals(_location)) { _location = value; OnPropertyChanged(); } } }
        
        private readonly IAuthenticationHelper helper;

        //List of skills 
        public ObservableCollection<SkillDTO> Skills { get; set; }

        public UserPageViewModel(IAuthenticationHelper _helper)
        {
            helper = _helper;
            account = CommonAttributes.account;
            UserName = account.UserName;

            SignInOutButtonText = account == null ? "Sign In" : "Sign Out";

            SignInOutCommand = new RelayCommand(async o =>
            {
                if (account != null)
                {
                    await helper.SignOutAsync(account);
                    account = null;
                    CommonAttributes.account = account;
                    SignInOutButtonText = "Sign In";
                }
                else
                {
                    account = await helper.SignInAsync();
                    if (account != null)
                    {
                        //Initialize();
                        UserName = CommonAttributes.account.UserName;

                        CommonAttributes.account = account;

                        MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;

                        SignInOutButtonText = "Sign Out";
                    }
                }
            });
            MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;
        }

        public void Initialize(UserViewModel userViewModel)
        {
            Firstname = userViewModel.Firstname;

            Surname = userViewModel.Surname;

            Mail = userViewModel.Mail;

            Location = userViewModel.Location;

            //Not sure
            Skills = (ObservableCollection<SkillDTO>) userViewModel.Skills;
            
        }
    }
}
