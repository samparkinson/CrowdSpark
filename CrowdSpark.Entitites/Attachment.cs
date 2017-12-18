using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CrowdSpark.Entitites
{
    public partial class Attachment
    {
        public int Id { get; set; }

        public string Description { get; set; }

        [Required]
        public string Data { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public Project Project { get; set; }
    }
}
