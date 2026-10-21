using Project.Models.Domains;
using Project.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Project.Models.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IRepository<Payment> _paymentRepository;
        public PaymentRepository(IRepository<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }
        public void Add(Payment payment)
        {
            _paymentRepository.Add(payment);
        }

        public void Update(Payment payment)
        {
            _paymentRepository.Update(payment);
        }

        public void Delete(int id)
        {
            _paymentRepository.Delete(id);
        }

        public Payment GetById(dynamic id)
        {
            return _paymentRepository.GetById(id);
        }

        public List<Payment> GetAll()
        {
            return _paymentRepository.GetAll();
        }
    }
}
