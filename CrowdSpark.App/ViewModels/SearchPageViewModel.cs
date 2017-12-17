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

        //called on page load
        public void Initialize(string Query)
        {
            //TODO:get the results from the repo async
            //initDummyProjects();

            ProjectResults = new ObservableCollection<ProjectViewModel>();
            UserResults = new ObservableCollection<UserDTO>();

            SearchProjects(Query);
        }

        private void initDummyProjects()
        {
            ProjectResults.Clear();

            var _location = new LocationDTO { Id = 1, City = "Copenhagen", Country = "Denmark" };

            var _dummyProjects = new List<ProjectDTO>();

            for (int i = 0; i < 20; i++)
            {
                _dummyProjects.Add(new ProjectDTO { Id = i, Title = "Project " + i, Location = _location, Description = "Description " + i, Category = new CategoryDTO { Name = "Programming" } });
            }

            foreach (var p in _dummyProjects)
            {
                ProjectResults.Add(new ProjectViewModel(p));
            }
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
            
            //for testing purposes
            var _location = new LocationDTO { Id = 1, City = "Helsinki", Country = "Finland" };

            var _dummyProjects = new List<ProjectDTO>();

            for (int i = 0; i < 20; i++)
            {
                _dummyProjects.Add(new ProjectDTO { Id = i, Title = Query + " " + i, Location = _location, Description = "Description " + i, Category = new CategoryDTO { Name = "Programming" } });
            }

            foreach (var p in _dummyProjects)
            {
                ProjectResults.Add(new ProjectViewModel(p));
            }
        }

        public async void SearchUsers(string Query)
        {

        }
    }
}
