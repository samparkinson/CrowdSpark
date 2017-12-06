﻿using CrowdSpark.Common;
using CrowdSpark.Entitites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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

        //public string City { get; set; }
        //public string Country { get; set; }

        public string _description;
        public string Description { get => _description; set { if (value != _description) { _description = value; OnPropertyChanged(); } } }

        public ICollection<Spark> _sparks;
        public ICollection<Spark> Sparks { get => _sparks; set { if (!value.Equals(_sparks)) { _sparks = value; OnPropertyChanged(); } } }

        public string LogStatusText { get; set; }

        public ImageSource CountryFlag { get; set; }

        public void Initialize(ProjectViewModel projectViewModel)
        {

            Id = projectViewModel.Id;

            Title = projectViewModel.Title;

            Description = projectViewModel.Description;

            Location = projectViewModel.Location;

            //City = Location.City;
            //Country = Location.Country;

            LogStatusText = "Log Out";

            CountryFlag = GetCountryFlag(Location.Country);
        }

        private ImageSource GetCountryFlag(string Country)
        {
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            CultureInfo cInfo = cultures.FirstOrDefault(culture => new RegionInfo(culture.LCID).EnglishName == Country);

            string CountryCode = cInfo.Name.Split("-")[1].ToLower();

            var fileLocation = new Uri(String.Format(@"ms-appx:Assets\flags\{0}.png", CountryCode));
            
            return new BitmapImage(fileLocation);
        }
    }
}
