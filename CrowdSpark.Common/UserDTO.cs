using CrowdSpark.Entitites;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CrowdSpark.Common
{
    public class UserDTO
    {
        [Required]
        [StringLength(30)]
        public string Firstname { get; set; }

        [Required]
        [StringLength(30)]
        public string Surname { get; set; }

        [Required]
        [StringLength(255)]
        public string Mail { get; set; }

        public Location Location { get; set; }

        public ICollection<Skill> Skills { get; set; }
    }
}
