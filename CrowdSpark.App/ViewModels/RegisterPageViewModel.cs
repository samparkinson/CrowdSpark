using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.App.Views;
using CrowdSpark.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrowdSpark.App.ViewModels
{
    class RegisterPageViewModel : BaseViewModel
    {
        private readonly IAuthenticationHelper helper;
        private readonly INavigationService service;
        private readonly IUserAPI userAPI;
        private readonly ISkillAPI skillAPI;

        private string _firstname;
        public string Firstname { get => _firstname; set { if (value != _firstname) { _firstname = value; OnPropertyChanged(); } } }

        private string _surname;
        public string Surname { get => _surname; set { if (value != _surname) { _surname = value; OnPropertyChanged(); } } }

        private string _mail;
        public string Mail { get => _mail; set { if (value != _mail) { _mail = value; OnPropertyChanged(); } } }

        private LocationDTO _location;
        public LocationDTO Location { get => _location; set { if (!value.Equals(_location)) { _location = value; OnPropertyChanged(); } } }

        public ObservableCollection<string> Countries { get; set; }

        public RegisterPageViewModel(IAuthenticationHelper _helper, INavigationService _service, IUserAPI _userAPI, ISkillAPI _skillAPI)
        {
            helper = _helper;
            service = _service;
            userAPI = _userAPI;
            skillAPI = _skillAPI;

            Countries = new ObservableCollection<string>(GetCountryList());
        }

        //Clear form and logout, for cancel button
        public async void Cancel()
        {
            await helper.SignOutAsync(account);
            account = null;
            //navigate to log in page
            service.Navigate(typeof(LogInPage), null);
        }

        public async Task<bool> RegisterUser(UserCreateDTO userCreateDTO)
        {
            CompareAndCreateSkills(userCreateDTO.Skills);

            var success = await userAPI.Create(userCreateDTO);
            
            if (success)
            {
                service.Navigate(typeof(UserPage), account);
                return true;
            }

            return false;
        }
        
        private async void CompareAndCreateSkills(ICollection<SkillDTO> skillDTOs)
        {
            foreach (SkillDTO skillDTO in skillDTOs)
            {
                var results = await skillAPI.GetBySearch(skillDTO.Name);

                if (results == null)
                {
                    var skillCreateDTO = new SkillCreateDTO { Name = skillDTO.Name };
                    var skillCreateResult =  await skillAPI.Create(skillCreateDTO);
                }
            }
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
