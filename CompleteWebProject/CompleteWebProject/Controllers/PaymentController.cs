using Microsoft.AspNetCore.Mvc;
using Project.Models.Domains;
using Project.Models.Interfaces;
using Project.Models.Repository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Project.Controllers
{
    public class PaymentController : Controller
    {
        private IPaymentRepository _paymentRepository;
        private IUserRepository _userRepository;
        private IBookingRepository _bookingRepository;

        public PaymentController(IPaymentRepository paymentRepository, IUserRepository userRepository, IBookingRepository bookingRepository)
        {
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            float price = float.Parse(HttpContext.Session.GetString("Price"));
            var ids = HttpContext.Session.GetString("PaymentBookings").Split(',').Select(int.Parse).ToList();
            var bookings = _bookingRepository.GetAll().Where(b => ids.Contains(b.Id)).ToList();
            var payments=new List<Payment>();

            foreach (Booking book in bookings)
            {
                var payment = new Payment();
                if (HttpContext.Request.Cookies.ContainsKey("name"))
                {
                    string name = HttpContext.Request.Cookies["name"];
                    string method = HttpContext.Request.Cookies["method"];
                    string expiryDate = HttpContext.Request.Cookies["expiryDate"];
                    string creditCardNo = HttpContext.Request.Cookies["creditCard"];
                    int CVV = int.Parse(HttpContext.Request.Cookies["cvv"]);

                    payment = new Payment
                    {
                        name = name,
                        method = method,
                        expiryDate = expiryDate,
                        creditCardNo = creditCardNo,
                        CVV = CVV

                    };
                }
                payment.userId=book.userId;
                payment.bookingId = book.Id;
                payment.amount = price;

                payments.Add(payment);
            }

            string userid = HttpContext.Session.GetString("UserId");
            var booking = _bookingRepository.GetAll();
            ViewBag.bookingCount = bookings.Count(booking => booking.userId == userid && booking.isPaid == 0);
            var users = _userRepository.GetAll();
            var user = users.FirstOrDefault(c => c.Id == userid);
            if (user != null)
            {
                ViewBag.Username = $"{user.firstName} {user.lastName}";
                ViewBag.url = user.imageUrl;
            }

            var viewLists=new ViewLists { bookList = booking, paymentList=payments };

            return View(viewLists);
        }

        [HttpPost]
        public IActionResult Index(ViewLists viewLists)
        {
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddDays(1);

            HttpContext.Response.Cookies.Append("name", viewLists.paymentList[0].name, options);
            HttpContext.Response.Cookies.Append("method",viewLists.paymentList[0].method,options);
            HttpContext.Response.Cookies.Append("date",viewLists.paymentList[0].expiryDate,options);
            HttpContext.Response.Cookies.Append("creditCard",viewLists.paymentList[0].creditCardNo,options);
            HttpContext.Response.Cookies.Append("cvv",viewLists.paymentList[0].CVV.ToString(),options);

            for(int i = 0; i < viewLists.bookList.Count; i++)
            {
                var payment = viewLists.paymentList[0];
                payment.bookingId = viewLists.bookList[i].Id;
                payment.userId = HttpContext.Session.GetString("UserId");
                payment.amount = float.Parse(HttpContext.Session.GetString("Price"));
                _paymentRepository.Add(payment);
                var booking = _bookingRepository.GetById(viewLists.bookList[i].Id);
                booking.isPaid = 1;
                _bookingRepository.Update(booking);
            }

            return RedirectToAction("BookingComplete");
        }

        public IActionResult BookingComplete()
        {
            string userid = HttpContext.Session.GetString("UserId");
            var users = _userRepository.GetAll();
            var user = users.FirstOrDefault(c => c.Id == userid);
            if (user != null)
            {
                ViewBag.Username = user.firstName;
                ViewBag.url = user.imageUrl;
            }

            return View();
        }
    }
}
