using CrowdSpark.Common;
using CrowdSpark.Entitites;
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
            var _projects = new[] { new ProjectDTO { Id = 1, Title = "Project1", Location = _location, Description = "Desc 1"},
            new ProjectDTO { Id = 2, Title = "Project2", Location = _location, Description = "Desc 2"},
            new ProjectDTO { Id = 3, Title = "Project3", Location = _location, Description = "Desc 3"}};

            foreach (var p in _projects)
            {
                Projects.Add(new ProjectViewModel(p));
            }
        }
    }
}
