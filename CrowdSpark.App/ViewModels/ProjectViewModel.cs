﻿using CrowdSpark.Common;
using CrowdSpark.Entitites;
using System.Collections.Generic;

namespace CrowdSpark.App.ViewModels
{
    class ProjectViewModel : BaseViewModel
    {
        private int _id;
        public int Id { get => _id; set { if (value != _id) { _id = value; OnPropertyChanged(); } } }

        private string _title;
        public string Title { get => _title; set { if (value != _title) { _title = value; OnPropertyChanged(); } } }

        private Location _location;
        public Location Location { get => _location; set { if (!value.Equals(_location)) { _location = value; OnPropertyChanged(); } } }

        public string _description;
        public string Description { get => _description; set { if (value != _description) { _description = value; OnPropertyChanged(); } } }

        public ICollection<Spark> _sparks;
        public ICollection<Spark> Sparks { get => _sparks; set { if (!value.Equals(_sparks)) { _sparks = value; OnPropertyChanged(); } } }

        public ProjectViewModel(ProjectDTO ProjectDTO)
        {
            Id = ProjectDTO.Id;

            Title = ProjectDTO.Title;

            Location = ProjectDTO.Location;

            Description = ProjectDTO.Description;

            //Sparks = PostDTO.Sparks; --??
        }
    }
}