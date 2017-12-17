using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.App.Views;
using CrowdSpark.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrowdSpark.App.ViewModels
{
    class MainPageViewModel : BaseViewModel
    {

        //get values from db
        //projects list
        private ObservableCollection<ProjectViewModel> _content;
        public ObservableCollection<ProjectViewModel> Content {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        //categories list
        private ObservableCollection<CategoryDTO> _categories;
        public ObservableCollection<CategoryDTO> Categories {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        private readonly IAuthenticationHelper helper;
        private readonly INavigationService service;
        private readonly IProjectAPI projectAPI;

        //command to repopulate the content of main page
        public ICommand RepopulateContentCommand { get; set; }

        //Command to initialize the login on app opening
        private ICommand SignInCommand { get; set; }
        
        public MainPageViewModel(IAuthenticationHelper _helper, IProjectAPI _projectAPI, INavigationService _service)
        {
            helper = _helper;
            service = _service;
            projectAPI = _projectAPI;
            
            initDummyCategories();
            
            //pop up the login screen if user is not logged in
            //called only on startup
            SignInCommand = new RelayCommand(async o =>
            {
                if (account == null)
                {
                    SignInOutButtonText = "Sign In";
                    
                    account = await helper.SignInAsync();

                    if (account != null)
                    {
                        CommonAttributes.account = account;

                        UserName = account.UserName;
                        await GetRecentProjects();

                        SignInOutButtonText = "Sign Out";

                        MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;
                    }
                }
                else
                {
                    await GetRecentProjects();

                    SignInOutButtonText = "Sign Out";
                }
            });
            
            SignInOutCommand = new RelayCommand(async o =>
            {
                //sign out
                if (account != null)
                {
                    await helper.SignOutAsync(account);
                    account = null;
                    CommonAttributes.account = account;
                    Content.Clear();
                    SignInOutButtonText = "Sign In";
                    service.Navigate(typeof(LogInPage), null);
                }
                else //sign in
                {
                    account = await helper.SignInAsync();
                    if (account != null)
                    {
                        //initDummyProjects();

                        await GetRecentProjects();

                        CommonAttributes.account = account;

                        UserName = account.UserName;

                        //Store the stuff in a static class
                        MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;

                        SignInOutButtonText = "Sign Out";
                    }
                }
            });
            
            RepopulateContentCommand = new RelayCommand(async (tabName) => 
            {
                //Populate lists 
                switch (tabName)
                {
                    case "Recent":
                        //initDummyProjects();
                        await GetRecentProjects();
                        break;
                    case "Categories":
                        initDummyCategories();
                        break;
                }
            });
            
            //pop up sign in page on startup
            SignInCommand.Execute(null);

            //Store the stuff in a static class
            MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;
            CommonAttributes.MenuOptions = MenuOptions;   
        }

        public async Task GetRecentProjects()
        {
            Content = null;
            Content = new ObservableCollection<ProjectViewModel>();
            //Content.Clear();
            
            if (account != null)
            {
                var recentProjects = await projectAPI.GetAll();
                Debug.WriteLine("Getting projects");

                //p is ProjectSummaryDTO
                foreach (var project in recentProjects.Select(p => new ProjectViewModel(p)))
                {
                    Debug.WriteLine(project.Title);
                    Content.Add(project);
                }
            }
        }

        public async Task GetCategories()
        {
            Categories = null;
            Categories = new ObservableCollection<CategoryDTO>();

            if (account != null)
            {
                //should be projectAPI.GetCategories();
                var categories = await projectAPI.GetAll();

                //TODO: change to category
                foreach (var category in categories.Select(c => new CategoryDTO { Name = c.Title }))
                {
                    Categories.Add(category);
                }
            }
        }

        private void initDummyCategories()
        {
            Debug.WriteLine("Getting categories");

            Categories = null;
            Categories = new ObservableCollection<CategoryDTO>();

            var category = new CategoryDTO { Name="Programming", Id=0};
            
            for (int i = 0; i < 20; i++)
            {
                Categories.Add(new CategoryDTO { Name = "Cat " + i, Id = i });
            }
        }

        
        private void initDummyProjects()
        {
            Content = null;
            Content = new ObservableCollection<ProjectViewModel>();
            
            var _location = new LocationDTO { Id = 1, City = "Copenhagen", Country = "Denmark" };

            var _dummyProjects = new List<ProjectDTO>();
            
            for (int i = 0; i < 20; i++)
            {
                _dummyProjects.Add(new ProjectDTO { Title = "Project " + i, Location = _location, Description = "Description " + i, Category = new CategoryDTO { Name = "Programming" } });
            }
            
            foreach (var p in _dummyProjects)
            {
                Content.Add(new ProjectViewModel(p));
            }
        }
    }
}
