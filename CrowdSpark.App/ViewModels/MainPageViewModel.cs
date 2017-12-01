using CrowdSpark.Entitites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrowdSpark.App.ViewModels
{
    class MainPageViewModel : BaseViewModel
    {
        public ObservableCollection<ProjectPageViewModel> Projects { get; private set; }
        public ObservableCollection<string> ProjectNames { get; set; }
        private IProjectsRepository _repository;

        public MainPageViewModel(ICrowdSparkContext repository)
        {
            _repository = repository;
            Projects = repository.
        }

        public 
    }
}
