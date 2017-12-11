using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace CrowdSpark.App.Converters
{
    class CountryFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            //value is the country name
            CultureInfo cInfo = cultures.FirstOrDefault(culture => new RegionInfo(culture.LCID).EnglishName == value.ToString());

            string CountryCode = cInfo.Name.Split("-")[1].ToLower();
            
            var fileLocation = new Uri(String.Format(@"ms-appx:Assets\flags\{0}.png", CountryCode));

            return new BitmapImage(fileLocation);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
