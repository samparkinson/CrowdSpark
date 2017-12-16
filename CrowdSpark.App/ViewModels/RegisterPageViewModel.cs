using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.App.Views;
using CrowdSpark.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrowdSpark.App.ViewModels
{
    class RegisterPageViewModel : BaseViewModel
    {
        private readonly IAuthenticationHelper helper;
        private readonly INavigationService service;
        private readonly IUserAPI userAPI;

        private string _firstname;
        public string Firstname { get => _firstname; set { if (value != _firstname) { _firstname = value; OnPropertyChanged(); } } }

        private string _surname;
        public string Surname { get => _surname; set { if (value != _surname) { _surname = value; OnPropertyChanged(); } } }

        private string _mail;
        public string Mail { get => _mail; set { if (value != _mail) { _mail = value; OnPropertyChanged(); } } }

        private LocationDTO _location;
        public LocationDTO Location { get => _location; set { if (!value.Equals(_location)) { _location = value; OnPropertyChanged(); } } }

        //Clear form and logout
        public ICommand ClearCommand;

        public RegisterPageViewModel(IAuthenticationHelper _helper, INavigationService _service, IUserAPI _userAPI)
        {
            helper = _helper;
            service = _service;
            userAPI = _userAPI;
            
            SignInOutCommand = new RelayCommand(async o =>
            {
                account = await helper.GetAccountAsync();
                //sign out
                if (account != null)
                {
                    service.Navigate(typeof(MainPage), null);
                }
                else //sign in
                {
                    account = await helper.SignInAsync();
                    if (account != null)
                    {
                        CommonAttributes.account = account;

                        Mail = account.UserName;
                    }
                }
            });

            ClearCommand = new RelayCommand(async o =>
            {
                await helper.SignOutAsync(account);
                account = null;
            });

            SignInOutCommand.Execute(null);
        }

        public async void RegisterUser(UserCreateDTO userCreateDTO)
        {
            var success = await userAPI.Create(userCreateDTO);
            
            if (success)
            {
                service.Navigate(typeof(UserPage), account);
            }
        }
    }
}
