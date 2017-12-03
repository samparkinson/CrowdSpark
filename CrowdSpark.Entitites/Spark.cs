namespace CrowdSpark.Entitites
{
    public class Spark
    {
        public int PostId { get; set; }
        public int UserId { get; set; }

        public Project Post { get; set; }
        public User User { get; set; }

        public int Status { get; set; }
    }

}
