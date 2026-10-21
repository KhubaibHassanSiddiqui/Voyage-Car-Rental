namespace Project.Models.Domains
{
    public class Payment
    {
        public int Id { get; set; }
        public int bookingId { get; set; }
        public string userId { get; set; }
        public string name { get; set; }
        public string method { get; set; }
        public string expiryDate { get; set; }
        public float amount { get; set; }
        public string creditCardNo { get; set; }
        public int CVV { get; set; }
    }
}
