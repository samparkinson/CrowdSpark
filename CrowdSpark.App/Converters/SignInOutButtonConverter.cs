using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace CrowdSpark.App.Converters
{
    class SignInOutButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //change the icons 
            return value.ToString() == "Sign In" ? new BitmapImage(new Uri(@"ms-appx:Assets\icons\login.png")) : new BitmapImage(new Uri(@"ms-appx:Assets\icons\logout.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
