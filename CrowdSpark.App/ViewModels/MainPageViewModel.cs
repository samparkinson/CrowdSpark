using CrowdSpark.App.Helpers;
using CrowdSpark.Common;
using CrowdSpark.Entitites;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                    await _helper.SignOutAsync(_account);
                    _account = null;
                    Characters.Clear();
                }
                else
                {
                    _account = await _helper.SignInAsync();
                    if (_account != null)
                    {
                        await Initialize();
                    }
                }
            });
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
