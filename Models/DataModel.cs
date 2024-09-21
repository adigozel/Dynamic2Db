
using System.Collections.Generic;

namespace Dynamic2Db.Models
{
    public class DataModel
    {
        private readonly IDictionary<string, DataQuery> _queries;
        private readonly IDictionary<string, DataQuerySet> _sets;

        public DataModel( IList<DataQuery> queries, IList<DataQuerySet> sets, DataModelConnectionType connectioType, string connectionString)
        {
            _queries = queries != null ? 
                queries.ToDictionary(x => x.TypeName, x => x)
                : new Dictionary<string, DataQuery>();

            _sets = sets != null ?
                sets.ToDictionary(x => x.Name, x => x)
                : new Dictionary<string, DataQuerySet>();

            ConnectionType = connectioType;
            ConnectionString = connectionString;
        }

        public DataQuery? Query(string typeName) => _queries.ContainsKey(typeName) ? _queries[typeName] : null;
        public DataQuerySet? Set(string name) => _sets.ContainsKey(name) ? _sets[name] : null;

        public ICollection<DataQuery> Queries => _queries.Values;
        public ICollection<DataQuerySet> Sets => _sets.Values;
        public DataModelConnectionType ConnectionType { get; set; }
        public string ConnectionString { get; set; }

    }
    public enum DataModelConnectionType
    {
        MsSql,
        ClickHouse
    }

    public class DataQuery
    {
        public DataQuery(string typeName, Type clrType, string query, IList<DataQueryField> fields)
        {
            if (string.IsNullOrEmpty(typeName) )
                throw new ArgumentNullException("DataQuery's TypeName is null");
            else
                if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException("DataQuery's  Query is null");
            else
                if (fields == null || fields.Count == 0)
                throw new ArgumentNullException("DataQuery's  Fields is null or Count is 0");

            TypeName = typeName;
            ClrType = clrType;
            Query = query;
            Fields = fields;
        }
        

        public string TypeName { get; set; }
        public Type ClrType { get; set; }
        public string Query { get; set; }
        public IList<DataQueryField> Fields { get; set; }
    }
   
    public class DataQueryField
    {
        public DataQueryField(string name, Type clrType)
        {
            Name = name;
            ClrType = clrType;
        }

        public string Name { get; set; }
        public Type ClrType { get; set; }
    }

    public class DataQuerySet
    {
        public DataQuerySet(string name, string mainDataQuery, string mainDataQueryField, IList<DataQuerySetItem> items)
        {
            Name = name;
            MainDataQuery = mainDataQuery;
            MainDataQueryField = mainDataQueryField;
            Items = items;
        }

        public string Name { get; set; }
        public string MainDataQuery { get; set; }
        public string MainDataQueryField { get; set; }
        public IList<DataQuerySetItem> Items { get; set; }
    }

    public class DataQuerySetItem
    {
        public DataQuerySetItem(string dataQuery, string dataQueryField)
        {
            DataQuery = dataQuery;
            DataQueryField = dataQueryField;
        }

        public string DataQuery { get; set; }
        public string DataQueryField { get; set; }
    }
}
