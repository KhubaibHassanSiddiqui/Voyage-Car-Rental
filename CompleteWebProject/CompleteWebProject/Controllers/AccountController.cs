using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Project.Models.Domains;
using Project.Models.Interfaces;

namespace Project.Controllers
{
    public class AccountController : Controller
    {
        private IUserRepository _userRepository;

        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Login() =>
            RedirectToPage("/Account/Login", new { area = "Identity" });

        [HttpGet]
        public IActionResult SignUpStep1() =>
            RedirectToPage("/Account/Register", new { area = "Identity" });

        [HttpGet]
        public IActionResult SignUpStep2() =>
            RedirectToPage("/Account/Register", new { area = "Identity" });

        //[HttpGet]
        //public IActionResult Login()
        //{
        //    var model = new User
        //    {
        //        //email = HttpContext.Request.Cookies["Email"]
        //    };

        //    return View(model);
        //}

        //[HttpPost]
        //public IActionResult Login(User userModel)
        //{
        //    var user = _userRepository.GetUserByEmail(userModel.email);

        //    if (user != null && user.password == userModel.password)
        //    {
        //        CookieOptions options = new CookieOptions();
        //        options.Expires = DateTime.Now.AddDays(1);
        //        HttpContext.Response.Cookies.Append("Email", userModel.email,options);
        //        HttpContext.Session.SetInt32("UserId", user.Id);
        //        HttpContext.Session.SetString("LoginTime", DateTime.UtcNow.ToString());

        //        if (user.isAdmin==1)
        //        {
        //            return RedirectToAction("Index", "Admin");
        //        }
        //        if (HttpContext.Session.GetInt32("Logged") != null && HttpContext.Session.GetInt32("Logged")==1)
        //        {
        //            return RedirectToAction("Book", "Booking", new { carid = HttpContext.Session.GetInt32("Booked") });
        //        }
        //        if (HttpContext.Session.GetInt32("Logged") != null && HttpContext.Session.GetInt32("Logged")== 3)
        //        {
        //            return RedirectToAction("Booking", "Home");
        //        }
        //        if (HttpContext.Session.GetInt32("Logged") != null && HttpContext.Session.GetInt32("Logged") == 2)
        //        {
        //            return RedirectToAction("AddReview", "Review");
        //        }
        //        return RedirectToAction("Index", "Home");
        //    }

        //    ViewBag.ErrorMessage = "* Wrong email or password";

        //    return View(userModel);
        //}

        //[HttpGet]
        //public ActionResult SignUpStep1()
        //{
        //    var model = new User
        //    {
        //        firstName = HttpContext.Request.Cookies["firstName"],
        //        lastName = HttpContext.Request.Cookies["lastName"],
        //        address = HttpContext.Request.Cookies["address"],
        //        phone = HttpContext.Request.Cookies["phone"],
        //        gender = HttpContext.Request.Cookies["gender"]
        //    };

        //    return View(model);
        //}

        //[HttpGet]
        //public IActionResult SignUpStep2()
        //{
        //    var model = new User
        //    {
        //        firstName = HttpContext.Request.Cookies["firstName"],
        //        lastName = HttpContext.Request.Cookies["lastName"],
        //        address = HttpContext.Request.Cookies["address"],
        //        phone = HttpContext.Request.Cookies["phone"],
        //        gender = HttpContext.Request.Cookies["gender"],
        //        email = HttpContext.Request.Cookies["mail"],
        //        password = HttpContext.Request.Cookies["pass"],
        //        isAdmin = 0
        //    };

        //    return View(model);
        //}

        [HttpGet]
        public IActionResult SignUpComplete()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult SignUpStep1(User user)
        //{
        //    CookieOptions options = new CookieOptions();
        //    options.Expires = DateTime.Now.AddDays(1);

        //    HttpContext.Response.Cookies.Append("firstName", user.firstName, options);
        //    HttpContext.Response.Cookies.Append("lastName", user.lastName, options);
        //    HttpContext.Response.Cookies.Append("address", user.address, options);
        //    HttpContext.Response.Cookies.Append("phone", user.phone, options);
        //    HttpContext.Response.Cookies.Append("gender", user.gender, options);

        //    return RedirectToAction("SignUpStep2");
        //}

        //[HttpPost]
        //public ActionResult SignUpStep2(User userModel)
        //{
        //    var user = _userRepository.GetUserByEmail(userModel.email);

        //    if (user == null)
        //    {
        //        CookieOptions options = new CookieOptions();
        //        options.Expires = DateTime.Now.AddDays(1);
        //        HttpContext.Response.Cookies.Append("mail", userModel.email, options);

        //        userModel.imageUrl = "abc";
        //        userModel.isAdmin = 0;

        //        _userRepository.Add(userModel);
        //        return RedirectToAction("SignUpComplete");
        //    }

        //    ViewBag.ErrorMessage = "Email Already Registered";
        //    return View(userModel);
        //}


        //[HttpPost]
        //public IActionResult SignUpComplete(User user)
        //{

        //    return RedirectToAction("Login");
        //}

        //public async Task<IActionResult> Logout()
        //{
        //    HttpContext.Session.Clear();

        //    await HttpContext.SignOutAsync();

        //    HttpContext.Response.Cookies.Delete("Email");
        //    HttpContext.Response.Cookies.Delete("mail");
        //    HttpContext.Response.Cookies.Delete("firstName");
        //    HttpContext.Response.Cookies.Delete("lastName");
        //    HttpContext.Response.Cookies.Delete("address");
        //    HttpContext.Response.Cookies.Delete("phone");
        //    HttpContext.Response.Cookies.Delete("gender");
        //    HttpContext.Response.Cookies.Delete("mail");
        //    HttpContext.Response.Cookies.Delete("pass");
        //    HttpContext.Response.Cookies.Delete("pickup");
        //    HttpContext.Response.Cookies.Delete("dropoff");
        //    HttpContext.Response.Cookies.Delete("startD");
        //    HttpContext.Response.Cookies.Delete("endD");
        //    HttpContext.Response.Cookies.Delete("startT");
        //    HttpContext.Response.Cookies.Delete("endT");
        //    HttpContext.Response.Cookies.Delete("content");
        //    HttpContext.Response.Cookies.Delete("title");
        //    HttpContext.Response.Cookies.Delete("name");
        //    HttpContext.Response.Cookies.Delete("method");
        //    HttpContext.Response.Cookies.Delete("creditCard");
        //    HttpContext.Response.Cookies.Delete("cvv");
        //    HttpContext.Session.Remove("UserId");
        //    HttpContext.Session.Remove("Price");
        //    HttpContext.Session.Remove("PaymentBookings");
        //    HttpContext.Session.Remove("Cars");
        //    HttpContext.Session.Remove("Profile");

        //    return RedirectToAction("Index", "Home");
        //}
    }
}
