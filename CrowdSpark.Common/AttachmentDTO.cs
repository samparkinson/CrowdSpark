using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CrowdSpark.Common
{
    public class AttachmentDTO
    {
        public int Id { get; set; }

        public string Description { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string Data { get; set; }

        [Required]
        public int Type { get; set; }
    }
}
