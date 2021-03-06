﻿using CrowdSpark.Common;
using System;
using System.Collections.Generic;

namespace CrowdSpark.App.ViewModels
{
    class UserViewModel : BaseViewModel
    {
        private int _id;
        public int Id { get => _id; set { if (value != _id) { _id = value; OnPropertyChanged(); } } }

        private string _firstname;
        public string Firstname { get => _firstname; set { if (value != _firstname) { _firstname = value; OnPropertyChanged(); } } }

        private string _surname;
        public string Surname { get => _surname; set { if (value != _surname) { _surname = value; OnPropertyChanged(); } } }

        private string _mail;
        public string Mail { get => _mail; set { if (value != _mail) { _mail = value; OnPropertyChanged(); } } }

        private LocationDTO _location;
        public LocationDTO Location { get => _location; set { if (!value.Equals(_location)) { _location = value; OnPropertyChanged(); } } }

        private ICollection<SparkDTO> _sparks;
        public ICollection<SparkDTO> Sparks { get => _sparks; set { if (!value.Equals(_sparks)) { _sparks = value; OnPropertyChanged(); } } }

        private ICollection<SkillDTO> _skills;
        public ICollection<SkillDTO> Skills { get => _skills; set { if (!value.Equals(_skills)) { _skills = value; OnPropertyChanged(); } } }

        public UserViewModel(UserDTO UserDTO)
        {
            Firstname = UserDTO.Firstname;

            Surname = UserDTO.Surname;

            Mail = UserDTO.Mail;

            Location = UserDTO.Location;

            Skills = UserDTO.Skills;
        }
    }
}
