using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CrowdSpark.App.ViewModels
{
    class SearchPageViewModel : BaseViewModel
    {
        public ObservableCollection<ProjectViewModel> ProjectResults { get; set; }
        public ObservableCollection<UserDTO> UserResults { get; set; }

        private readonly IProjectAPI projectAPI;

        private readonly IAuthenticationHelper helper;
        
        public SearchPageViewModel(IAuthenticationHelper _helper, IProjectAPI _projectAPI)
        {
            projectAPI = _projectAPI;
            helper = _helper;
            account = CommonAttributes.account;
            UserName = account.UserName;

            SignInOutButtonText = account == null ? "Sign In" : "Sign Out";
            
            ProjectResults = new ObservableCollection<ProjectViewModel>();
            UserResults = new ObservableCollection<UserDTO>();

            SignInOutCommand = new RelayCommand(async o =>
            {
                //sign out
                if (account != null)
                {
                    await helper.SignOutAsync(account);
                    account = null;
                    CommonAttributes.account = account;
                    ProjectResults.Clear();
                    SignInOutButtonText = "Sign In";
                }
                else //sign in
                {
                    account = await helper.SignInAsync();
                    if (account != null)
                    {
                        CommonAttributes.account = account;

                        UserName = account.UserName;

                        //Store the stuff in a static class
                        MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;

                        SignInOutButtonText = "Sign Out";
                    }
                }
            });
            
            MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;
        }

        public async void SearchByCategory(int categoryId)
        {
            ProjectResults.Clear();

            var searchResults = await projectAPI.GetByCategory(categoryId);
        }

        public async void SearchProjects(string Query)
        {
            //get the new results and write them into Results
            ProjectResults.Clear();

            var searchResults = await projectAPI.GetBySearch(Query);

            foreach(var result in searchResults)
            {
                ProjectResults.Add(new ProjectViewModel(result));
            }
        }
    }
}
