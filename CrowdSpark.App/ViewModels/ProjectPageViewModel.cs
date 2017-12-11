using CrowdSpark.App.Helpers;
using CrowdSpark.Entitites;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        private Location _location;
        public Location Location { get => _location; set { if (!value.Equals(_location)) { _location = value; OnPropertyChanged(); } } }
        
        public string _description;
        public string Description { get => _description; set { if (value != _description) { _description = value; OnPropertyChanged(); } } }

        public Category _category;
        public Category Category { get => _category; set { if (value != _category) { _category = value; OnPropertyChanged(); } } }

        public string _categoryString;
        public string CategoryString { get => _categoryString; set { if (value != _categoryString) { _categoryString = value; OnPropertyChanged(); } } }

        public ICollection<Skill> _skills { get; set; }
        public ICollection<Skill> Skills { get => _skills; set { if (!value.Equals(_skills)) { _skills = value; OnPropertyChanged(); } } }

        public ICollection<Spark> _sparks;
        public ICollection<Spark> Sparks { get => _sparks; set { if (!value.Equals(_sparks)) { _sparks = value; OnPropertyChanged(); } } }
        
        public void Initialize(ProjectViewModel projectViewModel)
        {
            Id = projectViewModel.Id;

            Title = projectViewModel.Title;

            Description = projectViewModel.Description;

            Location = projectViewModel.Location;

            //Skills = projectViewModel.Skills;

            Category = projectViewModel.Category;

            CategoryString = Category.Name.ToUpper();

            MenuOptions = CommonAttributes.MenuOptions;
        }
    }
}
