using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Project.Models.Interfaces;

namespace Project.Models.Repository
{
    public class GenericRepository<TEntity>: IRepository<TEntity> where TEntity:class
    {
        private readonly string _connectionString;

        public GenericRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public void Add(TEntity entity)
        {
            SqlConnection conn=new SqlConnection(_connectionString);

            conn.Open();

            var tableName=typeof(TEntity).Name;

            var properties=typeof(TEntity).GetProperties();

            var columnNames=new List<string>();

            var parameterNames = new List<string>();

            foreach (var property in properties)
            {
                if (property.Name != "Id")
                {
                    columnNames.Add(property.Name);

                    parameterNames.Add("@"+property.Name);
                }
            }

            string columnString = string.Join(",", columnNames);

            string parameterString = string.Join(",", parameterNames);

            string query = $"insert into [{tableName}] ({columnString}) values ({parameterString});";

            SqlCommand cmd=new SqlCommand(query, conn);

            foreach (var property in properties)
            {
                cmd.Parameters.AddWithValue("@" + property.Name, property.GetValue(entity));
            }

            cmd.ExecuteNonQuery();
        }

        public void Update(TEntity entity)
        {
            SqlConnection conn = new SqlConnection(_connectionString);

            conn.Open();

            var tableName = typeof(TEntity).Name;

            string pK = "Id";

            var properties = typeof(TEntity).GetProperties();

            var setList = new List<string>();

            foreach (var property in properties)
            {
                if (property.Name != "Id")
                {
                    setList.Add($"{property.Name} = @{property.Name}");
                }
            }

            string setClause = string.Join(",", setList);

            string query = $"update [{tableName}] set {setClause} where {pK} = @{pK}";

            SqlCommand cmd = new SqlCommand(query, conn);

            foreach (var property in properties)
            {
                if (property.Name != pK)
                {
                    cmd.Parameters.AddWithValue("@" + property.Name, property.GetValue(entity));
                }
            }
            cmd.Parameters.AddWithValue("@" + pK, typeof(TEntity).GetProperty(pK).GetValue(entity));

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);

            conn.Open();

            var tableName = typeof(TEntity).Name;

            string pK = "Id";

            string query = $"Delete from [{tableName}] where {pK} = @{pK}";

            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@" + pK, id);

            cmd.ExecuteNonQuery();
        }

        public TEntity GetById(dynamic id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);

            conn.Open();

            var tableName = typeof(TEntity).Name;

            string pK = "Id";

            string query = $"select * from [{tableName}] where {pK} = @{pK}";

            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@" + pK, id);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                var entity = Activator.CreateInstance<TEntity>();
                foreach (var property in typeof(TEntity).GetProperties())
                {
                    if (dr.HasRows && dr.GetOrdinal(property.Name) >= 0)
                    {
                        var columnType = dr.GetFieldType(dr.GetOrdinal(property.Name));

                        var value = dr.GetValue(dr.GetOrdinal(property.Name));
                        if (value != DBNull.Value)
                        {
                            var targetType = property.PropertyType;

                            if (targetType == typeof(float) && columnType == typeof(double))
                            {
                                property.SetValue(entity, Convert.ToSingle(value));
                            }
                            else if (targetType == typeof(double) && columnType == typeof(float))
                            {
                                property.SetValue(entity, Convert.ToDouble(value));
                            }
                            else
                            {
                                property.SetValue(entity, Convert.ChangeType(value, targetType));
                            }
                        }
                    }
                }
                return entity;
            }

            return null;
        }

        public List<TEntity> GetAll()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var list = new List<TEntity>();
                var tableName = typeof(TEntity).Name;

                string query = $"SELECT * FROM [{tableName}]";
                using (var cmd = new SqlCommand(query, conn))
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var entity = Activator.CreateInstance<TEntity>();
                        foreach (var property in typeof(TEntity).GetProperties())
                        {
                            Console.WriteLine(property.Name + ": " + property.GetValue(entity));
                            if (dr.HasRows && dr.GetOrdinal(property.Name) >= 0)
                            {
                                var columnType = dr.GetFieldType(dr.GetOrdinal(property.Name));
                                var value = dr.GetValue(dr.GetOrdinal(property.Name));

                                if (value != DBNull.Value)
                                {
                                    var targetType = property.PropertyType;
                                    if (targetType == typeof(DateOnly))
                                    {
                                        if (columnType == typeof(DateTime))
                                        {
                                            var dateTimeValue = (DateTime)value;
                                            property.SetValue(entity, DateOnly.FromDateTime(dateTimeValue));
                                        }
                                    }
                                    else if (targetType == typeof(TimeOnly))
                                    {
                                        if (columnType == typeof(DateTime))
                                        {
                                            var dateTimeValue = (DateTime)value;
                                            property.SetValue(entity, TimeOnly.FromDateTime(dateTimeValue));
                                        }
                                    }
                                    else if (targetType == typeof(float) && columnType == typeof(double))
                                    {
                                        property.SetValue(entity, Convert.ToSingle(value));
                                    }
                                    else if (targetType == typeof(double) && columnType == typeof(float))
                                    {
                                        property.SetValue(entity, Convert.ToDouble(value));
                                    }
                                    else
                                    {
                                        property.SetValue(entity, Convert.ChangeType(value, targetType));
                                    }
                                }
                            }
                        }
                        list.Add(entity);
                    }
                }
                return list;
            }
        }


    }
}
