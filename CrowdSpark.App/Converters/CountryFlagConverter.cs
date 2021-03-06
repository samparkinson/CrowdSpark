﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                return default(BitmapImage);
            }

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            //value is the country name
            CultureInfo cInfo = cultures.FirstOrDefault(culture => new RegionInfo(culture.LCID).EnglishName == value.ToString());

            if (cInfo != null)
            {
                string CountryCode = cInfo.Name.Split("-")[1].ToLower();

                var fileLocation = new Uri(String.Format(@"ms-appx:Assets\flags\{0}.png", CountryCode));

                if (Regex.IsMatch(CountryCode, "[A-Za-z]{2}"))
                {
                    return new BitmapImage(fileLocation);
                }
                    return new BitmapImage(new Uri(String.Format(@"ms-appx:Assets\icons\help.png")));
            }
            return default(BitmapImage);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
