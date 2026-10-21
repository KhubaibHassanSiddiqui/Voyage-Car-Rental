namespace Project.Models.Domains
{
    public class ViewModel
    {
        public Booking booking {  get; set; }
        public Car car { get; set; }
        public AspNetUsers user { get; set; }
        public Review review { get; set; }
        public Payment payment { get; set; }

    }
}
