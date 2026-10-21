using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Project.Models.Domains;
using Project.Models.Interfaces;
using Project.Models.Repository;

namespace Project.Controllers
{
    public class ReviewController : Controller
    {
        private IReviewRepository _reviewRepository;
        private IUserRepository _userRepository;
        private IBookingRepository _bookingRepository;

        public ReviewController(IReviewRepository reviewRepository, IUserRepository userRepository, IBookingRepository bookingRepository)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
        }
        public IActionResult Index()
        {
            var reviews= _reviewRepository.GetAll();
            string id = HttpContext.Session.GetString("UserId");
            var users = _userRepository.GetAll();
            var user = users.FirstOrDefault(c => c.Id == id);
            var bookings = _bookingRepository.GetAll();
            ViewBag.bookingCount = bookings.Count(booking => booking.userId == id && booking.isPaid == 0);
            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }

            var viewLists=new ViewLists { userList = users, reviewList=reviews };
            return View(viewLists);
        }

        [HttpGet]
        public IActionResult AddReview()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                HttpContext.Session.SetInt32("Logged", 2);
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            string id = HttpContext.Session.GetString("UserId");
            var model = new Review
            {
                userId = id,

                title = HttpContext.Request.Cookies["title"],

                content = HttpContext.Request.Cookies["content"]
            };

            var users = _userRepository.GetAll();
            var user = users.FirstOrDefault(c => c.Id == id);
            var bookings = _bookingRepository.GetAll();
            ViewBag.bookingCount = bookings.Count(booking => booking.userId == id && booking.isPaid == 0);
            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult AddReview(Review review)
        {
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddDays(1);

            HttpContext.Response.Cookies.Append("title", review.title, options);
            HttpContext.Response.Cookies.Append("content", review.content, options);

            _reviewRepository.Add(review);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(dynamic id)
        {
            var bookings = _bookingRepository.GetAll();
            ViewBag.bookingCount = bookings.Count(booking => booking.userId == id && booking.isPaid == 0);
            return View();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _reviewRepository.Delete(id);

            return RedirectToAction("Index");
        }
    }
}
