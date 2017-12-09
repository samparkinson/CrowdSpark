using CrowdSpark.Common;
using CrowdSpark.Entitites;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private Location _location;
        public Location Location { get => _location; set { if (!value.Equals(_location)) { _location = value; OnPropertyChanged(); } } }

        public ImageSource CountryFlag { get; set; }

        public void Initialize(UserViewModel userViewModel)
        {
            Firstname = userViewModel.Firstname;

            Surname = userViewModel.Surname;

            Mail = userViewModel.Mail;

            Location = userViewModel.Location;

            CountryFlag = GetCountryFlag(Location.Country);

            /*Firstname = "Firstname";
            Surname = "Surname";
            Mail = "mail@itu.dk";
            var _location = new Location { Id = 1, City = "Copenhagen", Country = "Denmark" };
            Location = _location;
            CountryFlag = GetCountryFlag(Location.Country); // dummy*/
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
