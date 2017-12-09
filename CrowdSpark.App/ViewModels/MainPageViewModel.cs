using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
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
        public ObservableCollection<ProjectViewModel> Projects { get; set; }

        //To set the height of scroll view
        public int ScrollViewHeight { get; set; }

        //store login information
        private WebAccount account;

        private readonly IAuthenticationHelper helper;

        public ICommand SignInOutCommand { get; }
        
        public MainPageViewModel(IAuthenticationHelper _helper)
        {
            Projects = new ObservableCollection<ProjectViewModel>();

            initDummy();

            ScrollViewHeight = Projects.Count * 60;

            helper = _helper;
            
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
                        Debug.WriteLine("Sign in successfull");
                        await Initialize();
                    }
                }
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
