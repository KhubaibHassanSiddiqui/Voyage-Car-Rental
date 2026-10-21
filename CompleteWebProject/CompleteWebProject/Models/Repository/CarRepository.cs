using Project.Models.Domains;
using Project.Models.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Project.Models.Repository
{
    public class CarRepository : ICarRepository
    {
        private readonly IRepository<Car> _carRepository;

        public CarRepository(IRepository<Car> carRepository)
        {
            _carRepository = carRepository;
        }
        public void Add(Car car)
        {
            _carRepository.Add(car);
        }

        public void Update(Car car)
        {
            _carRepository.Update(car);
        }

        public void Delete(int id)
        {
            _carRepository.Delete(id);
        }

        public Car GetById(dynamic id)
        {
            return _carRepository.GetById(id);
        }

        public List<Car> GetAll()
        {
            return _carRepository.GetAll();
        }

        public List<Car> searchCar(string query)
        {
            var all= _carRepository.GetAll();

            var cars= new List<Car>();

            if (query != null)
            {
                foreach (var car in all)
                {
                    if (car.make.ToLower().Contains(query.ToLower()) || car.model.ToLower().Contains(query.ToLower()) || car.fuelType.ToLower().Contains(query.ToLower()) || car.category.ToLower().Contains(query.ToLower()))
                    {
                        cars.Add(car);
                    }
                }

                return cars;
            }

            else
                return all;
        }

        public List<Car> search(carSearch model)
        {
            var all = _carRepository.GetAll();

            var cars = new List<Car>();

            if (model != null)
            {
                foreach (var car in all)
                {
                    if (car.fuelType.ToLower() == model.fuelType.ToLower() && car.category.ToLower() == (model.category.ToLower()) && car.year == model.year && (car.pricePerDay >= model.minPrice && car.pricePerDay <= model.maxPrice))
                    {
                        cars.Add(car);
                    }
                }
                return cars;
            }

            else
                return all;
        }
    }
}
