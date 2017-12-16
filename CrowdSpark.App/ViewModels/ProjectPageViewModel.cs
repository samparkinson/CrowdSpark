using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace CrowdSpark.App.ViewModels
{
    class ProjectPageViewModel : BaseViewModel
    {
        private int _id;
        public int Id { get => _id; set { if (value != _id) { _id = value; OnPropertyChanged(); } } }

        private string _title;
        public string Title { get => _title; set { if (value != _title) { _title = value; OnPropertyChanged(); } } }

        private LocationDTO _location;
        public LocationDTO Location { get => _location; set { if (!value.Equals(_location)) { _location = value; OnPropertyChanged(); } } }
        
        public string _description;
        public string Description { get => _description; set { if (value != _description) { _description = value; OnPropertyChanged(); } } }

        public CategoryDTO _category;
        public CategoryDTO Category { get => _category; set { if (value != _category) { _category = value; OnPropertyChanged(); } } }

        public string _categoryString;
        public string CategoryString { get => _categoryString; set { if (value != _categoryString) { _categoryString = value; OnPropertyChanged(); } } }

        public ICollection<SkillDTO> _skills { get; set; }
        public ICollection<SkillDTO> Skills { get => _skills; set { if (!value.Equals(_skills)) { _skills = value; OnPropertyChanged(); } } }

        public ICollection<SparkDTO> _sparks;
        public ICollection<SparkDTO> Sparks { get => _sparks; set { if (!value.Equals(_sparks)) { _sparks = value; OnPropertyChanged(); } } }

        //Command to initialize the login on app opening
        private ICommand SignInCommand { get; set; }

        private readonly IAuthenticationHelper helper;
        
        public ProjectPageViewModel(IAuthenticationHelper _helper)
        {
            helper = _helper;
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

        public void Initialize(ProjectViewModel projectViewModel)
        {
            Id = projectViewModel.Id;

            Title = projectViewModel.Title;

            Description = projectViewModel.Description;

            Location = projectViewModel.Location;

            //Skills = projectViewModel.Skills;

            Category = projectViewModel.Category;

            CategoryString = Category.Name.ToUpper();

            account = CommonAttributes.account;
            MenuOptions = CommonAttributes.MenuOptions;
        }
    }
}
