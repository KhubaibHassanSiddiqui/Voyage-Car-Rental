namespace Project.Models.Domains
{
    public class Car
    {
        public int Id { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public int year { get; set; }
        public string fuelType { get; set; }
        public string category { get; set; }
        public float pricePerDay { get; set; }
        public string imageUrl { get; set; }
        public int isAvailable { get; set; }
    }

}
