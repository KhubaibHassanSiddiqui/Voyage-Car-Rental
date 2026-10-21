using Project.Models.Domains;

namespace Project.Models.Interfaces
{
    public interface IBookingRepository:IRepository<Booking>
    {
        float calculateTotalPrice(Booking booking);
        List<Booking> GetBookingsByUser(dynamic id);
    }

}
