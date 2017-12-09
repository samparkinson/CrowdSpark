using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace CrowdSpark.App.Helpers
{
    //To store account and options
    static class CommonAttributes
    {
        public static WebAccount account { get; set; }

        //Options for hamburger menu, every page has a hamburger menu
        public static ObservableCollection<MenuOption> MenuOptions { get; set; }
    }
}
