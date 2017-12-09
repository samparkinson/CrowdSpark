using CrowdSpark.App.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrowdSpark.App.ViewModels
{
    class SearchPageViewModel : BaseViewModel
    {

        public ObservableCollection<ProjectViewModel> Projects { get; set; }
        
        public void Initialize(string Query)
        {

        }


    }
}
