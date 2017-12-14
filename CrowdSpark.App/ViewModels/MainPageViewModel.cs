using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Security.Credentials;

namespace CrowdSpark.App.ViewModels
{
    class MainPageViewModel : BaseViewModel
    {
        //get values from db
        //projects list
        public ObservableCollection<ProjectViewModel> Content { get; set; }

        //categories list
        public ObservableCollection<Category> Categories { get; set; }

        //To set the height of scroll view
        public int ScrollViewHeight { get; set; }

        private readonly IAuthenticationHelper helper;

        private readonly IProjectAPI projectAPI;

        //command to repopulate the content of main page
        public ICommand RepopulateContentCommand { get; set; }

        //Command to initialize the login on app opening
        private ICommand SignInCommand { get; set; }
        
        public MainPageViewModel(IAuthenticationHelper _helper, IProjectAPI _projectAPI)
        {
            //init the helper
            helper = _helper;
           
            //recently implemented juicy api
            projectAPI = _projectAPI;
            
            Content = new ObservableCollection<ProjectViewModel>();

            //initDummyProjects();

            Categories = new ObservableCollection<Category>();

            //initDummyCategories();
            
            //pop up the login screen if user is not logged in
            SignInCommand = new RelayCommand(async o =>
            {
                if (account == null)
                {
                    account = await helper.SignInAsync();

                    if (account != null)
                    {
                        Debug.WriteLine("Sign in successfull!");

                        initDummyProjects();

                        CommonAttributes.account = account;

                        UserName = account.UserName;
                        //await GetRecentProjects();
                    }
                }
            });

            SignInCommand.Execute(null);
            
            SignInOutCommand = new RelayCommand(async o =>
            {
                if (account != null)
                {
                    await helper.SignOutAsync(account);
                    account = null;
                    Content.Clear();
                    SignInOutButtonText = "Sign In";
                }
                else
                {
                    account = await helper.SignInAsync();
                    if (account != null)
                    {
                        Debug.WriteLine("Sign in successfull!");

                        initDummyProjects();

                        //await GetRecentProjects();

                        CommonAttributes.account = account;

                        UserName = account.UserName;
                        
                        SignInOutButtonText = "Sign Out";
                    }
                }
            });

            //TODO: use this somehow
            RepopulateContentCommand = new RelayCommand(async (tabName) => 
            {
                switch (tabName)
                {
                    case "Recent":
                        await GetRecentProjects();
                        break;
                    case "Categories":
                        initDummyCategories();
                        break;
                }
            });

            MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;

            ScrollViewHeight = Content.Count * 60;

            //Store the stuff in a static class
            CommonAttributes.MenuOptions = MenuOptions;
        }

        public async Task GetRecentProjects()
        {
            Content.Clear();
            account = await helper.GetAccountAsync();
            
            if (account != null)
            {
                var recentProjects = await projectAPI.GetAll();
                
                //p is ProjectSummaryDTO
                //take command is experimental
                foreach (var project in recentProjects.Select(p => new ProjectViewModel(p)).Take(10))
                {
                    Content.Add(project);
                }
            }
        }

        public async Task GetCategories()
        {
            Categories.Clear();
            account = await helper.GetAccountAsync();

            if (account != null)
            {
                //should be projectAPI.GetCategories();
                var categories = await projectAPI.GetAll();

                //TODO: change to category
                foreach (var category in categories.Select(c => new Category { Name = c.Title }))
                {
                    Categories.Add(category);
                }
            }
        }

        private void initDummyCategories()
        {
            Categories.Clear();

            var category = new Category { Name="Programming", Id=0};

            for (int i = 0; i < 20; i++)
            {
                Categories.Add(new Category { Name = "Cat " + i, Id = i });
            }
        }

        
        private void initDummyProjects()
        {
            Content.Clear();

            var _location = new Location { Id = 1, City = "Copenhagen", Country = "Denmark" };

            var _dummyProjects = new List<ProjectDTO>();
            
            for (int i = 0; i < 20; i++)
            {
                _dummyProjects.Add(new ProjectDTO { Id = i, Title = "Project " + i, Location = _location, Description = "Description " + i, Category = new Category { Name = "Programming" } });
            }

            foreach (var p in _dummyProjects)
            {
                Content.Add(new ProjectViewModel(p));
            }
        }
    }
}
