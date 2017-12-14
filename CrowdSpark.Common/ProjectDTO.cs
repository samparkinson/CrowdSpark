﻿using CrowdSpark.Entitites;
using System.Collections.Generic;
using System;

namespace CrowdSpark.Common
{
    public class ProjectDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Location Location { get; set; }

        public ICollection<SkillDTO> Skills { get; set; }

        public ICollection<SparkDTO> Sparks { get; set; }

        public CategoryDTO Category { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
