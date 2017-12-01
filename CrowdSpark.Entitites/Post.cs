using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CrowdSpark.Entitites
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public string Description { get; set; }
        
        public int? LocationId { get; set; }

        public Location Location { get; set; }

        public ICollection<Spark> Sparks { get; set; }
    }

}
