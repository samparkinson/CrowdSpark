using CrowdSpark.App.Helpers;
using CrowdSpark.App.Models;
using CrowdSpark.Common;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
        public LocationDTO Location { get => _location; set { if (!value.Equals(_location)) { _location = value; OnPropertyChanged(); } } }

        public ImageSource CountryFlag { get; set; }

        private readonly IAuthenticationHelper helper;

        //List of skills 
        public ObservableCollection<SkillDTO> Skills { get; set; }

        public UserPageViewModel(IAuthenticationHelper _helper)
        {
            MenuOptions = CommonAttributes.MenuOptions;

            helper = _helper;

            SignInOutCommand = new RelayCommand(async o =>
            {
                if (CommonAttributes.account != null)
                {
                    await helper.SignOutAsync(CommonAttributes.account);
                    CommonAttributes.account = null;
                    // Characters.Clear();
                }
                else
                {
                    CommonAttributes.account = await helper.SignInAsync();
                    if (CommonAttributes.account != null)
                    {
                        Debug.WriteLine("Sign in successfull");
                        //Initialize();
                    }
                }
            });
        }

        public void Initialize(UserViewModel userViewModel)
        {
            Firstname = userViewModel.Firstname;

            Surname = userViewModel.Surname;

            Mail = userViewModel.Mail;

            Location = userViewModel.Location;

            //Not sure
            Skills = (ObservableCollection<SkillDTO>) userViewModel.Skills;

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
