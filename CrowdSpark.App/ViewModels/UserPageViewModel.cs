using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.App.Views;
using CrowdSpark.Common;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Credentials;
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
        private readonly IUserAPI userAPI;
        private readonly INavigationService service;
        
        //List of skills 
        public ObservableCollection<SkillDTO> Skills { get; set; }
        
        public UserPageViewModel(IAuthenticationHelper _helper, IUserAPI _userAPI, INavigationService _service)
        {
            helper = _helper;
            userAPI = _userAPI;
            service = _service;
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
                    service.Navigate(typeof(LogInPage), null);
                }
                else
                {
                    account = await helper.SignInAsync();
                    if (account != null)
                    {
                        Initialize(account);
                        UserName = CommonAttributes.account.UserName;

                        CommonAttributes.account = account;

                        MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;

                        SignInOutButtonText = "Sign Out";
                    }
                }
            });

            MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;
        }

        public async void Initialize(WebAccount account)
        {
            if (account != null)
            {
                var userDTO = await userAPI.GetMyself();
                Firstname = userDTO.Firstname;
                Surname = userDTO.Surname;
                Mail = userDTO.Mail;
                Location = userDTO.Location;

                Skills = (ObservableCollection<SkillDTO>)userDTO.Skills;
            }
        }

        public async Task<bool> UpdateUser(UserDTO userDTO)
        {
            return await userAPI.Update(userDTO);
        }
    }
}
