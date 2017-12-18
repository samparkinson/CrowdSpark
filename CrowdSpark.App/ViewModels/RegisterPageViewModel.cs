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
            userCreateDTO.Skills = await CompareAndCreateSkills(userCreateDTO.Skills);
            
            var success = await userAPI.Create(userCreateDTO);
            
            if (success)
            {
                service.Navigate(typeof(UserPage), account);
                return true;
            }

            return false;
        }
        
        private async Task<ICollection<SkillDTO>> CompareAndCreateSkills(ICollection<SkillDTO> skillDTOs)
        {
            ICollection<SkillDTO> skillsWithIDs = new List<SkillDTO>();
            
            //create non existing skills
            foreach (SkillDTO skillDTO in skillDTOs)
            {
                var results = await skillAPI.GetBySearch(skillDTO.Name);

                //Create if non existent
                if (results == null)
                {
                    var skillCreateDTO = new SkillCreateDTO { Name = skillDTO.Name };
                    await skillAPI.Create(skillCreateDTO);
                    var skillsWithId = await skillAPI.GetBySearch(skillDTO.Name);
                    foreach (SkillDTO skillWithId in skillsWithId)
                    {
                        if (skillWithId.Name.Equals(skillDTO.Name))
                        {
                            skillsWithIDs.Add(skillWithId);
                        }
                    }
                }
                else
                {
                    foreach (var skill in results)
                    {
                        if (skill.Name.Equals(skillDTO.Name))
                        {
                            skillsWithIDs.Add(skill);
                            continue;
                        }
                        var skillCreateDTO = new SkillCreateDTO { Name = skillDTO.Name };
                        await skillAPI.Create(skillCreateDTO);
                        var skillsWithId = await skillAPI.GetBySearch(skillDTO.Name);
                        foreach (SkillDTO skillWithId in skillsWithId)
                        {
                            if (skillWithId.Name.Equals(skillDTO.Name))
                            {
                                skillsWithIDs.Add(skillWithId);
                            }
                        }
                    }
                }
            }

            //return SkillDTOs with ID
            return skillsWithIDs;
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
