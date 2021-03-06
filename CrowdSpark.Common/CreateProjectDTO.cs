﻿using System.Collections.Generic;
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

        public LocationDTO Location { get; set; }

        public CategoryDTO Category { get; set; }
    }
}
