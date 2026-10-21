using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Models.Domains;
using Project.Models.Interfaces;
using System.Linq;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarRepository _carRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IReviewRepository _reviewRepository;

        public HomeController(ICarRepository carRepository, IBookingRepository bookingRepository, IUserRepository userRepository, IPaymentRepository paymentRepository, IReviewRepository reviewRepository)
        {
            _carRepository = carRepository;
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cars = _carRepository.GetAll();
            var users = _userRepository.GetAll();
            string id = HttpContext.Session.GetString("UserId");
            var bookings=_bookingRepository.GetAll();
            var payments=_paymentRepository.GetAll();
            var reviews = _reviewRepository.GetAll();
            ViewBag.bookingCount = bookings.Count(booking => booking.userId == id && booking.isPaid == 0);
            var user=users.FirstOrDefault(c=>c.Id == id);
            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }
            var viewLists=new ViewLists { bookList=bookings, carList=cars, paymentList=payments, userList=users, reviewList=reviews };
            return View(viewLists);
        }

        [HttpPost]
        public IActionResult Index(Booking booking)
        {
            HttpContext.Session.SetString("pickup", booking.pickup);
            HttpContext.Session.SetString("dropoff", booking.dropoff);
            HttpContext.Session.SetString("startDate", booking.startDate.ToString());
            HttpContext.Session.SetString("endDate", booking.endDate.ToString());
            HttpContext.Session.SetString("startTime", booking.startTime.ToString());
            HttpContext.Session.SetString("endTime", booking.endTime.ToString());

            if (HttpContext.Session.GetString("UserId") == null)
            {
                HttpContext.Session.SetInt32("Logged", 3);
                return RedirectToAction("Login", "Account");
            }

            else
            {
                return RedirectToAction("Booking");
            }
        }
        public IActionResult Booking()
        {
            var cars = _carRepository.GetAll().OrderByDescending(c => c.Id).ToList();
            var users = _userRepository.GetAll();
            string id = HttpContext.Session.GetString("UserId");
            ViewBag.UserId = id;
            var user = users.FirstOrDefault(c => c.Id == id);
            var bookings = _bookingRepository.GetAll();
            ViewBag.bookingCount = bookings.Count(booking => booking.userId == id && booking.isPaid == 0);
            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }
            return View(cars);
        }
        public IActionResult CreateBooking(int carid)
        {
            Booking booking=new Booking();

            DateOnly startD = DateOnly.FromDateTime(DateTime.Parse(HttpContext.Session.GetString("startDate")));
            DateOnly endD = DateOnly.FromDateTime(DateTime.Parse(HttpContext.Session.GetString("endDate")));
            TimeOnly startT = TimeOnly.FromDateTime(DateTime.Parse(HttpContext.Session.GetString("startTime")));
            TimeOnly endT = TimeOnly.FromDateTime(DateTime.Parse(HttpContext.Session.GetString("endTime")));

            DateTime startDateTime = startD.ToDateTime(startT);
            DateTime endDateTime = endD.ToDateTime(endT);

            booking.startDate = startDateTime;
            booking.endDate = endDateTime;
            booking.startTime = DateTime.Parse(HttpContext.Session.GetString("startTime"));
            booking.endTime = DateTime.Parse(HttpContext.Session.GetString("endTime"));
            booking.pickup = HttpContext.Session.GetString("pickup");
            booking.dropoff = HttpContext.Session.GetString("dropoff");
            booking.userId = HttpContext.Session.GetString("UserId");
            booking.carId = carid;
            booking.totalPrice = _bookingRepository.calculateTotalPrice(booking);
            booking.isPaid = 0;

            HttpContext.Session.SetInt32("carid", carid);

            var car = _carRepository.GetById(carid);
            car.isAvailable = 0;
            _carRepository.Update(car);
            _bookingRepository.Add(booking);


            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult search(string searchQuery)
        {
            HttpContext.Session.SetInt32("Search", 1);
            List<Car> cars = _carRepository.searchCar(searchQuery);
            var users = _userRepository.GetAll();
            string id = HttpContext.Session.GetString("UserId");
            ViewBag.UserId = id;
            var user = users.FirstOrDefault(c => c.Id == id);
            var bookings = _bookingRepository.GetAll();
            ViewBag.bookingCount = bookings.Count(booking => booking.userId == id && booking.isPaid == 0);
            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }
            return View(cars);
        }

        [HttpGet]
        public IActionResult searchResults(string searchQuery)
        {
            List<Car> cars = _carRepository.searchCar(searchQuery);
            return PartialView("~/Views/Home/searchResults.cshtml", cars);
        }

        [HttpPost]
        public IActionResult GetCarsByCategory([FromBody] sortCars sortCars)
        {
            var cars = sortCars.Cars;

            switch (sortCars.sortOrder)
            {
                case "All":
                    cars = cars.ToList();
                    break;

                case "Luxury":
                    cars = cars.Where(c=>c.category=="Luxury").ToList();
                    break;

                case "Sedan":
                    cars = cars.Where(c => c.category == "Sedan").ToList();
                    break;

                default:
                    break;
            }

            return PartialView("~/Views/Home/GetCarsByCategory.cshtml", cars);
        }

        public IActionResult About()
        {
            var users = _userRepository.GetAll();
            string id = HttpContext.Session.GetString("UserId");
            ViewBag.UserId = id;
            var user = users.FirstOrDefault(c => c.Id == id);
            var bookings = _bookingRepository.GetAll();
            ViewBag.bookingCount = bookings.Count(booking => booking.userId == id && booking.isPaid == 0);
            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }

            return View();
        }
    }
}
