using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.App.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrowdSpark.App.ViewModels
{
    class LogInPageViewModel : BaseViewModel
    {
        public ICommand LogInCommand { get; set; }
        public ICommand InitCommand;
        private readonly IAuthenticationHelper helper;
        private readonly INavigationService service;
        private readonly IUserAPI userAPI;
        
        public LogInPageViewModel(IAuthenticationHelper _helper, INavigationService _service, IUserAPI _userAPI)
        {
            helper = _helper;
            service = _service;
            userAPI = _userAPI;

            //gets called when app starts
            InitCommand = new RelayCommand(async o => 
            {
                account = await helper.GetAccountAsync();
                if (account != null)
                {
                    //check api if user exists
                    var user = await userAPI.GetMyself();

                    //navigate to main page
                    if (user.Mail != null)
                    {
                        CommonAttributes.account = account;
                        service.Navigate(typeof(MainPage), null);
                    }
                }
            });

            //gets called when log in button is pressed
            LogInCommand = new RelayCommand(async o => 
            {
                account = await helper.GetAccountAsync();
                //navigate to main page
                if (account == null)
                {
                    //pop up sign in page
                    account = await helper.SignInAsync();

                    //if sign in is successfull
                    if (account != null)
                    {
                        CommonAttributes.account = account;
                        //check api if user exists
                        var user = await userAPI.GetMyself();

                        if (user.Mail == null)
                        {
                            service.Navigate(typeof(RegisterPage), account);
                        }
                        else
                        {
                            service.Navigate(typeof(MainPage), null);
                        }
                    }
                }
                else
                {
                    CommonAttributes.account = account;
                    //check api if user exists
                    var user = await userAPI.GetMyself();

                    if (user.Mail == null)
                    {
                        service.Navigate(typeof(RegisterPage), account);
                    }
                    else
                    {
                        service.Navigate(typeof(MainPage), null);
                    }
                }
            });
        }
    }
}
