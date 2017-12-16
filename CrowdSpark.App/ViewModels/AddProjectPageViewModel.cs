using CrowdSpark.App.Converters;
using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Data;

namespace CrowdSpark.App.ViewModels
{
    class AddProjectPageViewModel : BaseViewModel
    {
        public ObservableCollection<string> Countries { get; set; }
        
        public ObservableCollection<string> Cities { get; set; }

        public ICommand PostProjectCommand { get; set; }

        IProjectAPI projectAPI;

        public CreateProjectDTO createProjectDTO { get; set; }

        IAuthenticationHelper helper;

        public AddProjectPageViewModel(IProjectAPI _projectAPI, IAuthenticationHelper _helper)
        {
            projectAPI = _projectAPI;
            helper = _helper;
            account = CommonAttributes.account;
            UserName = account.UserName;

            SignInOutButtonText = account == null ? "Sign In" : "Sign Out";
            
            Countries = new ObservableCollection<string>();
            Cities = new ObservableCollection<string>();
            
            PostProjectCommand = new RelayCommand(async (project) =>
            {
                if (account != null)
                {
                    Debug.WriteLine(((CreateProjectDTO)project).Title);
                    await projectAPI.Create((CreateProjectDTO)project);
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
                    SignInOutButtonText = "Sign In";
                }
                else //sign in
                {
                    account = await helper.SignInAsync();
                    if (account != null)
                    {
                        CommonAttributes.account = account;

                        UserName = account.UserName;

                        SignInOutButtonText = "Sign Out";
                    }
                }
            });

            Countries.Add("Denmark");
            Countries.Add("Turkey");
            Countries.Add("Germany");
            Countries.Add("Sweden");

            Cities.Add("Copenhagen");
            Cities.Add("Ankara");
            Cities.Add("Malmø");
            
            MenuOptions = new HamburgerMenuOptionsFactory(CommonAttributes.account).MenuOptions;
        }
    }
}
