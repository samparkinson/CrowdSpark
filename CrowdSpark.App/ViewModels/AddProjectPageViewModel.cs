using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.App.Views;
using CrowdSpark.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Input;
using Windows.Storage;

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
        private readonly ISkillAPI skillAPI;
        private readonly IAttachmentAPI attachmentAPI;
        private readonly ILocationAPI locationAPI;

        public AddProjectPageViewModel(IProjectAPI _projectAPI, IAuthenticationHelper _helper, INavigationService _service, ILocationAPI _locationAPI, ISkillAPI _skillAPI, IAttachmentAPI _attachmentAPI)
        {
            projectAPI = _projectAPI;
            helper = _helper;
            service = _service;
            skillAPI = _skillAPI;
            attachmentAPI = _attachmentAPI;
            locationAPI = _locationAPI;
            account = CommonAttributes.account;
            UserName = account.UserName;

            SignInOutButtonText = account == null ? "Sign In" : "Sign Out";
            
            //init countries list
            Countries = new ObservableCollection<string>(GetCountryList());

            SignInOutCommand = new RelayCommand(async o =>
            {
                //sign out
                if (account != null)
                {
                    await helper.SignOutAsync(account);
                    account = null;
                    CommonAttributes.account = account;
                    SignInOutButtonText = "Sign In";
                    service.Navigate(typeof(LogInPage), null);
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
            return null;
        }

        public async Task<bool> PostProject(CreateProjectDTO createProjectDTO, List<SkillCreateDTO> SkillsList, List<StorageFile> attachments)
        {
            if (account != null)
            {
                LocationCreateDTO locationCreateDTO = new LocationCreateDTO { Country = createProjectDTO.Location.Country, City = createProjectDTO.Location.City };
                var locationID = await locationAPI.Create(locationCreateDTO);
                LocationDTO locationDTO = new LocationDTO { Country = createProjectDTO.Location.Country, City = createProjectDTO.Location.City, Id = locationID };
                createProjectDTO.Location = locationDTO;

                //this should return the id
                var result = await projectAPI.Create(createProjectDTO);

                foreach (SkillCreateDTO skillCreateDTO in SkillsList)
                {
                    var skillID = await skillAPI.Create(skillCreateDTO);

                    //replace with actual id 
                    SkillDTO skillDTO = new SkillDTO { Name = skillCreateDTO.Name, Id = skillID };
                    await projectAPI.AddSkill(result, skillDTO);
                }

                foreach (StorageFile file in attachments)
                {
                    byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);

                    string serializedData = Convert.ToBase64String(imageArray);

                    var attachment = new AttachmentCreateDTO { Data = serializedData, Description = file.Name, ProjectId = result, Type = (int)AttachmentTypes.BITMAP };
                    await attachmentAPI.Create(attachment);
                }
                
                if (result != -1)
                {
                    service.Navigate(typeof(ProjectPage), new ProjectViewModel(createProjectDTO));
                }
                return true;
            }
            return false;
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
