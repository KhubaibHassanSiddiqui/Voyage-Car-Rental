namespace Project.Models.Domains
{
    public class ViewLists
    {
        public List<Booking> bookList { get; set; }
        public List<Car> carList { get; set; }
        public List<AspNetUsers> userList { get; set; }
        public List<Review> reviewList { get; set; }
        public List<Payment> paymentList { get; set; }
    }
}
