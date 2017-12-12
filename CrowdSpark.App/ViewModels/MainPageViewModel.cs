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
        private readonly IProjectRepository projectRepository;
        private readonly ICategoryRepository categoryRepository;

        //command to repopulate the content of main page
        public ICommand RepopulateContentCommand { get; set; }

        //Command to initialize the login on app opening
        private ICommand SignInCommand { get; set; }
        
        public MainPageViewModel(IAuthenticationHelper _helper, IProjectRepository _projectRepository, ICategoryRepository _categoryRepository)
        {
            //init the helper
            helper = _helper;
            //init the repo for projects
            projectRepository = _projectRepository;
            categoryRepository = _categoryRepository;
            
            Content = new ObservableCollection<ProjectViewModel>();

            //initDummyProjects();

            Categories = new ObservableCollection<Category>();

            initDummyCategories();
            
            //pop up the login screen if user is not logged in
            SignInCommand = new RelayCommand(async o =>
            {
                if (account == null)
                {
                    account = await helper.SignInAsync();

                    if (account != null)
                    {
                        Debug.WriteLine("Sign in successfull!");
                        await GetRecentProjects();
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
                }
                else
                {
                    account = await helper.SignInAsync();
                    if (account != null)
                    {
                        Debug.WriteLine("Sign in successfull!");
                        await GetRecentProjects();
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
                        await GetCategories();
                        break;
                }
            });
            

            MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;

            ScrollViewHeight = Content.Count * 60;

            //Store the stuff in a static class
            CommonAttributes.MenuOptions = MenuOptions;
            CommonAttributes.account = account;
        }

        public async Task GetRecentProjects()
        {
            Content.Clear();
            account = await helper.GetAccountAsync();
            
            if (account != null)
            {
                var recentProjects = await projectRepository.ReadAsync();
                
                //p is ProjectSummaryDTO
                foreach (var project in recentProjects.Select(p => new ProjectViewModel(p)))
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
                var categories = await categoryRepository.ReadAsync();

                //p is ProjectSummaryDTO
                foreach (var category in categories.Select(c => new Category { Name = c.Name, Id = c.Id }))
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
