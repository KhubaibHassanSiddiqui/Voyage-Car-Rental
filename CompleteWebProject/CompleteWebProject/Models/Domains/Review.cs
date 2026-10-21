namespace Project.Models.Domains
{
    public class Review
    {
        public int Id { get; set; }
        public string userId { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public int rating { get; set; }
    }

}
