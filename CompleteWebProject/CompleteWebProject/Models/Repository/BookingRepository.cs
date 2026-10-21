using Project.Models.Domains;
using Project.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Project.Models.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IRepository<Booking> _bookingRepository;

        private readonly IRepository<Car> _carRepository;

        public BookingRepository(IRepository<Booking> bookingRepository, IRepository<Car> carRepository)
        {
            _bookingRepository = bookingRepository;

            _carRepository = carRepository;
        }
        public void Add(Booking booking)
        {
            _bookingRepository.Add(booking);
        }

        public void Update(Booking booking)
        {
            _bookingRepository.Update(booking);
        }

        public void Delete(int id)
        {
            _bookingRepository.Delete(id);
        }

        public Booking GetById(dynamic id)
        {
            return _bookingRepository.GetById(id);
        }

        public List<Booking> GetAll()
        {
            return _bookingRepository.GetAll();
        }

        public float calculateTotalPrice(Booking booking)
        {

            TimeSpan rentalDuration = booking.endDate - booking.startDate;

            double totalDays = rentalDuration.TotalDays;

            var car = _carRepository.GetById(booking.carId);

            return  (float)(totalDays * car.pricePerDay);
        }

        public List<Booking> GetBookingsByUser(dynamic id)
        {
            var bookings=new List<Booking>();

            foreach(var booking in _bookingRepository.GetAll())
            {
                if(booking.userId==id)
                    bookings.Add(booking);
            }

            return bookings;
        }
    }
}
