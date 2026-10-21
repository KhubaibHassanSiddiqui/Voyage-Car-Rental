using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Project.Models.Domains;
using Project.Models.Interfaces;

namespace Project.Controllers
{
    public class UserController : Controller
    {
        private IRepository<Car> _carRepository;
        private IRepository<AspNetUsers> _userRepository;
        private IRepository<Booking> _bookingRepository;
        private IRepository<Review> _reviewRepository;
        private IRepository<Payment> _paymentRepository;

        public UserController(IRepository<Car> carRepository, IRepository<AspNetUsers> userRepository, IRepository<Booking> bookingRepository, IRepository<Review> reviewRepository, IRepository<Payment> paymentRepository)
        {
            _carRepository = carRepository;
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
            _reviewRepository = reviewRepository;
            _paymentRepository = paymentRepository;
        }

        public IActionResult Index()
        {
            var users = _userRepository.GetAll();
            string id = HttpContext.Session.GetString("UserId");
            var bookings = _bookingRepository.GetAll();
            ViewBag.bookingCount = bookings.Count(booking => booking.userId == id && booking.isPaid == 0);
            var user = users.FirstOrDefault(c => c.Id == id);
            var payments = _paymentRepository.GetAll();

            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }

            return View(user);
        }

        public IActionResult manageBookings()
        {
            string id = HttpContext.Session.GetString("UserId");
            var bookingsList = _bookingRepository.GetAll().Where(c => c.userId == id && c.isPaid == 1);
            List<Booking> bookings = bookingsList.ToList();
            var cars = new List<Car>();

            foreach (var booking in bookings)
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

            var viewLists = new ViewLists { bookList = bookings, carList = cars };

            return View(viewLists);
        }

        public IActionResult manageReviews()
        {
            string id = HttpContext.Session.GetString("UserId");

            var reviews = _reviewRepository.GetAll().Where(r => r.userId == id).ToList();
            var users = new List<AspNetUsers>();
            foreach (var review in reviews)
            {
                var newuser = _userRepository.GetById(review.userId);
                users.Add(newuser);
            }

            var userList = _userRepository.GetAll();
            var user = userList.FirstOrDefault(c => c.Id == id);
            var book = _bookingRepository.GetAll();
            ViewBag.bookingCount = book.Count(booking => booking.userId == id && booking.isPaid == 0);
            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }

            var viewLists = new ViewLists { reviewList = reviews, userList = users };

            return View(viewLists);
        }

        [HttpGet]
        public IActionResult editProfile()
        {
            var users = _userRepository.GetAll();
            string id = HttpContext.Session.GetString("UserId");
            var bookings = _bookingRepository.GetAll();
            var user = users.FirstOrDefault(c => c.Id == id);
            var payments = _paymentRepository.GetAll();

            var model = new AspNetUsers
            {
                firstName = HttpContext.Request.Cookies["firstName"],
                lastName = HttpContext.Request.Cookies["lastName"],
                address = HttpContext.Request.Cookies["address"],
                gender=user.gender
            };

            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult editProfile(AspNetUsers user, IFormFile imageUrl)
        {
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddDays(1);

            HttpContext.Response.Cookies.Append("firstName", user.firstName, options);
            HttpContext.Response.Cookies.Append("lastName", user.lastName, options);
            HttpContext.Response.Cookies.Append("address", user.address, options);

            if (imageUrl != null && imageUrl.Length > 0)
            {
                var fileName = Path.GetFileName(imageUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageUrl.CopyTo(stream);
                }

                user.imageUrl = "/uploads/" + fileName;
            }

            user.Id = HttpContext.Session.GetString("UserId");

            _userRepository.Update(user);

            HttpContext.Session.SetInt32("Profile", 1);

            return RedirectToAction("Index");
        }

        public IActionResult deleteBookings(int id)
        {
            var booking = _bookingRepository.GetById(id);
            var car = _carRepository.GetById(booking.carId);
            car.isAvailable = 1;
            _carRepository.Update(car);
            _bookingRepository.Delete(id);
            return RedirectToAction("manageBookings");
        }

        public IActionResult deleteReviews(int id)
        {
            _reviewRepository.Delete(id);

            return RedirectToAction("manageReviews");
        }
    }
}
