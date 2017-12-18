using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.App.Views;
using CrowdSpark.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace CrowdSpark.App.ViewModels
{
    class UserPageViewModel : BaseViewModel
    {
        private int _id;
        public int Id { get => _id; set { if (value != _id) { _id = value; OnPropertyChanged(); } } }

        private string _firstname;
        public string Firstname { get => _firstname; set { if (value != _firstname) { _firstname = value; OnPropertyChanged(); } } }

        private string _surname;
        public string Surname { get => _surname; set { if (value != _surname) { _surname = value; OnPropertyChanged(); } } }

        private string _mail;
        public string Mail { get => _mail; set { if (value != _mail) { _mail = value; OnPropertyChanged(); } } }

        private LocationDTO _location;
        public LocationDTO Location { get => _location; set { if (value != null && !value.Equals(_location)) { _location = value; OnPropertyChanged(); } } }

        public ObservableCollection<string> Countries { get; set; }

        private readonly IAuthenticationHelper helper;
        private readonly IUserAPI userAPI;
        private readonly INavigationService service;
        private readonly ISkillAPI skillAPI;
        
        //List of skills 
        public ObservableCollection<SkillDTO> Skills { get; set; }
        
        public UserPageViewModel(IAuthenticationHelper _helper, IUserAPI _userAPI, ISkillAPI _skillAPI, INavigationService _service)
        {
            helper = _helper;
            userAPI = _userAPI;
            service = _service;
            skillAPI = _skillAPI;
            account = CommonAttributes.account;
            UserName = account.UserName;
            Countries = new ObservableCollection<string>(GetCountryList());

            SignInOutButtonText = account == null ? "Sign In" : "Sign Out";
            
            SignInOutCommand = new RelayCommand(async o =>
            {
                if (account != null)
                {
                    await helper.SignOutAsync(account);
                    account = null;
                    CommonAttributes.account = account;
                    SignInOutButtonText = "Sign In";
                    service.Navigate(typeof(LogInPage), null);
                }
                else
                {
                    account = await helper.SignInAsync();
                    if (account != null)
                    {
                        Initialize(account);
                        UserName = CommonAttributes.account.UserName;

                        CommonAttributes.account = account;

                        MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;

                        SignInOutButtonText = "Sign Out";
                    }
                }
            });

            MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;
        }

        public async void Initialize(WebAccount account)
        {
            if (account != null)
            {
                var userDTO = await userAPI.GetMyself();
                Firstname = userDTO.Firstname;
                Surname = userDTO.Surname;
                Mail = userDTO.Mail;
                Location = userDTO.Location;
                Id = userDTO.Id;

                Skills = new ObservableCollection<SkillDTO>(userDTO.Skills);
            }
        }

        public async Task<bool> UpdateUser(UserDTO userDTO, List<SkillCreateDTO> skillCreateDTOs)
        {
            var userSkills = new List<SkillDTO>();
            foreach (SkillCreateDTO skillCreateDTO in skillCreateDTOs)
            {
                var skillId = await skillAPI.Create(skillCreateDTO);

                //replace the id with the actual one
                SkillDTO skillDTO = new SkillDTO { Id = skillId, Name = skillCreateDTO.Name };
                await userAPI.AddSkill(skillDTO);
                userSkills.Add(skillDTO);
            }
            userDTO.Skills = userSkills;
            userDTO.Id = Id;
            var result = await userAPI.Update(userDTO);
            return result;
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

        public async Task<List<SkillDTO>> GetSkillsAsync(string Query)
        {
           var result = await skillAPI.GetBySearch(Query);

           if (result != null)
           {
               lock (result)
               {
                   return new List<SkillDTO>(result);
               }
           }
           else
                return null;
        }

        public List<SkillCreateDTO> populateSkillsList()
        {
            var result = new List<SkillCreateDTO>();
            if (Skills != null)
            {
                foreach (SkillDTO skillDTO in Skills)
                {
                    result.Add(new SkillCreateDTO { Name = skillDTO.Name });
                }
            }
            return result;
        }
    }
}
