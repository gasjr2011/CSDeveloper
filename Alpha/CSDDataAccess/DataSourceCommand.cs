using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
namespace CSDeveloper.DataAccess
{
    public delegate T TransformCurrentRecord<T>(IDataReader reader);
    public class DataSourceCommand<T>
    {
        public DataSourceCommandType Type = DataSourceCommandType.Query;
        public string Command = string.Empty;
        public TransformCurrentRecord<T> Transformer = default(TransformCurrentRecord<T>);
        public Dictionary<string, object> Parameters = new Dictionary<string, object>();
        public object Result = default(object);
    }
}
