using CrowdSpark.Entitites;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace CrowdSpark.Common
{
    public class CreateProjectDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public Location Location { get; set; }

        public ICollection<SkillDTO> Skills { get; set; }

        public CategoryDTO Category { get; set; }
    }
}
