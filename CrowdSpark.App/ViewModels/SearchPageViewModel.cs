using CrowdSpark.App.Helpers;
using CrowdSpark.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CrowdSpark.App.ViewModels
{
    class SearchPageViewModel : BaseViewModel
    {

        public ObservableCollection<ProjectViewModel> Results { get; set; }
        
        public void Initialize(string Query)
        {
            //TODO:get the results from the repo async
            //initDummyProjects();

            Results = new ObservableCollection<ProjectViewModel>();

            MenuOptions = CommonAttributes.MenuOptions;

            renewResults(Query);
        }

        private void initDummyProjects()
        {
            Results.Clear();

            var _location = new LocationDTO { Id = 1, City = "Copenhagen", Country = "Denmark" };

            var _dummyProjects = new List<ProjectDTO>();

            for (int i = 0; i < 20; i++)
            {
                _dummyProjects.Add(new ProjectDTO { Id = i, Title = "Project " + i, Location = _location, Description = "Description " + i, Category = new CategoryDTO { Name = "Programming" } });
            }

            foreach (var p in _dummyProjects)
            {
                Results.Add(new ProjectViewModel(p));
            }
        }

        public void renewResults(string Query)
        {
            //get the new results and write them into Results
            Results.Clear();
            
            //for testing purposes
            var _location = new LocationDTO { Id = 1, City = "Helsinki", Country = "Finland" };

            var _dummyProjects = new List<ProjectDTO>();

            for (int i = 0; i < 20; i++)
            {
                _dummyProjects.Add(new ProjectDTO { Id = i, Title = Query + " " + i, Location = _location, Description = "Description " + i, Category = new CategoryDTO { Name = "Programming" } });
            }

            foreach (var p in _dummyProjects)
            {
                Results.Add(new ProjectViewModel(p));
            }
        }
    }
}
