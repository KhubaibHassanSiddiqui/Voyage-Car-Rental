using Microsoft.AspNetCore.Mvc;
using Project.Models.Domains;
using Project.Models.Interfaces;

namespace Project.Controllers
{
    public class BookingController : Controller
    {
        private IBookingRepository _bookingRepository;
        private ICarRepository _carRepository;
        private IUserRepository _userRepository;
        public BookingController(IBookingRepository bookingRepository, ICarRepository carRepository, IUserRepository userRepository)
        {
            _bookingRepository = bookingRepository;
            _carRepository = carRepository;
            _userRepository = userRepository;
        }
        public IActionResult Index()
        {
            var cars = _carRepository.GetAll().OrderByDescending(c=>c.Id).ToList();
            var users = _userRepository.GetAll();
            string id = HttpContext.Session.GetString("UserId");
            ViewBag.UserId = id;
            var user = users.FirstOrDefault(c => c.Id == id);
            var bookings = _bookingRepository.GetAll();
            ViewBag.bookingCount = bookings.Count(booking => booking.userId == id && booking.isPaid==0);
            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }
            return View(cars);
        }

        [HttpPost]
        public IActionResult sort([FromBody] sortCars sortCars)
        {
            var cars = sortCars.Cars;

            switch (sortCars.sortOrder)
            {
                case "priceLowToHigh":
                    cars=cars.OrderBy(c=> c.pricePerDay).ToList(); 
                    break;

                case "priceHighToLow":
                    cars = cars.OrderByDescending(c => c.pricePerDay).ToList();
                    break;

                case "newest":
                    cars = cars.OrderByDescending(c => c.Id).ToList();
                    break;

                default:
                    break;
            }

            return PartialView("~/Views/Booking/sortedCars.cshtml", cars);
        }

        [HttpPost]
        public IActionResult findCars([FromBody] carSearch car)
        {
            var cars = _carRepository.search(car);

            return PartialView("~/Views/Booking/sortedCars.cshtml", cars);
        }

        [HttpGet]
        public IActionResult Book(int carid)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                HttpContext.Session.SetInt32("Logged",1);
                HttpContext.Session.SetInt32("Booked",carid);
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            var booking = new Booking();

            if (HttpContext.Request.Cookies.ContainsKey("pickup"))
            {
                var pickup = HttpContext.Request.Cookies["pickup"] ?? string.Empty;
                var dropoff = HttpContext.Request.Cookies["dropoff"] ?? string.Empty;

                DateTime startDate = DateTime.Parse(HttpContext.Request.Cookies["startD"]);
                DateTime endDate = DateTime.Parse(HttpContext.Request.Cookies["endD"]);
                DateTime startTime = DateTime.Parse(HttpContext.Request.Cookies["startT"]);
                DateTime endTime = DateTime.Parse(HttpContext.Request.Cookies["endT"]);

                booking = new Booking
                {
                    pickup = pickup,
                    dropoff = dropoff,
                    startDate = startDate,
                    endDate = endDate,
                    startTime = startTime,
                    endTime = endTime,
                    isPaid = 0
                };
            }
            string userid= HttpContext.Session.GetString("UserId");
            booking.carId = carid;
            booking.userId = userid;

            var car = _carRepository.GetById(carid);
            var users = _userRepository.GetAll();
            var user = users.FirstOrDefault(c => c.Id == userid);
            var bookings = _bookingRepository.GetAll();
            ViewBag.bookingCount = bookings.Count(booking => booking.userId == userid && booking.isPaid == 0);

            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }

            var viewModel = new ViewModel { booking = booking, car = car };
            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Book(ViewModel model)
        {
            var booking = model.booking;

            DateOnly startD = DateOnly.FromDateTime(booking.startDate);
            DateOnly endD = DateOnly.FromDateTime(booking.endDate);
            TimeOnly startT = TimeOnly.FromDateTime(booking.startTime);
            TimeOnly endT = TimeOnly.FromDateTime(booking.endTime);

            DateTime startDateTime = startD.ToDateTime(startT);
            DateTime endDateTime = endD.ToDateTime(endT);

            booking.totalPrice = _bookingRepository.calculateTotalPrice(booking);
            booking.startDate = startDateTime;
            booking.endDate = endDateTime;
            booking.isPaid = 0;

            var car = model.car;
            car.isAvailable = 0;
            _carRepository.Update(car);
            _bookingRepository.Add(booking);

            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            };

            HttpContext.Response.Cookies.Append("pickup", booking.pickup, options);
            HttpContext.Response.Cookies.Append("dropoff", booking.dropoff, options);
            HttpContext.Response.Cookies.Append("startD", booking.startDate.ToString("yyyy-MM-dd"), options);
            HttpContext.Response.Cookies.Append("endD", booking.endDate.ToString("yyyy-MM-dd"), options);
            HttpContext.Response.Cookies.Append("startT", booking.startTime.ToString("HH:mm"), options);
            HttpContext.Response.Cookies.Append("endT", booking.endTime.ToString("HH:mm"), options);

            return RedirectToAction("Index");
        }

        public IActionResult Cart()
        {
            string id = HttpContext.Session.GetString("UserId");
            var bookingsList=_bookingRepository.GetBookingsByUser(id);
            List<Booking> bookings = bookingsList.Where(b => b.isPaid == 0).ToList();
            var cars=new List<Car>();

            foreach(var booking in bookings)
            {
                var newcar = _carRepository.GetById(booking.carId);
                cars.Add(newcar);
            }
            var users = _userRepository.GetAll();
            var user = users.FirstOrDefault(c => c.Id == id);
            var book = _bookingRepository.GetAll();
            ViewBag.bookingCount = bookings.Count(booking => booking.userId == id && booking.isPaid == 0);
            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }

            var viewLists= new ViewLists { bookList = bookings, carList=cars };

            return View(viewLists);
        }
        public IActionResult cancelBooking(int id)
        {
            var booking = _bookingRepository.GetById(id);
            var car=_carRepository.GetById(booking.carId);
            car.isAvailable = 1;
            _carRepository.Update(car);
            _bookingRepository.Delete(id);

            return RedirectToAction("Index");
        }
    }
}
