using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace CrowdSpark.App.Converters
{
    class BorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        { 
            return new SolidColorBrush((value is String s && !String.IsNullOrWhiteSpace(s)) ? Colors.Blue : Colors.Red);
        }   

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
