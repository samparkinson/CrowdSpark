﻿using CrowdSpark.Common;
using System;
using System.Collections.Generic;

namespace CrowdSpark.App.ViewModels
{
    class ProjectViewModel : BaseViewModel
    {
        private int _id;
        public int Id { get => _id; set { if (value != _id) { _id = value; OnPropertyChanged(); } } }

        private string _title;
        public string Title { get => _title; set { if (value != null && value != _title) { _title = value; OnPropertyChanged(); } } }

        private LocationDTO _location;
        public LocationDTO Location { get => _location; set { if (value != null && !value.Equals(_location)) { _location = value; OnPropertyChanged(); } } }

        public string _description;
        public string Description { get => _description; set { if (value != null && value != _description) { _description = value; OnPropertyChanged(); } } }

        public CategoryDTO _category;
        public CategoryDTO Category{ get => _category; set { if (value != null && value != _category) { _category = value; OnPropertyChanged(); } } }
        
        public ICollection<SkillDTO> _skills { get; set; }
        public ICollection<SkillDTO> Skills { get => _skills; set { if (value != null && !value.Equals(_skills)) { _skills = value; OnPropertyChanged(); } } }

        public ICollection<SparkDTO> _sparks;
        public ICollection<SparkDTO> Sparks { get => _sparks; set { if (value != null && !value.Equals(_sparks)) { _sparks = value; OnPropertyChanged(); } } }

        public ICollection<AttachmentDTO> _attachments { get; set; }
        public ICollection<AttachmentDTO> Attachments { get => _attachments; set { if (value != null && value != _attachments) { _attachments = value; OnPropertyChanged(); } } }

        public DateTime _createdDate;
        public DateTime CreatedDate { get => _createdDate; set { if (value != null && !value.Equals(_createdDate)) { _createdDate = value; OnPropertyChanged(); } } }

        public ProjectViewModel(ProjectDTO ProjectDTO)
        {
            Id = ProjectDTO.Id;

            Title = ProjectDTO.Title;

            Location = ProjectDTO.Location;

            Description = ProjectDTO.Description;

            Skills = ProjectDTO.Skills;

            Category = ProjectDTO.Category;
             
            //Sparks = ProjectDTO.Sparks;
        }

        public ProjectViewModel(ProjectSummaryDTO projectSummaryDTO)
        {
            Id = projectSummaryDTO.Id;

            Title = projectSummaryDTO.Title;

            Description = projectSummaryDTO.Description;

            //Location.Id = projectSummaryDTO.Id;

            Category = projectSummaryDTO.Category;
        }

        public ProjectViewModel(CreateProjectDTO createProjectDTO)
        {
            Title = createProjectDTO.Title;
            Description = createProjectDTO.Description;
            Category = createProjectDTO.Category;
            Location = createProjectDTO.Location;
        }
    }
}
