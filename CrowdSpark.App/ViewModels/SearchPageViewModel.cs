using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CrowdSpark.App.ViewModels
{
    class SearchPageViewModel : BaseViewModel
    {
        public ObservableCollection<ProjectViewModel> Results { get; set; }

        private readonly IProjectAPI projectAPI;

        private readonly IAuthenticationHelper helper;

        public SearchPageViewModel(IAuthenticationHelper _helper, IProjectAPI _projectAPI)
        {
            projectAPI = _projectAPI;
            helper = _helper;
            account = CommonAttributes.account;
            UserName = account.UserName;

            SignInOutButtonText = account == null ? "Sign In" : "Sign Out";

            SignInOutCommand = new RelayCommand(async o =>
            {
                //sign out
                if (account != null)
                {
                    await helper.SignOutAsync(account);
                    account = null;
                    CommonAttributes.account = account;
                    Results.Clear();
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

        public void Initialize(string Query)
        {
            //TODO:get the results from the repo async
            //initDummyProjects();

            Results = new ObservableCollection<ProjectViewModel>();

            MenuOptions = CommonAttributes.MenuOptions;

            renewResults(Query);
        }

        private void initDummyProjects()
        {
            Results.Clear();

            var _location = new LocationDTO { Id = 1, City = "Copenhagen", Country = "Denmark" };

            var _dummyProjects = new List<ProjectDTO>();

            for (int i = 0; i < 20; i++)
            {
                _dummyProjects.Add(new ProjectDTO { Id = i, Title = "Project " + i, Location = _location, Description = "Description " + i, Category = new CategoryDTO { Name = "Programming" } });
            }

            foreach (var p in _dummyProjects)
            {
                Results.Add(new ProjectViewModel(p));
            }
        }

        public void renewResults(string Query)
        {
            //get the new results and write them into Results
            Results.Clear();
            
            //for testing purposes
            var _location = new LocationDTO { Id = 1, City = "Helsinki", Country = "Finland" };

            var _dummyProjects = new List<ProjectDTO>();

            for (int i = 0; i < 20; i++)
            {
                _dummyProjects.Add(new ProjectDTO { Id = i, Title = Query + " " + i, Location = _location, Description = "Description " + i, Category = new CategoryDTO { Name = "Programming" } });
            }

            foreach (var p in _dummyProjects)
            {
                Results.Add(new ProjectViewModel(p));
            }
        }
    }
}
