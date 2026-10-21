using Project.Models.Domains;
using Project.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Project.Models.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IRepository<AspNetUsers> _userRepository;
        public UserRepository(IRepository<AspNetUsers> userRepository)
        {
            _userRepository = userRepository;
        }
        public void Add(AspNetUsers user)
        {
            _userRepository.Add(user);
        }

        public void Update(AspNetUsers user)
        {
            _userRepository.Update(user);
        }

        public void Delete(int id)
        {
            _userRepository.Delete(id);
        }

        public AspNetUsers GetById(dynamic id)
        {
            return _userRepository.GetById(id);
        }

        public List<AspNetUsers> GetAll()
        {
            return _userRepository.GetAll();
        }

        public AspNetUsers GetUserByEmail(string email)
        {
            foreach (var user in GetAll())
            {
                if(user.Email == email)
                {
                    return user;
                }
            }

            return null;
        }
    }
}
