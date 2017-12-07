using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Security.Credentials;

namespace CrowdSpark.App.ViewModels
{
    class MainPageViewModel : BaseViewModel
    {
        //get values from db
        public ObservableCollection<ProjectViewModel> Projects { get; set; }

        //To set the height of scroll view
        public int ScrollViewHeight { get; set; }

        //Options
        public ObservableCollection<MenuOption> MenuOptions { get; set; }

        // login
        private static WebAccount account;

        private readonly IAuthenticationHelper helper;

        public ICommand SignInOutCommand { get; }


        public MainPageViewModel()
        {
            Projects = new ObservableCollection<ProjectViewModel>();

            initDummy();

            ScrollViewHeight = Projects.Count * 60;

            MenuOptions = new HamburgerMenuOptionsFactory().MenuOptions;

            SignInOutCommand = new RelayCommand(async o =>
            {
                if (account != null)
                {
                    await helper.SignOutAsync(account);
                    account = null;
                   // Characters.Clear();
                }
                else
                {
                    account = await helper.SignInAsync();
                    if (account != null)
                    {
                        await Initialize();
                    }
                }
            });
        }

<<<<<<< HEAD
<<<<<<< HEAD
        public async Task Initialize()
        {
            account = await helper.GetAccountAsync();

            if (account != null)
            {
              // var characters = await _repository.ReadAsync();

    /*            foreach (var character in characters.Select(c => new CharacterViewModel(c)))
                {
                    Characters.Add(character);
                }
                */
            }
        }
=======
>>>>>>> parent of 9301929... userpage
=======
>>>>>>> parent of 9301929... userpage


        private void initDummy()
        {
            var _location = new Location { Id = 1, City = "Copenhagen", Country = "Denmark" };

            var dummyProjects = new List<ProjectDTO>();
            
            for (int i = 0; i < 20; i++)
            {
                dummyProjects.Add(new ProjectDTO { Id = i, Title = "Project " + i, Location = _location, Description = "Description " + i });
            }

            foreach (var p in dummyProjects)
            {
                Projects.Add(new ProjectViewModel(p));
            }
        }
    }
}
