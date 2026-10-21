using Project.Models.Domains;

namespace Project.Models.Interfaces
{
    public interface IUserRepository : IRepository<AspNetUsers>
    {
        AspNetUsers GetUserByEmail(string email);
    }

}
