using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.App.Views;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        
        //store login information, 
        //should this be in base class or common attributes static class??
        private WebAccount account;

        private readonly IAuthenticationHelper helper;
        private readonly IProjectRepository repository;
        
        //command to repopulate the content of main page
        public ICommand RepopulateContentCommand { get; set; }

        //Command to initialize the login on app opening
        private ICommand SignInCommand { get; set; }
        
        public MainPageViewModel(IAuthenticationHelper _helper, IProjectRepository _repository)
        {
            //init the helper
            helper = _helper;
            repository = _repository;

            //pop up the login screen if user is not logged in
            SignInCommand = new RelayCommand(async o =>
            {
                if (account == null)
                {
                    account = await helper.SignInAsync();

                    if (account != null)
                    {
                        Debug.WriteLine("Sign in successfull!");
                        await Initialize();
                    }
                }
            });

            SignInCommand.Execute(null);

            Content = new ObservableCollection<ProjectViewModel>();

            initDummyProjects();

            Categories = new ObservableCollection<Category>();

            initDummyCategories();

            ScrollViewHeight = Content.Count * 60;
            
            SignInOutCommand = new RelayCommand(async o =>
            {
                if (account != null)
                {
                    await helper.SignOutAsync(account);
                    account = null;
                }
                else
                {
                    account = await helper.SignInAsync();
                    if (account != null)
                    {
                        Debug.WriteLine("Sign in successfull!");
                        await Initialize();
                    }
                }
            });

            //TODO: use this somehow
            RepopulateContentCommand = new RelayCommand((tab) => 
            {

            });
            
            MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;

            //Store the stuff in a static class
            CommonAttributes.MenuOptions = MenuOptions;
            CommonAttributes.account = account;
        }

        public async Task Initialize()
        {
            account = await helper.GetAccountAsync();
            
            if (account != null)
            {
                Debug.WriteLine("Signed in as " + account.UserName);
                
              // var characters = await _repository.ReadAsync();

    /*            foreach (var character in characters.Select(c => new CharacterViewModel(c)))
                {
                    Characters.Add(character);
                }
                */
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
