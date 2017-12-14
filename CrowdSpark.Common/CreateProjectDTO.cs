using CrowdSpark.Entitites;
using System.Collections.Generic;
using System;

namespace CrowdSpark.Common
{
    public class CreateProjectDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public Location Location { get; set; }

        public ICollection<Skill> Skills { get; set; }

        public ICollection<Spark> Sparks { get; set; }

        public Category Category { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
