using CrowdSpark.Entitites;
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

        public Location Location { get; set; }

        public ICollection<Skill> Skills { get; set; }

        public ICollection<Spark> Sparks { get; set; }

        public CurrentUser()
        {

        }
    }
}
