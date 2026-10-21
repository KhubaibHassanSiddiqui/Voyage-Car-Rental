using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using Project.Models.Domains;
using Project.Models.Interfaces;
using System.Reflection;

namespace Project.Controllers
{
    public class AdminController : Controller
    {
        private IRepository<Car> _carRepository;
        private IRepository<AspNetUsers> _userRepository;
        private IRepository<Booking> _bookingRepository;
        private IRepository<Review> _reviewRepository;
        private IRepository<Payment> _paymentRepository;

        public AdminController(IRepository<Car> carRepository, IRepository<AspNetUsers> userRepository, IRepository<Booking> bookingRepository, IRepository<Review> reviewRepository, IRepository<Payment> paymentRepository)
        {
            _carRepository = carRepository;
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
            _reviewRepository = reviewRepository;
            _paymentRepository = paymentRepository;
        }

        public IActionResult Index()
        {
            var currentYear = DateTime.Now.Year;
            var monthlyTotals = Enumerable.Repeat(0f, 12).ToList();

            var totals = _bookingRepository.GetAll()
                .Where(b => b.startDate.Year == currentYear)
                .GroupBy(b => b.startDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalPrice = g.Sum(b => b.totalPrice)
                })
                .ToList();

            foreach (var t in totals)
            {
                monthlyTotals[t.Month - 1] = (float)t.TotalPrice;
            }
            var categories = new List<string>
            {
                "Luxury",  // Index 0
                "Sedan",   // Index 1
                "SUV",      // Index 2
                "Convertible", // Index 3
                "hatchback"     // Index 4
            };

            var categoryCounts = Enumerable.Repeat(0, categories.Count).ToList();

            var counts = _carRepository.GetAll()
                .GroupBy(c => c.category)
                .Select(g => new
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .ToList();

            foreach (var category in categories)
            {
                var count = counts.FirstOrDefault(c => c.Category == category);
                categoryCounts[categories.IndexOf(category)] = count != null ? count.Count : 0;
            }

            var users = _userRepository.GetAll();
            string id = HttpContext.Session.GetString("UserId");
            var bookings = _bookingRepository.GetAll();
            var user = users.FirstOrDefault(c => c.Id == id);
            var payments=_paymentRepository.GetAll();
            var amount = payments.GroupBy(b => b.bookingId).Select(b => b.First()).ToList();
            float total = 0;
            foreach (var a in amount)
            {
                total += a.amount;
            }
            if (user != null)
            {
                ViewBag.total = total;
                ViewBag.Username = $"{user.firstName}";
                ViewBag.url = user.imageUrl;
            }
            statistics stats = new statistics { floats = monthlyTotals, ints = categoryCounts };
            return View(stats);
        }

        public IActionResult Profile()
        {
            var users = _userRepository.GetAll();
            string id = HttpContext.Session.GetString("UserId");
            var bookings = _bookingRepository.GetAll();
            var user = users.FirstOrDefault(c => c.Id == id);
            var payments = _paymentRepository.GetAll();

            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }

            return View(user);
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
                gender = user.gender
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

            return RedirectToAction("Profile", "Admin");
        }

        public IActionResult manageCars()
        {
            var cars = _carRepository.GetAll();
            return View(cars);
        }

        [HttpGet]
        public IActionResult addCars()
        {
            var model = new Car
            {
                make = HttpContext.Request.Cookies["make"],
                model = HttpContext.Request.Cookies["model"],
                fuelType = HttpContext.Request.Cookies["fuelType"],
                category = HttpContext.Request.Cookies["category"],
                isAvailable = 1
            };
            var users = _userRepository.GetAll();
            string id = HttpContext.Session.GetString("UserId");
            var bookings = _bookingRepository.GetAll();
            var user = users.FirstOrDefault(c => c.Id == id);
            var payments = _paymentRepository.GetAll();

            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult addCars(Car car, IFormFile imageUrl)
        {
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddDays(1);

            HttpContext.Response.Cookies.Append("make", car.make, options);
            HttpContext.Response.Cookies.Append("model", car.model, options);
            HttpContext.Response.Cookies.Append("year", car.year.ToString(), options);
            HttpContext.Response.Cookies.Append("fuelType", car.fuelType, options);
            HttpContext.Response.Cookies.Append("category", car.category, options);
            HttpContext.Response.Cookies.Append("price", car.pricePerDay.ToString(), options);

            if (imageUrl != null && imageUrl.Length > 0)
            {
                var fileName = Path.GetFileName(imageUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageUrl.CopyTo(stream);
                }

                car.imageUrl = "/uploads/" + fileName;
            }

            _carRepository.Add(car);

            HttpContext.Session.SetInt32("Cars", 1);

            return RedirectToAction("manageCars", "Admin");
        }

        public IActionResult manageBookings()
        {
            var bookingsList = _bookingRepository.GetAll().Where(c=>c.isPaid == 1);
            List<Booking> bookings = bookingsList.ToList();
            var cars = new List<Car>();

            foreach (var booking in bookings)
            {
                var newcar = _carRepository.GetById(booking.carId);
                cars.Add(newcar);
            }
            var users = _userRepository.GetAll();
            string id = HttpContext.Session.GetString("UserId");
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

        public IActionResult manageUsers()
        {
            var users = _userRepository.GetAll().Where(u => u.isAdmin == 0).ToList();
            return View(users);
        }

        public IActionResult manageReviews()
        {
            var reviews = _reviewRepository.GetAll();
            var users=new List<AspNetUsers>();
            foreach (var review in reviews)
            {
                var newuser = _userRepository.GetById(review.userId);
                users.Add(newuser);
            }

            string id = HttpContext.Session.GetString("UserId");
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

        public IActionResult deleteUsers(int id)
        {
            _userRepository.Delete(id);

            return RedirectToAction("manageUsers");
        }
        public IActionResult deleteCars(int id)
        {
            _carRepository.Delete(id);
            return RedirectToAction("manageCars");
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
