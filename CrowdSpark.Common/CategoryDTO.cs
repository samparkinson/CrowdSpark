using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CrowdSpark.Common
{
    public class CategoryDTO
    {
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
    }
}
