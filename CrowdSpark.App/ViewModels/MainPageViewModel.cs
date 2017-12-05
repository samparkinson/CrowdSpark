using CrowdSpark.Common;
using CrowdSpark.Entitites;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CrowdSpark.App.ViewModels
{
    class MainPageViewModel : BaseViewModel
    {
        //get values from db
        public ObservableCollection<ProjectViewModel> Projects { get; set; }

        //dummy items
        public ObservableCollection<string> ProjectNames = new ObservableCollection<string>();
        public ObservableCollection<string> listItems = new ObservableCollection<string>();
        
        public MainPageViewModel()
        {
            Projects = new ObservableCollection<ProjectViewModel>();

            initDummy();
        }
       
        private void initDummy()
        {
            var _location = new Location { Id = 1, City = "Copengahen", Country = "Denmark" };

            var dummyProjects = new List<ProjectDTO>();
            
            for (int i = 0; i < 20; i++)
            {
                dummyProjects.Add(new ProjectDTO { Id = i, Title = "Project " + i, Location = _location, Description = "Desc " + i });
            }

            foreach (var p in dummyProjects)
            {
                Projects.Add(new ProjectViewModel(p));
            }
        }
    }
}
