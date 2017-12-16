using System;

namespace CrowdSpark.Entitites
{
    public class Spark
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }

        public Project Project { get; set; }
        public User User { get; set; }

        public int Status { get; set; }

        public DateTime CreatedDate { get; set; }
    }

}
