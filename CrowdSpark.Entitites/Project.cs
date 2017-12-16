using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CrowdSpark.Entitites
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public int? LocationId { get; set; }

        public Location Location { get; set; }

        public ICollection<Skill> Skills { get; set; }

        public ICollection<Spark> Sparks { get; set; }

        public Category Category { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public int CreatorId { get; set; }

        [Required]
        public User Creator { get; set; }
    } 
}
