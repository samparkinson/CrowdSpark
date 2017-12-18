using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrowdSpark.Entitites
{
    public partial class Skill
    {
        public Skill()
        {
            Users = new HashSet<User>();
            Projects = new HashSet<Project>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}
