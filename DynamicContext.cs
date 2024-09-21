using Dynamic2Db.Models;
using Dynamic2Db.Services;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.ClickHouse;
using System.Reflection;
using static LinqToDB.DataExtensions;

namespace Dynamic2Db
{
    public class TestModel
    {
        public string Name { get; set; }
        public string Data { get; set; }
    }
    public class DynamicContext: DataConnection
    {
        private DataModel _dataModel;
        public DynamicContext(DataOptions dataOptions, DataModel dataModel) : base(dataOptions)
        {
            _dataModel = dataModel;
        }
        public static DynamicContext Create(DataModel dataModel)
        {
            var options = default(DataOptions);

            if (dataModel.ConnectionType == DataModelConnectionType.MsSql)
                options= new DataOptions().UseSqlServer(dataModel.ConnectionString);
            else
                if (dataModel.ConnectionType == DataModelConnectionType.ClickHouse)
                options = new DataOptions().UseClickHouse(ClickHouseProvider.ClickHouseClient, dataModel.ConnectionString);
            else
                throw new NotSupportedException($"Dynamic Context doesn't support connection type:{dataModel.ConnectionType}");

            return new DynamicContext(options, dataModel);
        }

        public IQueryable<TEntity> Set<TEntity>(string sql, params object?[] parameters)
        {
            var result =  this.FromSql<TEntity>(sql, parameters);

            return result;
        }

        public IQueryable Query(string typeName)
        {
            var query = _dataModel.Query(typeName);

            if (query == null)
               throw new ArgumentNullException($"Type not found: {typeName}");


            var clrType = query.ClrType;

            var context = this;

            var metods = typeof(DynamicContext).GetMethods();

            MethodInfo setMethod = metods.Where(x => x.Name == "Set" && x.GetParameters().Length == 2).First();

            MethodInfo methodInfo = setMethod.MakeGenericMethod(clrType);

            object? obj;
            if ((object)methodInfo == null)
            {
                obj = null;
            }
            else
            {
                object[] parameters = new object[2] {
                    query.Query,
                    new object?[0]{ }
                };
                obj = methodInfo.Invoke(context, parameters);
            }

            if (obj == null)
                throw new Exception("Type not found: " + clrType.FullName);


            return ((IQueryable)obj) ?? throw new Exception("Type not found: " + clrType);
        }

    }
}
