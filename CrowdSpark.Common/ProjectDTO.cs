using CrowdSpark.Entitites;
using System.Collections.Generic;

namespace CrowdSpark.Common
{
    public class ProjectDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Location Location { get; set; }

        public ICollection<Spark> Sparks { get; set; }
    }
}
