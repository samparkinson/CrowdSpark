﻿using System;
using System.Collections.Generic;
using CrowdSpark.Entitites;
using System.ComponentModel.DataAnnotations;

namespace CrowdSpark.Common
{
    public class ProjectSummaryDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public string Description { get; set; }

        public int? LocationId { get; set; }

        public CategoryDTO Category { get; set; }
    }
}
