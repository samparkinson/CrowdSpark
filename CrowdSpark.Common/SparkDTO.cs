﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CrowdSpark.Common
{
    public class SparkDTO
    {
        [Required]
        public int PId { get; set; }
    }
}
