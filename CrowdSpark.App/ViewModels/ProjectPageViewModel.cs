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

        public ICollection<Spark> _sparks;
        public ICollection<Spark> Sparks { get => _sparks; set { if (!value.Equals(_sparks)) { _sparks = value; OnPropertyChanged(); } } }
        
        public ImageSource CountryFlag { get; set; }
        
        public void Initialize(ProjectViewModel projectViewModel)
        {
            Id = projectViewModel.Id;

            Title = projectViewModel.Title;

            Description = projectViewModel.Description;

            Location = projectViewModel.Location;
            
            CountryFlag = GetCountryFlag(Location.Country);

            MenuOptions = CommonAttributes.MenuOptions;
        }

        private ImageSource GetCountryFlag(string Country)
        {
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            CultureInfo cInfo = cultures.FirstOrDefault(culture => new RegionInfo(culture.LCID).EnglishName == Country);

            string CountryCode = cInfo.Name.Split("-")[1].ToLower();

            //get from db?
            var fileLocation = new Uri(String.Format(@"ms-appx:Assets\flags\{0}.png", CountryCode));
            
            return new BitmapImage(fileLocation);
        }
    }
}
