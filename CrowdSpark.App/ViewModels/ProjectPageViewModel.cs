using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.App.Views;
using CrowdSpark.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Drawing;


namespace CrowdSpark.App.ViewModels
{
    class ProjectPageViewModel : BaseViewModel
    {
        private int _id;
        public int Id { get => _id; set { if (value != _id) { _id = value; OnPropertyChanged(); } } }

        private string _title;
        public string Title { get => _title; set { if (value != _title) { _title = value; OnPropertyChanged(); } } }

        private LocationDTO _location;
        public LocationDTO Location { get => _location; set { if (value != null && !value.Equals(_location)) { _location = value; OnPropertyChanged(); } } }
        
        public string _description;
        public string Description { get => _description; set { if (value != _description) { _description = value; OnPropertyChanged(); } } }

        public CategoryDTO _category;
        public CategoryDTO Category { get => _category; set { if (value != _category) { _category = value; OnPropertyChanged(); } } }

        public string _categoryString;
        public string CategoryString { get => _categoryString; set { if (value != _categoryString) { _categoryString = value; OnPropertyChanged(); } } }

        public ICollection<SkillDTO> _skills { get; set; }
        public ICollection<SkillDTO> Skills { get => _skills; set { if (value != null && !value.Equals(_skills)) { _skills = value; OnPropertyChanged(); } } }

        public ICollection<SparkDTO> _sparks;
        public ICollection<SparkDTO> Sparks { get => _sparks; set { if (!value.Equals(_sparks)) { _sparks = value; OnPropertyChanged(); } } }

        public ICollection<AttachmentDTO> _attachments { get; set; }
        public ICollection<AttachmentDTO> Attachments { get => _attachments; set { if (value != _attachments) { _attachments = value; OnPropertyChanged(); } } }
        //public ICollection<Image> Images { get; set; }

        //Command to initialize the login on app opening
        private ICommand SignInCommand { get; set; }

        private readonly IAuthenticationHelper helper;
        private readonly IProjectAPI projectAPI;
        private readonly INavigationService service;

        public ProjectPageViewModel(IAuthenticationHelper _helper, IProjectAPI _projectAPI, INavigationService _service, IAttachmentAPI _attachmentAPI)
        {
            helper = _helper;
            projectAPI = _projectAPI;
            service = _service;
            account = CommonAttributes.account;
            UserName = account.UserName;
            
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
                        CommonAttributes.account = account;

                        UserName = account.UserName;

                        SignInOutButtonText = "Sign Out";
                    }
                }
            });

            MenuOptions = new HamburgerMenuOptionsFactory(account).MenuOptions;
        }

        public async void Initialize(ProjectViewModel projectViewModel)
        {
            ProjectDTO realProject = new ProjectDTO();
            realProject = await projectAPI.Get(projectViewModel.Id);

            Id = realProject.Id;

            Title = realProject.Title;

            Description = realProject.Description;

            Location = realProject.Location;

            Skills = realProject.Skills;

            Category = realProject.Category;

            CategoryString = realProject.Title.ToUpper();

            Attachments = projectViewModel.Attachments;

            account = CommonAttributes.account;
            MenuOptions = CommonAttributes.MenuOptions;

            
            /*for( )
            {
                var img = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(Attachments.)));
            }*/
        }

        public async Task<bool> SparkProject()
        {
            return await projectAPI.CreateSpark(Id);
        }
    }
}
