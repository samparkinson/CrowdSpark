using CrowdSpark.App.Converters;
using CrowdSpark.App.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Data;

namespace CrowdSpark.App.ViewModels
{
    class AddProjectPageViewModel : BaseViewModel
    {
        public ObservableCollection<string> Countries { get; set; }
        
        public ObservableCollection<string> Cities { get; set; }

        public ICommand PostProjectCommand { get; set; }

        public AddProjectPageViewModel()
        {
            Countries = new ObservableCollection<string>();
            Cities = new ObservableCollection<string>();

            Countries.Add("Denmark");
            Countries.Add("Turkey");
            Countries.Add("Germany");
            Countries.Add("Sweden");

            Cities.Add("Copenhagen");
            Cities.Add("Ankara");
            Cities.Add("Malmø");
            
            MenuOptions = new HamburgerMenuOptionsFactory(CommonAttributes.account).MenuOptions;
        }
    }
}
