using CrowdSpark.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrowdSpark.App.Models
{
    class CurrentUser
    {
        public int Id { get; set; }

        //For auth 
        public string Token { get; set; }
        
        public string Firstname { get; set; }
        
        public string Surname { get; set; }
        
        public string Mail { get; set; }

        public int? LocationId { get; set; }

        public LocationDTO Location { get; set; }

        public ICollection<SkillDTO> Skills { get; set; }

        public ICollection<SparkDTO> Sparks { get; set; }

        public CurrentUser()
        {

        }
    }
}
