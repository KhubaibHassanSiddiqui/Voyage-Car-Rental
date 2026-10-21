namespace Project.Models.Interfaces
{
    public interface IRepository<TEntity>
    {
        void Add(TEntity entity);
        TEntity GetById(dynamic id);
        List<TEntity> GetAll();
        void Update(TEntity entity);
        void Delete(int id);
    }
}
