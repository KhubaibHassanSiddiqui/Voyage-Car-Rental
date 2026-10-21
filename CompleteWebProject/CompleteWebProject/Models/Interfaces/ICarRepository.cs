using Project.Models.Domains;

namespace Project.Models.Interfaces
{
    public interface ICarRepository:IRepository<Car>
    {
        List<Car> searchCar(string query);
        List<Car> search(carSearch model);
    }

}
