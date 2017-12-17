using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.App.Views;
using CrowdSpark.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;

namespace CrowdSpark.App.ViewModels
{
    class AddProjectPageViewModel : BaseViewModel
    {
        public ObservableCollection<string> Countries { get; set; }
        
        public ICommand PostProjectCommand { get; set; }

        private readonly IProjectAPI projectAPI;

        public CreateProjectDTO createProjectDTO { get; set; }

        private readonly IAuthenticationHelper helper;
        private readonly INavigationService service;

        public AddProjectPageViewModel(IProjectAPI _projectAPI, IAuthenticationHelper _helper, INavigationService _service)
        {
            projectAPI = _projectAPI;
            helper = _helper;
            service = _service;
            account = CommonAttributes.account;
            UserName = account.UserName;

            SignInOutButtonText = account == null ? "Sign In" : "Sign Out";
            
            //init countries list
            Countries = new ObservableCollection<string>(GetCountryList());
            
            PostProjectCommand = new RelayCommand(async (project) =>
            {
                if (account != null)
                {
                    if (project != null)
                    {
                        var createProjectDTO = (CreateProjectDTO)project;
                        Debug.WriteLine(((CreateProjectDTO)project).Title);
                        var result = await projectAPI.Create(createProjectDTO);
                        if (result)
                        {
                            service.Navigate(typeof(ProjectPage), new ProjectViewModel(createProjectDTO));
                        }
                    }
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
            
            MenuOptions = new HamburgerMenuOptionsFactory(CommonAttributes.account).MenuOptions;
        }

        private List<string> GetCountryList()
        {
            List<string> cultureList = new List<string>();

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo culture in cultures)
            {
                RegionInfo region = new RegionInfo(culture.LCID);

                if (!(cultureList.Contains(region.EnglishName)))
                {
                    cultureList.Add(region.EnglishName);
                }
            }

            cultureList.Sort();

            return cultureList;
        }
    }
}
