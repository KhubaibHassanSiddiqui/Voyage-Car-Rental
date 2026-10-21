using Microsoft.AspNetCore.Identity;
namespace Project.Models.Domains
{
    public class AspNetUsers:IdentityUser
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; } = "170 E-5";
        public string gender { get; set; } = "Male";
        public string imageUrl { get; set; } = "/img/user.jpg";
        public int isAdmin { get; set; } = 0;
    }

}
