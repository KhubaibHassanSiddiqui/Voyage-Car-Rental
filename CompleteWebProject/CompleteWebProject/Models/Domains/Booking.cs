namespace Project.Models.Domains
{
	public class Booking
	{
		public int Id { get; set; }
		public int carId { get; set; }
		public string userId { get; set; }
		public string pickup {  get; set; }
		public string dropoff { get; set; }
		public DateTime startDate { get; set; }
		public DateTime endDate { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public float totalPrice { get; set; }
		public int isPaid { get; set; }
	}

}
