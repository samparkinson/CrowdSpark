using CrowdSpark.Entitites;
using System.Collections.Generic;

namespace CrowdSpark.Common
{
    public class UserDTO
    {
        public int Id { get; set; }
        
        public string Firstname { get; set; }
        
        public string Surname { get; set; }
        
        public string Mail { get; set; }

        public int? LocationId { get; set; }

        public Location Location { get; set; }
        
        public ICollection<Spark> Sparks { get; set; }
    }
}
