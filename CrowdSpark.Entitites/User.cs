using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CrowdSpark.Entitites
{
    public partial class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Firstname { get; set; }

        [Required]
        [StringLength(30)]
        public string Surname { get; set; }

        [Required]
        [StringLength(255)]
        public string Mail { get; set; }
        
        public int? LocationId { get; set; }

        public Location Location { get; set; }

        public ICollection<UserSkill> Skills { get; set; }

        public ICollection<Spark> Sparks { get; set; }

        [Required]
        [StringLength(60)]
        public string AzureUId { get; set; }
    }
}
