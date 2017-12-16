using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CrowdSpark.Common
{
    public class UserCreateDTO
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

        public LocationDTO Location { get; set; }

        public ICollection<SkillDTO> Skills { get; set; }
    }
}
