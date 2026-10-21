using Project.Models.Domains;
using Project.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Project.Models.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly IRepository<Review> _reviewRepository;
        public ReviewRepository(IRepository<Review> reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }
        public void Add(Review review)
        {
            _reviewRepository.Add(review);
        }

        public void Update(Review review)
        {
            _reviewRepository.Update(review);
        }

        public void Delete(int id)
        {
            _reviewRepository.Delete(id);
        }

        public Review GetById(dynamic id)
        {
            return _reviewRepository.GetById(id);
        }

        public List<Review> GetAll()
        {
            return _reviewRepository.GetAll();
        }
    }
}
